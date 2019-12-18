using MedicalApparatusManage.Domain;
using MedicalApparatusManage.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MedicalApparatusManage.Controllers
{
    public class SysFavoriteController : Controller
    {
        public int resultCount = 0; // 总条数 

        [CheckLogin()]
        public ActionResult Index(SysFavoriteModels evalModel)
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
            evalModel.DataModel = evalModel.DataModel ?? new SysFavorite();
            evalModel.DataList = SysFavoriteDomain.GetInstance().PageSysFavorite(evalModel.DataModel, evalModel.StartTime, evalModel.EndTime, currentPage, pagesize, out pagecount, out resultCount);
            evalModel.resultCount = resultCount;
            return View("~/Views/SysFavorite/Index.cshtml", evalModel);
        }

        [CheckLogin()]
        public ActionResult Save(System.Int32 id, string tag)
        {
            SysFavoriteModels model = new SysFavoriteModels();
            model.DataModel = new SysFavorite();
            //加载用户列表
            SysUserModels whsQymode = new SysUserModels();

            whsQymode.DataModel = whsQymode.DataModel ?? new SysUser();

            whsQymode.DataList = SysUserDomain.GetInstance().GetAllT_SysUser(whsQymode.DataModel);

            ViewData["SysUser"] = new SelectList(whsQymode.DataList, "UserID", "UserName", "请选择");
            if (id != 0)
            {
                model.DataModel = SysFavoriteDomain.GetInstance().GetModelById(id);
            }
            model.Tag = tag;
            return View("~/Views/SysFavorite/Save.cshtml", model);
        }

        [HttpPost]
        [CheckLogin()]
        public void Save(SysFavoriteModels model)
        {
            int result = 0;
            try
            {
                if (model.Tag == "Add")
                {
                    result = SysFavoriteDomain.GetInstance().AddModel(model.DataModel);
                }
                else if (model.Tag == "Edit")
                {
                    result = SysFavoriteDomain.GetInstance().UpdateModel(model.DataModel, model.DataModel.FavoriteID);
                }

            }
            catch { }
            Response.ContentType = "text/json";
            if (result > 0)
                Response.Write("{\"statusCode\":\"200\", \"message\":\"操作成功\",\"callbackType\":\"closeCurrentReloadTab\",\"forwardUrl\":\"/WebMain/Main\"}");
            else
                Response.Write("{\"statusCode\":\"300\", \"message\":\"操作失败\"}");
        }

        [HttpPost]
        [CheckLogin()]
        public void Delete(System.Int32 id)
        {
            int result = SysFavoriteDomain.GetInstance().DeleteModelById(id);
            Response.ContentType = "text/json";
            if (result > 0)
                Response.Write("{\"statusCode\":\"200\", \"message\":\"操作成功\",\"callbackType\":\"forward\",\"forwardUrl\":\"/WebMain/Main\"}");
            else
                Response.Write("{\"statusCode\":\"300\", \"message\":\"操作失败\"}");
        }
    }
}
