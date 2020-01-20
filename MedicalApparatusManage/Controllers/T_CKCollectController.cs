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
    public class T_CKCollectController : Controller
    {
        public int resultCount = 0; // 总条数 

        [CheckLogin()]
        public ActionResult Index(T_CKCollectModels evalModel)
        {
            try
            {
                evalModel.currentPage = int.Parse(Request["pageNum"].ToString());
            }
            catch { }
            string order = "";
            try
            {
                order = Request["orderField"].ToString();
            }
            catch { }

            if (order.Trim() == "${param.orderField}")
            {
                order = "";
            }
            int pagesize = Convert.ToInt32(evalModel.pageSize);
            int pagecount = Convert.ToInt32(evalModel.pagecount);
            int currentPage = Convert.ToInt32(evalModel.currentPage);
            evalModel.DataModel = evalModel.DataModel ?? new T_CKCollect();
            if (Request["strCKName"] != null)
            {
                string str = Request["strCKName"].ToString();
                if (!String.IsNullOrEmpty(str))
                {
                    evalModel.DataModel.CKID = int.Parse(str);
                }
                ViewData["strCKName"] = str;
            }
            var strCKPerson = "请选择";
            if (Request["strCKPerson"] != null)
            {
                strCKPerson = Request["strCKPerson"].ToString();
                if (!String.IsNullOrEmpty(strCKPerson))
                {
                    evalModel.DataModel.CollectPerson = strCKPerson;
                }
            }
            SysUser UserModel = Session["UserModel"] as SysUser;
            T_Person person = new T_Person();
            person.PsQYID = (int)UserModel.UserCompanyID;
            ViewBag.Persons = new SelectList(T_PersonDomain.GetInstance().GetAllT_Person(person), "PsMZ", "PsMZ");
            ViewData["strCKPerson"] = strCKPerson;

            var lstck = GetAllCK();
            ViewBag.CKList = new SelectList(lstck, "CKID", "CKMC");

            evalModel.DataList = T_CKCollectDomain.GetInstance().PageT_CKCollect(evalModel.DataModel, evalModel.StartTime, evalModel.EndTime, currentPage, pagesize, out pagecount, out resultCount);
            evalModel.resultCount = resultCount;
            return View("~/Views/T_CKCollect/Index.cshtml", evalModel);
        }

        [CheckLogin()]
        public ActionResult Save(System.Int32 id, string tag)
        {
            T_CKCollectModels model = new T_CKCollectModels();
            model.DataModel = new T_CKCollect();
            if (id != 0)
            {
                model.DataModel = T_CKCollectDomain.GetInstance().GetModelById(id);
            }
            model.Tag = tag;
            T_Person person = new T_Person();
            SysUser UserModel = Session["UserModel"] as SysUser;
            person.PsQYID = (int)UserModel.UserCompanyID;
            ViewBag.Persons = new SelectList(T_PersonDomain.GetInstance().GetAllT_Person(person), "PsMZ", "PsMZ");

            var lstck = GetAllCK();
            ViewBag.CKList = new SelectList(lstck, "CKID", "CKMC");

            return View("~/Views/T_CKCollect/Save.cshtml", model);
        }

        [HttpPost]
        [CheckLogin()]
        public void Save(T_CKCollectModels model)
        {
            int result = 0;
            try
            {
                if (model.Tag == "Add")
                {
                    result = T_CKCollectDomain.GetInstance().AddModel(model.DataModel);
                }
                else if (model.Tag == "Edit")
                {
                    result = T_CKCollectDomain.GetInstance().UpdateModel(model.DataModel, model.DataModel.CollectID);
                }

            }
            catch { }
            Response.ContentType = "text/json";
            if (result > 0)
                Response.Write("{\"statusCode\":\"200\", \"message\":\"操作成功\",\"callbackType\":\"closeCurrentReloadTab\",\"forwardUrl\":\"/T_CKCollect/Index\"}");
            else
                Response.Write("{\"statusCode\":\"300\", \"message\":\"操作失败\"}");
        }

        [HttpPost]
        [CheckLogin()]
        public void Delete(System.Int32 id)
        {
            int result = T_CKCollectDomain.GetInstance().DeleteModelById(id);
            Response.ContentType = "text/json";
            if (result > 0)
                Response.Write("{\"statusCode\":\"200\", \"message\":\"操作成功\",\"callbackType\":\"forward\",\"forwardUrl\":\"/T_CKCollect/Index\"}");
            else
                Response.Write("{\"statusCode\":\"300\", \"message\":\"操作失败\"}");
        }
        public List<T_CK> GetAllCK()
        {
            var lstResult = new List<T_CK>();
            try
            {
                Expression<Func<T_CK, bool>> where = PredicateBuilder.True<T_CK>();
                where = where.And(p => p.FLAG == "是");
                lstResult = T_CKDomain.GetInstance().GetAllModels<int>(where);
            }
            catch (Exception ex)
            {
            }
            return lstResult;
        }


        #region excel导出、打印

        [CheckLogin()]
        public void ExportExcel(ExcelModel eModel)
        {
            var curModel = new T_CKCollect();
            if (eModel.CKID != null)
            {
                curModel.CKID = (int)eModel.CKID;
            }
            if (!string.IsNullOrEmpty(eModel.CJRID))
            {
                curModel.CollectPerson = eModel.CJRID;
            }

            int pagesize = eModel.PageSize;
            int pagecount = 0;
            int currentPage = eModel.PageNum;
            var DataList = T_CKCollectDomain.GetInstance().PageT_CKCollect(curModel, eModel.StartDate, eModel.EndDate, currentPage, pagesize, out pagecount, out resultCount);
            var str = ExportExcelPR(DataList);
            //调用输出Excel表的方法
            ExportToExcel("application/vnd.ms-excel", "仓库采集数据.xls", str);
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
        public string ExportExcelPR(List<T_CKCollect> listMX)
        {
            if (listMX == null)
                listMX = new List<T_CKCollect>();

            //命名导出表格的StringBuilder变量
            StringBuilder sHtml = new StringBuilder(string.Empty);

            sHtml.Append("<table border=\"1\" width=\"100%\"  style='border-collapse:collapse;border:1px solid black;'>");
            sHtml.Append("<tr height=\"30\" align=\"center\"><th>仓库名称</th><th>仓库类型</th><th>仓库当前温度</th><th>仓库当前湿度</th><th>采集日期</th><th>采集人</th><th>备注</th></tr>");

            for (int i = 0; i < listMX.Count; i++)
            {
                var item = listMX[i];
                sHtml.Append("<tr height=\"30\" align=\"center\"><td>" + item.T_CK.CKMC
                            + "</td><td>" + item.T_CK.CKLX
                            + "</td><td>" + item.CurWD
                            + "</td><td>" + item.CurSD
                            + "</td><td>" + (item.CollectDate != null && item.CollectDate.HasValue ? item.CollectDate.Value.ToString("yyyy/MM/dd") : "")
                            + "</td><td>" + item.CollectPerson
                            + "</td><td>" + item.Remark
                            + "</td></tr>");
            }
            sHtml.Append("</table>");
            return sHtml.ToString();
        }

        [CheckLogin()]
        public ActionResult Details(ExcelModel eModel)
        {
            var curModel = new T_CKCollect();
            if (eModel.CKID != null)
            {
                curModel.CKID = (int)eModel.CKID;
            }
            if (!string.IsNullOrEmpty(eModel.CJRID))
            {
                curModel.CollectPerson = eModel.CJRID;
            }

            int pagesize = eModel.PageSize;
            int pagecount = 0;
            int currentPage = eModel.PageNum;
            var DataList = T_CKCollectDomain.GetInstance().PageT_CKCollect(curModel, eModel.StartDate, eModel.EndDate, currentPage, pagesize, out pagecount, out resultCount);

            ViewData["ParaStr"] = ExportExcelPR(DataList);
            return View("~/Views/T_CKCollect/Details.cshtml", eModel);
        }
        #endregion
    }
}