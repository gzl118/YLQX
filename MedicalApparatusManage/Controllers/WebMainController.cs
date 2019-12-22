using MedicalApparatusManage.Models;
using System;
using System.Web.Mvc;
using MedicalApparatusManage.Domain;

namespace MedicalApparatusManage.Controllers
{
    public class WebMainController : Controller
    {
        // GET: WebMain
        public ActionResult Index(WebMainModels model)
        {
            if (Session["UserModel"] != null)
            {
                try
                {
                    SysUser UserModel = Session["UserModel"] as SysUser;
                    model.ListResource = SysResourceDomain.GetInstance().GetPagesByUserId(UserModel.UserId);
                    model.UserName = UserModel.UserName;
                    var RoleCode = GetRoleCode();
                    model.RoleType = RoleCode == "1" ? "超级管理员" : (RoleCode == "2" ? "部门领导" : "普通员工");
                    return View(model);
                }
                catch (Exception)
                {
                    return RedirectToAction("Login", "WebMain");
                }
            }
            else
                return RedirectToAction("Login", "WebMain");
        }

        public ActionResult Main(WebMainModels model)
        {
            model.ListSysFavorite = SysFavoriteDomain.GetInstance().GetFavoriteList(1);
            return View(model);
        }

        public ActionResult Login()
        {
            return View();
        }

        public ActionResult LoginNew(string userAccount, string password)
        {
            SysUser user = null;
            string result = SysUserDomain.GetInstance().Login(userAccount, password, out user);
            if (user != null)
            {
                Session["UserModel"] = user;
                Session["RoleId"] = user.RoleId;
                if (user.RoleId != null)
                {
                    var roleModel = SysRoleDomain.GetInstance().GetModelById(user.RoleId);
                    if (roleModel != null)
                    {
                        Session["RoleCode"] = roleModel.RoleCode;  //1超级管理员，2部门领导，3普通职员
                    }
                }
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Logout()
        {
            Response.Cookies.Clear();
            Session["UserModel"] = null;
            Session["RoleCode"] = null;
            Session["RoleId"] = null;
            return RedirectToAction("Login", "WebMain");
        }

        public void Logoff()
        {
            Response.Write("<html><script type='text/JavaScript'> alertMsg.warn('登录超时！', {okCall: function () {  top.location.href = '/WebMain/Logout';}});</script></html>");
        }

        /// <summary>
        /// 主页修改密码
        /// </summary>
        /// <returns></returns>
        [CheckLogin()]
        public ActionResult UpdatePw()
        {
            WebMainModels main = new WebMainModels();
            return View(main);
        }

        [HttpPost]
        [CheckLogin()]
        public void UpdatePw(WebMainModels main)
        {
            SysUser result = null;
            if (Session["UserModel"] != null)
            {
                result = SysUserDomain.GetInstance().UpdatePwd(Session["UserModel"] as SysUser, main.passWord);
            }
            Response.ContentType = "text/json";
            if (result != null)
            {
                Session["UserModel"] = result;
                Response.Write("{\"statusCode\":\"200\", \"message\":\"操作成功\",\"callbackType\":\"closeCurrent\",\"forwardUrl\":\"/WebMain/Main\"}");
            }
            else
                Response.Write("{\"statusCode\":\"300\", \"message\":\"操作失败\"}");
        }

        [CheckLogin()]
        public void getPassword(string id)
        {
            SysUser UserModel = Session["UserModel"] as SysUser;
            Response.ContentType = "text/html";
            if (UserModel != null && UserModel.UserPassword == id)
            {
                Response.Write("success");
            }
            else
            {
                Response.Write("error");
            }
        }
        private string GetRoleCode()
        {
            return Session["RoleCode"] == null ? "" : Session["RoleCode"].ToString();
        }
    }
}