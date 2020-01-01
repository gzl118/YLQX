using MedicalApparatusManage.Common;
using MedicalApparatusManage.Domain;
using MedicalApparatusManage.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;

namespace MedicalApparatusManage.Controllers
{
    public class SysUserController : Controller
    {
        public int resultCount = 0; // 总条数 

        [CheckLogin()]
        public ActionResult Index(SysUserModels evalModel)
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
            evalModel.DataModel = evalModel.DataModel ?? new SysUser();
            evalModel.DataList = SysUserDomain.GetInstance().PageSysUser(evalModel.DataModel, evalModel.StartTime, evalModel.EndTime, currentPage, pagesize, out pagecount, out resultCount);
            evalModel.resultCount = resultCount;
            return View("~/Views/SysUser/Index.cshtml", evalModel);
        }

        [CheckLogin()]
        public ActionResult Save(System.Int32 id, string tag)
        {
            SysUserModels model = new SysUserModels();
            model.DataModel = new SysUser();
            //加载性别类型
            List<SelectListItem> list = new List<SelectListItem>();
            list.Add(new SelectListItem { Text = "启用", Value = "1" });
            list.Add(new SelectListItem { Text = "禁用", Value = "0" });
            ViewData["ZT"] = new SelectList(list, "Value", "Text", "请选择");
            if (id != 0)
            {
                model.DataModel = SysUserDomain.GetInstance().GetModelById(id);
            }
            model.Tag = tag;
            //加载权限列表
            SysRoleModels rolemode = new SysRoleModels();
            rolemode.DataModel = rolemode.DataModel ?? new SysRole();
            rolemode.DataList = SysRoleDomain.GetInstance().GetAllT_SysUser(rolemode.DataModel);
            ViewData["SysRole"] = new SelectList(rolemode.DataList, "RoleId", "RoleName", "请选择");

            return View("~/Views/SysUser/Save.cshtml", model);
        }

        [HttpPost]
        [CheckLogin()]
        public void Save(SysUserModels model)
        {
            int result = 0;
            try
            {
                if (model.DataModel.UserCompanyID == null)
                {
                    Expression<Func<T_WhsQY, bool>> where = PredicateBuilder.True<T_WhsQY>();
                    var list = T_WhsQYDomain.GetInstance().GetAllModels<System.Int32>(where);
                    if (list != null && list.Count > 0)
                    {
                        model.DataModel.UserCompanyID = list[0].WhsID;
                    }
                    else
                    {
                        Response.Write("{\"statusCode\":\"300\", \"message\":\"请先添加本企业信息\"}");
                    }
                }
                if (model.Tag == "Add")
                {
                    model.DataModel.UserCreateDate = DateTime.Now;
                    result = SysUserDomain.GetInstance().AddModel(model.DataModel);
                }
                else if (model.Tag == "Edit")
                {
                    model.DataModel.UserCreateDate = DateTime.Now;
                    result = SysUserDomain.GetInstance().UpdateModel(model.DataModel, model.DataModel.UserId);
                }

            }
            catch { }
            Response.ContentType = "text/json";
            if (result > 0)
                Response.Write("{\"statusCode\":\"200\", \"message\":\"操作成功\",\"callbackType\":\"closeCurrentReloadTab\",\"forwardUrl\":\"/SysUser/Index\"}");
            else
                Response.Write("{\"statusCode\":\"300\", \"message\":\"操作失败\"}");
        }

        [HttpPost]
        [CheckLogin()]
        public void Delete(System.Int32 id)
        {
            int result = SysUserDomain.GetInstance().DeleteModelById(id);
            Response.ContentType = "text/json";
            if (result > 0)
                Response.Write("{\"statusCode\":\"200\", \"message\":\"操作成功\",\"callbackType\":\"forward\",\"forwardUrl\":\"/SysUser/Index\"}");
            else
                Response.Write("{\"statusCode\":\"300\", \"message\":\"操作失败\"}");
        }
    }
}
