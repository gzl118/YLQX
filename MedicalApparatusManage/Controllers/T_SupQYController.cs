using MedicalApparatusManage.Domain;
using MedicalApparatusManage.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;

namespace MedicalApparatusManage.Controllers
{
    public class T_SupQYController : Controller
    {
        public int resultCount = 0; // 总条数 
        public SysUser loginUser = null;

        [CheckLogin()]
        public ActionResult Index(T_SupQYModels evalModel)
        {
            loginUser = Session["UserModel"] as SysUser;
            if (loginUser != null)
            {
                try
                {
                    SysUser UserModel = Session["UserModel"] as SysUser;
                    ViewData["shUserId"] = UserModel.UserId;
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
                evalModel.DataModel = evalModel.DataModel ?? new T_SupQY();

                if (Request["strSupMc"] != null)
                {
                    string str = Request["strSupMc"].ToString();
                    if (!String.IsNullOrEmpty(str))
                    {
                        evalModel.DataModel.SupMC = str;
                    }
                }
                string strQYLX = "--请选择--";
                if (Request["strQYLX"] != null)
                {
                    strQYLX = Request["strQYLX"].ToString();
                    if (!String.IsNullOrEmpty(strQYLX))
                    {
                        if (strQYLX != "--请选择--")
                        {
                            evalModel.DataModel.SupDWXZ = strQYLX;
                        }

                    }
                }
                string strSHZT = "--请选择--";
                if (Request["strSHZT"] != null)
                {
                    strSHZT = Request["strSHZT"].ToString();
                    if (!String.IsNullOrEmpty(strSHZT))
                    {
                        if (strSHZT == "未审批")
                        {
                            evalModel.DataModel.SupStatus = 0;
                        }
                        else if (strSHZT == "已审批")
                        {
                            evalModel.DataModel.SupStatus = 1;
                        }
                        else if (strSHZT == "审批未通过")
                        {
                            evalModel.DataModel.SupStatus = 2;
                        }
                    }
                }
                evalModel.DataModel.WhsID = loginUser.UserCompanyID ?? 0;

                evalModel.DataList = T_SupQYDomain.GetInstance().PageT_SupQY(evalModel.DataModel, evalModel.StartTime, evalModel.EndTime, currentPage, pagesize, out pagecount, out resultCount);
                evalModel.resultCount = resultCount;

                //企业类型
                string[] QYL = { "--请选择--", "国有企业", "集体企业", "股份合作企业", "有限责任公司", "联营企业", "中外合资经营企业", "中外合作经营企业" };
                List<SelectListItem> QYlist = new List<SelectListItem>();
                foreach (string s in QYL)
                {
                    QYlist.Add(new SelectListItem { Text = s, Value = s });
                }
                ViewBag.QYLX = new SelectList(QYlist, "Value", "Text");
                ViewData["strQYLX"] = strQYLX;

                //审核类型
                string[] SHLX = { "--请选择--", "未审批", "已审批", "审批未通过" };
                List<SelectListItem> SHlist = new List<SelectListItem>();
                foreach (string s in SHLX)
                {
                    SHlist.Add(new SelectListItem { Text = s, Value = s });
                }
                ViewBag.SHZT = new SelectList(SHlist, "Value", "Text");

                ViewData["strSHZT"] = strSHZT;
                return View("~/Views/T_SupQY/Index.cshtml", evalModel);
            }
            else
            {
                return RedirectToAction("Login", "WebMain");
            }
        }

        [CheckLogin()]
        public ActionResult Save(System.Int32 id, string tag)
        {
            T_SupQYModels model = new T_SupQYModels();
            model.DataModel = new T_SupQY();
            if (id != 0)
            {
                model.DataModel = T_SupQYDomain.GetInstance().GetModelById(id);
                model.QYZZList = T_QYZZDomain.GetInstance().GetQYZZByQyid(id);
            }
            //经营范围（下）
            T_JYFWModels jyfwmodels = new T_JYFWModels();
            jyfwmodels.DataModel = jyfwmodels.DataModel ?? new T_JYFW();
            model.JYFWList = T_JYFWDomain.GetInstance().GetAllT_JYFW(jyfwmodels.DataModel).ToList();

            model.Tag = tag;
            model.RoleCode = GetRoleCode();
            return View("~/Views/T_SupQY/Save.cshtml", model);
        }

        [HttpPost]
        [CheckLogin()]
        public void Save(T_SupQYModels model)
        {
            int result = 0;
            try
            {
                loginUser = Session["UserModel"] as SysUser;
                model.DataModel.WhsID = loginUser.UserCompanyID ?? 0;
                if (model.Tag == "Add")
                {
                    model.DataModel.SupStatus = 0;
                    result = T_SupQYDomain.GetInstance().AddModel(model.DataModel);
                }
                else if (model.Tag == "Edit")
                {
                    model.DataModel.SupStatus = 0;
                    result = T_SupQYDomain.GetInstance().UpdateModel(model.DataModel, model.DataModel.SupID);
                }
                //企业资质操作
                T_SupQYDomain.GetInstance().OperQyzz(model.DataModel.SupID, model.QYZZFiles);
            }
            catch { }
            Response.ContentType = "text/json";
            if (result > 0)
                Response.Write("{\"statusCode\":\"200\", \"message\":\"操作成功\",\"callbackType\":\"closeCurrentReloadTab\",\"forwardUrl\":\"/T_SupQY/Index\"}");
            else
                Response.Write("{\"statusCode\":\"300\", \"message\":\"操作失败\"}");
        }

        [CheckLogin()]
        public ActionResult AuditIndex(System.Int32 id, string tag)
        {
            T_SupQYModels model = new T_SupQYModels();
            model.DataModel = new T_SupQY();
            T_QYZZ zzmodel = new T_QYZZ();
            if (id != 0)
            {
                model.DataModel = T_SupQYDomain.GetInstance().GetModelById(id);
                model.QYZZList = T_QYZZDomain.GetInstance().GetAllT_QYZZ(zzmodel);
                if (model.QYZZList.Count > 0)
                {
                    model.QYZZList = model.QYZZList.Where(p => p.QYID == id).ToList();
                }
            }
            model.Tag = tag;
            return View("~/Views/T_SupQY/AuditIndex.cshtml", model);
        }

        [HttpPost]
        [CheckLogin()]
        public void through(T_SupQYModels model, int id)
        {
            int result = 0;
            try
            {
                Int32 supqyid = model.DataModel.SupID;
                result = T_SupQYDomain.GetInstance().Sh(supqyid, id);
            }
            catch { }
            Response.ContentType = "text/json";
            if (result > 0)
                Response.Write("{\"statusCode\":\"200\", \"message\":\"操作成功\",\"callbackType\":\"closeCurrentReloadTab\",\"forwardUrl\":\"/T_SupQY/Index\"}");
            else
                Response.Write("{\"statusCode\":\"300\", \"message\":\"操作失败\"}");
        }

        [HttpPost]
        [CheckLogin()]
        public void Delete(System.Int32 id)
        {
            var rCode = GetRoleCode();
            if (rCode != "1")
            {
                var temp = T_SupQYDomain.GetInstance().GetModelById(id);
                if (temp != null && (temp.SupStatus == 1))
                {
                    Response.Write("{\"statusCode\":\"300\", \"message\":\"已审批通过的数据不能删除！\"}");
                    return;
                }
            }
            Expression<Func<T_YLCP, bool>> where = p => (p.CPGYSID == id || p.CPSCQYID == id);
            var list = T_YLCPDomain.GetInstance().GetAllModels<int>(where);
            if (list != null && list.Count > 0)
            {
                Response.Write("{\"statusCode\":\"300\", \"message\":\"该企业下已有产品，不能删除！\"}");
                return;
            }
            Expression<Func<T_CGMX, bool>> whereCGD = p => (p.SupID == id || p.CPSCQYID == id);
            var lstCGMX = T_CGMXDomain.GetInstance().GetAllModels<int>(whereCGD);
            if (lstCGMX != null && lstCGMX.Count > 0)
            {
                Response.Write("{\"statusCode\":\"300\", \"message\":\"该企业下已有采购单，不能删除！\"}");
                return;
            }
            int result = T_SupQYDomain.GetInstance().DeleteModelById(id);
            Response.ContentType = "text/json";
            if (result > 0)
                Response.Write("{\"statusCode\":\"200\", \"message\":\"操作成功\",\"callbackType\":\"forward\",\"forwardUrl\":\"/T_SupQY/Index\"}");
            else
                Response.Write("{\"statusCode\":\"300\", \"message\":\"操作失败\"}");
        }

        [CheckLogin()]
        public ActionResult Upload(HttpPostedFileBase Filedata)
        {
            string uptype = Request["uptype"].ToString();
            string uploadDate = DateTime.Now.ToString("yyyymmddhhmmss");
            // 如果没有上传文件
            if (Filedata == null ||
                string.IsNullOrEmpty(Filedata.FileName) ||
                Filedata.ContentLength == 0)
            {
                return this.HttpNotFound();
            }
            string filename = uploadDate + Path.GetFileName(Filedata.FileName);
            //目录
            string directory;
            if (uptype == "0")
            {
                directory = Path.Combine(Server.MapPath("~/UploadFiles/"), "供货企业资质");
            }
            else
            {
                directory = Path.Combine(Server.MapPath("~/UploadFiles/"), "供货企业资质");
            }
            //上传文件记录上传到数据库中
            //上传顺序：先数据库中增加记录成功，再上传文件
            var fileUpload = new UploadFile();
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            string path = directory + "\\" + filename;
            Filedata.SaveAs(path);
            return Json(new { status = "OK", filename = filename, path = "/UploadFiles/供货企业资质/" + filename });
        }
        private string GetRoleCode()
        {
            return Session["RoleCode"] == null ? "" : Session["RoleCode"].ToString();
        }
    }
}
