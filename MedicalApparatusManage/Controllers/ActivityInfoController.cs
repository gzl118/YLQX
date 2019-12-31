using MedicalApparatusManage.Domain;
using MedicalApparatusManage.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MedicalApparatusManage.Controllers
{
    public class ActivityInfoController : Controller
    {
        public int resultCount = 0; // 总条数 

        [CheckLogin()]
        public ActionResult Index(ActivityInfoModels evalModel)
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
            evalModel.DataModel = evalModel.DataModel ?? new ActivityInfo();
            evalModel.DataList = ActivityInfoDomain.GetInstance().PageActivityInfo(evalModel.DataModel, evalModel.StartTime, evalModel.EndTime, currentPage, pagesize, out pagecount, out resultCount);
            evalModel.resultCount = resultCount;
            return View("~/Views/ActivityInfo/Index.cshtml", evalModel);
        }

        [CheckLogin()]
        public ActionResult Save(System.Int32 id, string tag)
        {
            ActivityInfoModels model = new ActivityInfoModels();
            model.DataModel = new ActivityInfo();
            if (id != 0)
            {
                model.DataModel = ActivityInfoDomain.GetInstance().GetModelById(id);
            }
            model.Tag = tag;
            return View("~/Views/ActivityInfo/Save.cshtml", model);
        }

        [HttpPost]
        [CheckLogin()]
        public void Save(ActivityInfoModels model)
        {
            int result = 0;
            try
            {
                model.DataModel.CreateTime = DateTime.Now;
                SysUser UserModel = Session["UserModel"] as SysUser;
                if (UserModel != null)
                {
                    model.DataModel.CreatePersom = UserModel.UserAccount;
                }
                if (model.Tag == "Add")
                {
                    result = ActivityInfoDomain.GetInstance().AddModel(model.DataModel);
                }
                else if (model.Tag == "Edit")
                {
                    result = ActivityInfoDomain.GetInstance().UpdateModel(model.DataModel, model.DataModel.ID);
                }

            }
            catch { }
            Response.ContentType = "text/json";
            if (result > 0)
                Response.Write("{\"statusCode\":\"200\", \"message\":\"操作成功\",\"callbackType\":\"closeCurrentReloadTab\",\"forwardUrl\":\"/ActivityInfo/Index\"}");
            else
                Response.Write("{\"statusCode\":\"300\", \"message\":\"操作失败\"}");
        }

        [HttpPost]
        [CheckLogin()]
        public void Delete(System.Int32 id)
        {
            int result = ActivityInfoDomain.GetInstance().DeleteModelById(id);
            Response.ContentType = "text/json";
            if (result > 0)
                Response.Write("{\"statusCode\":\"200\", \"message\":\"操作成功\",\"callbackType\":\"forward\",\"forwardUrl\":\"/ActivityInfo/Index\"}");
            else
                Response.Write("{\"statusCode\":\"300\", \"message\":\"操作失败\"}");
        }
    }
}
