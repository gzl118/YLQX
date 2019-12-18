using MedicalApparatusManage.Domain;
using MedicalApparatusManage.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MedicalApparatusManage.Controllers
{
    public class T_SHFWController : Controller
    {
        public int resultCount = 0; // 总条数 

        [CheckLogin()]
        public ActionResult Index(T_SHFWModels evalModel)
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
            evalModel.DataModel = evalModel.DataModel ?? new T_SHFW();

            if (Request["strFWMC"] != null)
            {
                string str = Request["strFWMC"].ToString();
                if(!String.IsNullOrEmpty(str))
                {
                    evalModel.DataModel.FWMC = str;
                }
            }
            evalModel.DataList = T_SHFWDomain.GetInstance().PageT_SHFW(evalModel.DataModel, evalModel.StartTime, evalModel.EndTime, currentPage, pagesize, out pagecount, out resultCount);
            evalModel.resultCount = resultCount;
            return View("~/Views/T_SHFW/Index.cshtml", evalModel);
        }

        [CheckLogin()]
        public ActionResult Save(System.Int32 id, string tag)
        {

            //加载企业列表
            T_CusQYModels cusQymode = new T_CusQYModels();

            cusQymode.DataModel = cusQymode.DataModel ?? new T_CusQY();

            cusQymode.DataList = T_CusQYDomain.GetInstance().GetAllT_CusQY(cusQymode.DataModel).Where(p => p.CusStatus == Convert.ToInt32("1")).ToList();

            ViewData["CusQY"] = new SelectList(cusQymode.DataList, "CusID", "CusMC");

            T_SHFWModels model = new T_SHFWModels();
            model.DataModel = new T_SHFW();
            if (id != 0)
            {
                model.DataModel = T_SHFWDomain.GetInstance().GetModelById(id);
            }
            model.Tag = tag;
            return View("~/Views/T_SHFW/Save.cshtml", model);
        }

        [HttpPost]
        [CheckLogin()]
        public void Save(T_SHFWModels model)
        {
            int result = 0;
            try
            {
                if (model.Tag == "Add")
                {
                    result = T_SHFWDomain.GetInstance().AddModel(model.DataModel);
                }
                else if (model.Tag == "Edit")
                {
                    result = T_SHFWDomain.GetInstance().UpdateModel(model.DataModel, model.DataModel.SHID);
                }

            }
            catch { }
            Response.ContentType = "text/json";
            if (result > 0)
                Response.Write("{\"statusCode\":\"200\", \"message\":\"操作成功\",\"callbackType\":\"closeCurrentReloadTab\",\"forwardUrl\":\"/T_SHFW/Index\"}");
            else
                Response.Write("{\"statusCode\":\"300\", \"message\":\"操作失败\"}");
        }

        [HttpPost]
        [CheckLogin()]
        public void Delete(System.Int32 id)
        {
            int result = T_SHFWDomain.GetInstance().DeleteModelById(id);
            Response.ContentType = "text/json";
            if (result > 0)
                Response.Write("{\"statusCode\":\"200\", \"message\":\"操作成功\",\"callbackType\":\"forward\",\"forwardUrl\":\"/T_SHFW/Index\"}");
            else
                Response.Write("{\"statusCode\":\"300\", \"message\":\"操作失败\"}");
        }
    }
}
