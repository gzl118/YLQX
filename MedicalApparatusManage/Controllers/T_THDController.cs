using MedicalApparatusManage.Domain;
using MedicalApparatusManage.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MedicalApparatusManage.Controllers
{
    public class T_THDController : Controller
    {
        public int resultCount = 0; // 总条数 

        [CheckLogin()]
        public ActionResult Index(T_THDModels evalModel, string id)
        {
            try
            {
                id = "1";
                ViewData["XSType"] = id;
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
            evalModel.DataModel = evalModel.DataModel ?? new T_THD();

            if (Request["strTHMC"] != null)
            {
                string str = Request["strTHMC"].ToString();
                if(!String.IsNullOrEmpty(str))
                {
                    evalModel.DataModel.THMC = str;
                }
            }

            if (Request["strTHDW"] != null)
            {
                string str = Request["strTHDW"].ToString();
                if (!String.IsNullOrEmpty(str))
                {
                    evalModel.DataModel.THKHMC = str;
                }
            }

            evalModel.DataList = T_THDDomain.GetInstance().PageT_THD(evalModel.DataModel, evalModel.StartTime, evalModel.EndTime, currentPage, pagesize, out pagecount, out resultCount).Where(p => p.FLAG == Convert.ToInt32(id)).ToList();
            evalModel.resultCount = resultCount;
            return View("~/Views/T_THD/Index.cshtml", evalModel);
        }

        [CheckLogin()]
        public ActionResult Save(System.Int32 id, string tag)
        {

            //加载购买商商企业列表
            T_CusQYModels cusQymode = new T_CusQYModels();

            cusQymode.DataModel = cusQymode.DataModel ?? new T_CusQY();

            cusQymode.DataList = T_CusQYDomain.GetInstance().GetAllT_CusQY(cusQymode.DataModel).Where(p => p.CusStatus == Convert.ToInt32("1")).ToList();

            ViewData["CusQY"] = new SelectList(cusQymode.DataList, "CusID", "CusMC");

            //加载购买商商企业列表
            T_CGDModels cudmode = new T_CGDModels();

            cudmode.DataModel = cudmode.DataModel ?? new T_CGD();

            cudmode.DataList = T_CGDDomain.GetInstance().GetAllT_CGD(cudmode.DataModel).Where(p => p.ISSH == Convert.ToInt32("1")).ToList();

            ViewData["CGD"] = new SelectList(cudmode.DataList, "CGID", "CGDMC");


            //加载购买商商企业列表
            T_XSDModels xsdmode = new T_XSDModels();

            xsdmode.DataModel = xsdmode.DataModel ?? new T_XSD();

            xsdmode.DataList = T_XSDDomain.GetInstance().GetAllT_XSD(xsdmode.DataModel).Where(p => p.XSFLAG == Convert.ToInt32("1")).ToList();

            ViewData["XSD"] = new SelectList(xsdmode.DataList, "XSID", "XSMC");

            T_THDModels model = new T_THDModels();
            model.DataModel = new T_THD();
            if (id != 0)
            {
                model.DataModel = T_THDDomain.GetInstance().GetModelById(id);
            }
            model.Tag = tag;
            return View("~/Views/T_THD/Save.cshtml", model);
        }

        [HttpPost]
        [CheckLogin()]
        public void Save(T_THDModels model)
        {
            int result = 0;
            try
            {
                if (model.Tag == "Add")
                {
                    model.DataModel.FLAG = 1;
                    result = T_THDDomain.GetInstance().AddModel(model.DataModel);
                }
                else if (model.Tag == "Edit")
                {
                    model.DataModel.FLAG = 1;
                    result = T_THDDomain.GetInstance().UpdateModel(model.DataModel, model.DataModel.THID);
                }

            }
            catch { }
            Response.ContentType = "text/json";
            if (result > 0)
                Response.Write("{\"statusCode\":\"200\", \"message\":\"操作成功\",\"callbackType\":\"closeCurrentReloadTab\",\"forwardUrl\":\"/T_THD/Index\"}");
            else
                Response.Write("{\"statusCode\":\"300\", \"message\":\"操作失败\"}");
        }

        [HttpPost]
        [CheckLogin()]
        public void XSSave(T_THDModels model)
        {
            int result = 0;
            try
            {
                if (model.Tag == "Add")
                {
                    model.DataModel.FLAG = 2;
                    result = T_THDDomain.GetInstance().AddModel(model.DataModel);
                }
                else if (model.Tag == "Edit")
                {
                    model.DataModel.FLAG = 2;
                    result = T_THDDomain.GetInstance().UpdateModel(model.DataModel, model.DataModel.THID);
                }

            }
            catch { }
            Response.ContentType = "text/json";
            if (result > 0)
                Response.Write("{\"statusCode\":\"200\", \"message\":\"操作成功\",\"callbackType\":\"closeCurrentReloadTab\",\"forwardUrl\":\"/T_THD/Index\"}");
            else
                Response.Write("{\"statusCode\":\"300\", \"message\":\"操作失败\"}");
        }

        [HttpPost]
        [CheckLogin()]
        public void Delete(System.Int32 id)
        {
            int result = T_THDDomain.GetInstance().DeleteModelById(id);
            Response.ContentType = "text/json";
            if (result > 0)
                Response.Write("{\"statusCode\":\"200\", \"message\":\"操作成功\",\"callbackType\":\"forward\",\"forwardUrl\":\"/T_THD/Index\"}");
            else
                Response.Write("{\"statusCode\":\"300\", \"message\":\"操作失败\"}");
        }
    }
}
