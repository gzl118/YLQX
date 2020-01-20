using MedicalApparatusManage.Common;
using MedicalApparatusManage.Domain;
using MedicalApparatusManage.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace MedicalApparatusManage.Controllers
{
    public class T_CGHZController : Controller
    {
        public int resultCount = 0; // 总条数 
        // GET: T_CGHZ
        public ActionResult Index(T_CGMXModels evalModel)
        {
            try
            {
                evalModel.currentPage = int.Parse(Request["pageNum"].ToString());
            }
            catch { }
            string order = "";
            string qyid = "";
            try
            {
                order = Request["orderField"].ToString();
            }
            catch { }
            if (order.Trim() == "${param.orderField}")
            {
                order = "";
            }
            evalModel.DataModel = evalModel.DataModel ?? new T_CGMX();

            //加载产品生产企业
            T_SupQYModels qymode = new T_SupQYModels();
            qymode.DataModel = qymode.DataModel ?? new T_SupQY();
            qymode.DataList = T_SupQYDomain.GetInstance().GetAllT_SupQY(qymode.DataModel);
            ViewData["QY"] = new SelectList(qymode.DataList, "SupID", "SupMC");
            if (Request["QYList"] != null)
            {
                qyid = Request["QYList"].ToString();
                if (!String.IsNullOrEmpty(qyid))
                {
                    T_SupQY qycp = new T_SupQY();
                    qycp.SupID = int.Parse(qyid);
                    evalModel.DataModel.T_SupQY1 = qycp;
                }
            }
            ViewData["QYList"] = qyid;

            Expression<Func<T_YLCP, bool>> where = PredicateBuilder.True<T_YLCP>();
            var lst = T_YLCPDomain.GetInstance().GetAllModels<int>(where);
            ViewData["CGHZ_YLCP"] = new SelectList(lst, "CPID", "CPMC");

            var cpid = "";
            if (Request["CGHZ_CPID"] != null)
            {
                cpid = Request["CGHZ_CPID"].ToString();
                if (!String.IsNullOrEmpty(cpid))
                {
                    evalModel.DataModel.CPID = int.Parse(cpid);
                }
            }
            ViewData["CGHZ_CPID"] = cpid;

            int pagesize = Convert.ToInt32(evalModel.pageSize);
            int pagecount = Convert.ToInt32(evalModel.pagecount);
            int currentPage = Convert.ToInt32(evalModel.currentPage);
            evalModel.DataModel = evalModel.DataModel ?? new T_CGMX();
            evalModel.DataList = T_CGMXDomain.GetInstance().PageT_CGMX(evalModel.DataModel, evalModel.StartTime, evalModel.EndTime, currentPage, pagesize, out pagecount, out resultCount);
            evalModel.resultCount = resultCount;

            var totalNum = 0;
            if (evalModel.DataList != null && evalModel.DataList.Count > 0)
            {
                evalModel.DataList.ForEach(p =>
                {
                    totalNum += p.CPNUM == null ? 0 : (int)p.CPNUM;
                });
            }
            ViewBag.TotalNum = totalNum;
            return View("~/Views/T_CGHZ/Index.cshtml", evalModel);
        }
        #region excel导出、打印

        [CheckLogin()]
        public JsonResult ExportExcel(ExcelModel eModel)
        {
            var curModel = new T_CGMX();
            if (eModel.SCQYID != null)
            {
                T_SupQY qycp = new T_SupQY();
                qycp.SupID = (int)eModel.SCQYID;
                curModel.T_SupQY1 = qycp;
            }
            if (eModel.CPID != null)
            {
                curModel.CPID = (int)eModel.CPID;
            }

            int pagesize = eModel.PageSize;
            int pagecount = 0;
            int currentPage = eModel.PageNum;
            var DataList = T_CGMXDomain.GetInstance().PageT_CGMX(curModel, eModel.StartDate, eModel.EndDate, currentPage, pagesize, out pagecount, out resultCount);
            var str = ExportExcelPR(DataList);
            //调用输出Excel表的方法
            ExportToExcel("application/vnd.ms-excel", "采购汇总.xls", str);
            return Json("true");
        }

        [CheckLogin()]
        public void ExportToExcel(string FileType, string FileName, string ExcelContent)
        {
            System.Web.HttpContext.Current.Response.Charset = "UTF-8";
            System.Web.HttpContext.Current.Response.ContentEncoding = System.Text.Encoding.GetEncoding("UTF-8");
            var fName = HttpUtility.UrlEncode(FileName, System.Text.Encoding.GetEncoding("UTF-8")).ToString();
            System.Web.HttpContext.Current.Response.AppendHeader("Content-Disposition", "attachment;filename=" + fName);
            System.Web.HttpContext.Current.Response.ContentType = FileType;
            System.IO.StringWriter tw = new System.IO.StringWriter();
            System.Web.HttpContext.Current.Response.Output.Write(ExcelContent.ToString());
            System.Web.HttpContext.Current.Response.Flush();
            System.Web.HttpContext.Current.Response.End();
        }

        [CheckLogin()]
        public string ExportExcelPR(List<T_CGMX> listMX)
        {
            if (listMX == null)
                listMX = new List<T_CGMX>();
            var totalNum = 0;
            if (listMX != null && listMX.Count > 0)
            {
                listMX.ForEach(p =>
                {
                    totalNum += p.CPNUM == null ? 0 : (int)p.CPNUM;
                });
            }

            //命名导出表格的StringBuilder变量
            StringBuilder sHtml = new StringBuilder(string.Empty);

            sHtml.Append("<table border=\"1\" width=\"100%\"  style='border-collapse:collapse;border:1px solid black;'>");
            sHtml.Append("<tr height=\"30\" align=\"center\"><th>采购单号</th><th>产品名称</th><th>产品数量</th><th>产品单位</th><th>进货单价(元)</th><th>供货企业</th><th>生产企业</th><th>产品规格</th><th>产品型号</th><th>许可证号</th><th>注册证号</th><th>采购日期</th></tr>");

            for (int i = 0; i < listMX.Count; i++)
            {
                var item = listMX[i];
                sHtml.Append("<tr height=\"30\" align=\"center\"><td>" + (item.T_CGD != null && !string.IsNullOrEmpty(item.T_CGD.CGDH) ? item.T_CGD.CGDH : "")
                            + "</td><td>" + (item.T_YLCP != null && !string.IsNullOrEmpty(item.T_YLCP.CPMC) ? item.T_YLCP.CPMC : "")
                            + "</td><td>" + item.CPNUM
                            + "</td><td>" + (item.T_YLCP != null && !string.IsNullOrEmpty(item.T_YLCP.CPDW) ? item.T_YLCP.CPDW : "")
                            + "</td><td>" + item.CPPRICE + "</td><td>" + (item.T_SupQY != null && !string.IsNullOrEmpty(item.T_SupQY.SupMC) ? item.T_SupQY.SupMC : "")
                            + "</td><td>" + (item.T_SupQY1 != null && !string.IsNullOrEmpty(item.T_SupQY1.SupMC) ? item.T_SupQY1.SupMC : "") + "</td><td>" + (item.T_YLCP != null && !string.IsNullOrEmpty(item.T_YLCP.CPGG) ? item.T_YLCP.CPGG : "")
                            + "</td><td>" + (item.T_YLCP != null && !string.IsNullOrEmpty(item.T_YLCP.CPXH) ? item.T_YLCP.CPXH : "")
                            + "</td><td>" + (item.T_SupQY1 != null && !string.IsNullOrEmpty(item.T_SupQY1.SupXKZBH) ? item.T_SupQY1.SupXKZBH : "")
                            + "</td><td>" + (item.T_YLCP != null && !string.IsNullOrEmpty(item.T_YLCP.CPZCZ) ? item.T_YLCP.CPZCZ : "")
                            + "</td><td>" + (item.T_CGD != null && item.T_CGD.CGRQ.HasValue ? item.T_CGD.CGRQ.Value.ToString("yyyy/MM/dd") : "")
                            + "</td></tr>");
            }
            sHtml.Append("<tr height=\"30\"><td colspan=\"2\">总计：</td><td style='vnd.ms-excel.numberformat:@' colspan=\"10\">" + totalNum.ToString() + "</td></tr>");
            sHtml.Append("</table>");
            return sHtml.ToString();
        }

        [CheckLogin()]
        public ActionResult Details(ExcelModel eModel)
        {
            var curModel = new T_CGMX();
            if (eModel.SCQYID != null)
            {
                T_SupQY qycp = new T_SupQY();
                qycp.SupID = (int)eModel.SCQYID;
                curModel.T_SupQY1 = qycp;
            }
            if (eModel.CPID != null)
            {
                curModel.CPID = (int)eModel.CPID;
            }

            int pagesize = eModel.PageSize;
            int pagecount = 0;
            int currentPage = eModel.PageNum;
            var DataList = T_CGMXDomain.GetInstance().PageT_CGMX(curModel, eModel.StartDate, eModel.EndDate, currentPage, pagesize, out pagecount, out resultCount);


            ViewData["ParaStr"] = ExportExcelPR(DataList);
            return View("~/Views/T_CGHZ/Details.cshtml", eModel);
        }
        #endregion
    }
    public class ExcelModel
    {
        public int? SCQYID { get; set; }
        public int? CPID { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int PageNum { get; set; }
        public int PageSize { get; set; }

        public int? CKID { get; set; }
        public string CJRID { get; set; }
    }
}