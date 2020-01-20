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
    public class T_XSHZController : Controller
    {
        public int resultCount = 0; // 总条数 
        // GET: T_XSHZ
        public ActionResult Index(T_XSMXModels evalModel)
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
            evalModel.DataModel = evalModel.DataModel ?? new T_XSMX();
            evalModel.DataModel.T_YLCP = evalModel.DataModel.T_YLCP ?? new T_YLCP();

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
                    evalModel.DataModel.T_YLCP.T_SupQY1 = qycp;
                }
            }
            ViewData["QYList"] = qyid;

            Expression<Func<T_YLCP, bool>> where = PredicateBuilder.True<T_YLCP>();
            var lst = T_YLCPDomain.GetInstance().GetAllModels<int>(where);
            ViewData["XSHZ_YLCP"] = new SelectList(lst, "CPID", "CPMC");
            var cpid = "";
            if (Request["XSHZ_CPID"] != null)
            {
                cpid = Request["XSHZ_CPID"].ToString();
                if (!String.IsNullOrEmpty(cpid))
                {
                    evalModel.DataModel.CPID = int.Parse(cpid);
                }
            }
            ViewData["XSHZ_CPID"] = cpid;

            int pagesize = Convert.ToInt32(evalModel.pageSize);
            int pagecount = Convert.ToInt32(evalModel.pagecount);
            int currentPage = Convert.ToInt32(evalModel.currentPage);
            evalModel.DataModel = evalModel.DataModel ?? new T_XSMX();
            evalModel.DataList = T_XSMXDomain.GetInstance().PageT_XSMX(evalModel.DataModel, evalModel.StartTime, evalModel.EndTime, currentPage, pagesize, out pagecount, out resultCount);
            evalModel.resultCount = resultCount;

            var totalNum = 0;
            if (evalModel.DataList != null && evalModel.DataList.Count > 0)
            {
                evalModel.DataList.ForEach(p =>
                {
                    totalNum += (p.CPSL ?? 0);
                });
            }
            ViewBag.TotalNum = totalNum;
            return View("~/Views/T_XSHZ/Index.cshtml", evalModel);
        }
        #region excel导出、打印

        [CheckLogin()]
        public void ExportExcel(ExcelModel eModel)
        {
            var curModel = new T_XSMX();
            if (eModel.SCQYID != null)
            {
                T_SupQY qycp = new T_SupQY();
                qycp.SupID = (int)eModel.SCQYID;
                curModel.T_YLCP.T_SupQY1 = qycp;
            }
            if (eModel.CPID != null)
            {
                curModel.CPID = (int)eModel.CPID;
            }

            int pagesize = eModel.PageSize;
            int pagecount = 0;
            int currentPage = eModel.PageNum;
            var DataList = T_XSMXDomain.GetInstance().PageT_XSMX(curModel, eModel.StartDate, eModel.EndDate, currentPage, pagesize, out pagecount, out resultCount);
            var str = ExportExcelPR(DataList);
            //调用输出Excel表的方法
            ExportToExcel("application/vnd.ms-excel", "销售汇总.xls", str);
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
        public string ExportExcelPR(List<T_XSMX> listMX)
        {
            if (listMX == null)
                listMX = new List<T_XSMX>();
            var totalNum = 0;
            if (listMX != null && listMX.Count > 0)
            {
                listMX.ForEach(p =>
                {
                    totalNum += p.CPSL == null ? 0 : (int)p.CPSL;
                });
            }

            //命名导出表格的StringBuilder变量
            StringBuilder sHtml = new StringBuilder(string.Empty);

            sHtml.Append("<table border=\"1\" width=\"100%\"  style='border-collapse:collapse;border:1px solid black;'>");
            sHtml.Append("<tr height=\"30\" align=\"center\"><th>销售单号</th><th>产品名称</th><th>产品数量</th><th>产品单位</th><th>销售单价(元)</th><th>购货企业</th><th>生产企业</th><th>产品规格</th><th>产品型号</th><th>许可证号</th><th>注册证号</th><th>销售日期</th></tr>");

            for (int i = 0; i < listMX.Count; i++)
            {
                var item = listMX[i];
                sHtml.Append("<tr height=\"30\" align=\"center\"><td>" + (item.T_XSD != null && !string.IsNullOrEmpty(item.T_XSD.XSDH) ? item.T_XSD.XSDH : "")
                            + "</td><td>" + (item.T_YLCP != null && !string.IsNullOrEmpty(item.T_YLCP.CPMC) ? item.T_YLCP.CPMC : "")
                            + "</td><td>" + item.CPSL
                            + "</td><td>" + (item.T_YLCP != null && !string.IsNullOrEmpty(item.T_YLCP.CPDW) ? item.T_YLCP.CPDW : "")
                            + "</td><td>" + item.XSJG
                            + "</td><td>" + (item.T_XSD != null && item.T_XSD.T_CusQY != null && !string.IsNullOrEmpty(item.T_XSD.T_CusQY.CusMC) ? item.T_XSD.T_CusQY.CusMC : "")
                            + "</td><td>" + (item.T_YLCP != null && item.T_YLCP.T_SupQY1 != null && !string.IsNullOrEmpty(item.T_YLCP.T_SupQY1.SupMC) ? item.T_YLCP.T_SupQY1.SupMC : "")
                            + "</td><td>" + (item.T_YLCP != null && !string.IsNullOrEmpty(item.T_YLCP.CPGG) ? item.T_YLCP.CPGG : "")
                            + "</td><td>" + (item.T_YLCP != null && !string.IsNullOrEmpty(item.T_YLCP.CPXH) ? item.T_YLCP.CPXH : "")
                            + "</td><td>" + (item.T_YLCP != null && item.T_YLCP.T_SupQY1 != null && !string.IsNullOrEmpty(item.T_YLCP.T_SupQY1.SupXKZBH) ? item.T_YLCP.T_SupQY1.SupXKZBH : "")
                            + "</td><td>" + (item.T_YLCP != null && !string.IsNullOrEmpty(item.T_YLCP.CPZCZ) ? item.T_YLCP.CPZCZ : "")
                            + "</td><td>" + (item.T_XSD != null && item.T_XSD.XSRQ.HasValue ? item.T_XSD.XSRQ.Value.ToString("yyyy/MM/dd") : "")
                            + "</td></tr>");
            }
            sHtml.Append("<tr height=\"30\"><td colspan=\"2\">总计：</td><td style='vnd.ms-excel.numberformat:@' colspan=\"10\">" + totalNum.ToString() + "</td></tr>");
            sHtml.Append("</table>");
            return sHtml.ToString();
        }

        [CheckLogin()]
        public ActionResult Details(ExcelModel eModel)
        {
            var curModel = new T_XSMX();
            if (eModel.SCQYID != null)
            {
                T_SupQY qycp = new T_SupQY();
                qycp.SupID = (int)eModel.SCQYID;
                curModel.T_YLCP.T_SupQY1 = qycp;
            }
            if (eModel.CPID != null)
            {
                curModel.CPID = (int)eModel.CPID;
            }

            int pagesize = eModel.PageSize;
            int pagecount = 0;
            int currentPage = eModel.PageNum;
            var DataList = T_XSMXDomain.GetInstance().PageT_XSMX(curModel, eModel.StartDate, eModel.EndDate, currentPage, pagesize, out pagecount, out resultCount);


            ViewData["ParaStr"] = ExportExcelPR(DataList);
            return View("~/Views/T_XSHZ/Details.cshtml", eModel);
        }
        #endregion
    }
}