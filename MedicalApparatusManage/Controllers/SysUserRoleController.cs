using MedicalApparatusManage.Domain;
using MedicalApparatusManage.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MedicalApparatusManage.Controllers
{
    public class SysUserRoleController : Controller
    {
        public int resultCount = 0; // 总条数 

        [CheckLogin()]
        public ActionResult Index(SysUserRoleModels evalModel)
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
            evalModel.DataModel = evalModel.DataModel ?? new SysUserRole();
            evalModel.DataList = SysUserRoleDomain.GetInstance().PageSysUserRole(evalModel.DataModel, evalModel.StartTime, evalModel.EndTime, currentPage, pagesize, out pagecount, out resultCount);
            evalModel.resultCount = resultCount;
            return View("~/Views/SysUserRole/Index.cshtml", evalModel);
        }

        [CheckLogin()]
        public ActionResult Save(System.Int32 id, string tag)
        {
            SysUserRoleModels model = new SysUserRoleModels();
            model.DataModel = new SysUserRole();
            //加载用户列表
            SysUserModels usermode = new SysUserModels();
            usermode.DataModel = usermode.DataModel ?? new SysUser();
            usermode.DataList = SysUserDomain.GetInstance().GetAllT_SysUser(usermode.DataModel);
            ViewData["SysUser"] = new SelectList(usermode.DataList, "UserID", "UserName", "请选择");
            //加载权限列表
            SysRoleModels rolemode = new SysRoleModels();
            rolemode.DataModel = rolemode.DataModel ?? new SysRole();
            rolemode.DataList = SysRoleDomain.GetInstance().GetAllT_SysUser(rolemode.DataModel);
            ViewData["SysRole"] = new SelectList(rolemode.DataList, "RoleId", "RoleName", "请选择");
            if (id != 0)
            {
                model.DataModel = SysUserRoleDomain.GetInstance().GetModelById(id);
            }
            model.Tag = tag;
            return View("~/Views/SysUserRole/Save.cshtml", model);
        }

        [HttpPost]
        [CheckLogin()]
        public void Save(SysUserRoleModels model)
        {
            int result = 0;
            try
            {
                if (model.Tag == "Add")
                {
                    result = SysUserRoleDomain.GetInstance().AddModel(model.DataModel);
                }
                else if (model.Tag == "Edit")
                {
                    result = SysUserRoleDomain.GetInstance().UpdateModel(model.DataModel, model.DataModel.ActionId);
                }

            }
            catch { }
            Response.ContentType = "text/json";
            if (result > 0)
                Response.Write("{\"statusCode\":\"200\", \"message\":\"操作成功\",\"callbackType\":\"closeCurrentReloadTab\",\"forwardUrl\":\"/SysUserRole/Index\"}");
            else
                Response.Write("{\"statusCode\":\"300\", \"message\":\"操作失败\"}");
        }

        [HttpPost]
        [CheckLogin()]
        public void Delete(System.Int32 id)
        {
            int result = SysUserRoleDomain.GetInstance().DeleteModelById(id);
            Response.ContentType = "text/json";
            if (result > 0)
                Response.Write("{\"statusCode\":\"200\", \"message\":\"操作成功\",\"callbackType\":\"forward\",\"forwardUrl\":\"/SysUserRole/Index\"}");
            else
                Response.Write("{\"statusCode\":\"300\", \"message\":\"操作失败\"}");
        }
    }
}
