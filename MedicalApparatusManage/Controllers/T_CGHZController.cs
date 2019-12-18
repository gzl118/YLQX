using MedicalApparatusManage.Domain;
using MedicalApparatusManage.Models;
using System;
using System.Collections.Generic;
using System.Linq;
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
            int pagesize = Convert.ToInt32(evalModel.pageSize);
            int pagecount = Convert.ToInt32(evalModel.pagecount);
            int currentPage = Convert.ToInt32(evalModel.currentPage);
            evalModel.DataModel = evalModel.DataModel ?? new T_CGMX();
            evalModel.DataList = T_CGMXDomain.GetInstance().PageT_CGMX(evalModel.DataModel, evalModel.StartTime, evalModel.EndTime, currentPage, pagesize, out pagecount, out resultCount);
            evalModel.resultCount = resultCount;
            return View("~/Views/T_CGHZ/Index.cshtml", evalModel);
        }
    }
}