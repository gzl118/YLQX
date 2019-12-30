using MedicalApparatusManage.Domain;
using MedicalApparatusManage.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MedicalApparatusManage.Controllers
{
    public class T_CusQYController : Controller
    {
        public int resultCount = 0; // 总条数 
        public SysUser CurUser = null;

        [CheckLogin()]
        public ActionResult Index(T_CusQYModels evalModel)
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
            evalModel.DataModel = evalModel.DataModel ?? new T_CusQY();

            if (Request["strCusMc"] != null)
            {
                string str = Request["strCusMc"].ToString();
                if (!String.IsNullOrEmpty(str))
                {
                    evalModel.DataModel.CusMC = str;
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
                        evalModel.DataModel.CusDWXZ = strQYLX;
                    }

                }
            }
            string strSHZT = "--请选择--";
            if (Request["strSHZT"] != null)
            {
                strSHZT = Request["strSHZT"].ToString();
                if (!String.IsNullOrEmpty(strSHZT))
                {
                    if (strSHZT == "已审批")
                    {
                        evalModel.DataModel.CusStatus = 1;
                    }
                    else if (strSHZT == "审批未通过")
                    {
                        evalModel.DataModel.CusStatus = 2;
                    }
                    else if (strSHZT == "未审批")
                    {
                        evalModel.DataModel.CusStatus = 0;
                    }
                }
            }
            evalModel.DataList = T_CusQYDomain.GetInstance().PageT_CusQY(evalModel.DataModel, evalModel.StartTime, evalModel.EndTime, currentPage, pagesize, out pagecount, out resultCount);
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
            return View("~/Views/T_CusQY/Index.cshtml", evalModel);
        }

        [CheckLogin()]
        public ActionResult Save(System.Int32 id, string tag)
        {
            T_CusQYModels model = new T_CusQYModels();
            model.DataModel = new T_CusQY();
            if (id != 0)
            {
                model.DataModel = T_CusQYDomain.GetInstance().GetModelById(id);
                model.CusQYZZList = T_CusQYZZDomain.GetInstance().GetQYZZByQyid(id);
            }
            //经营范围（下）
            T_JYFWModels jyfwmodels = new T_JYFWModels();
            jyfwmodels.DataModel = jyfwmodels.DataModel ?? new T_JYFW();
            model.JYFWList = T_JYFWDomain.GetInstance().GetAllT_JYFW(jyfwmodels.DataModel).ToList();
            model.Tag = tag;
            model.RoleCode = GetRoleCode();
            return View("~/Views/T_CusQY/Save.cshtml", model);
        }

        [HttpPost]
        [CheckLogin()]
        public void Save(T_CusQYModels model)
        {
            int result = 0;
            CurUser = Session["UserModel"] as SysUser;
            model.DataModel.WhsQYID = CurUser.UserCompanyID ?? 0;
            try
            {
                if (model.Tag == "Add")
                {
                    model.DataModel.CusStatus = 0;
                    result = T_CusQYDomain.GetInstance().AddModel(model.DataModel);
                }
                else if (model.Tag == "Edit")
                {
                    model.DataModel.CusStatus = 0;
                    result = T_CusQYDomain.GetInstance().UpdateModel(model.DataModel, model.DataModel.CusID);
                }
                //企业资质操作
                T_CusQYDomain.GetInstance().OperQyzz(model.DataModel.CusID, model.QYZZFiles);
            }
            catch { }
            Response.ContentType = "text/json";
            if (result > 0)
                Response.Write("{\"statusCode\":\"200\", \"message\":\"操作成功\",\"callbackType\":\"closeCurrentReloadTab\",\"forwardUrl\":\"/T_CusQY/Index\"}");
            else
                Response.Write("{\"statusCode\":\"300\", \"message\":\"操作失败\"}");
        }

        [CheckLogin()]
        public ActionResult CusAuditIndex(System.Int32 id, string tag)
        {
            T_CusQYModels model = new T_CusQYModels();
            model.DataModel = new T_CusQY();
            T_CusQYZZ zzmodel = new T_CusQYZZ();
            if (id != 0)
            {
                model.DataModel = T_CusQYDomain.GetInstance().GetModelById(id);
                model.CusQYZZList = T_CusQYZZDomain.GetInstance().GetAllT_BJD(zzmodel);
                if (model.CusQYZZList.Count > 0)
                {
                    model.CusQYZZList = model.CusQYZZList.Where(p => p.QYID == id).ToList();
                }
            }
            model.Tag = tag;
            return View("~/Views/T_CusQY/CusAuditIndex.cshtml", model);
        }

        [HttpPost]
        [CheckLogin()]
        public void through(T_CusQYModels model, int id)
        {
            int result = 0;
            try
            {
                Int32 cusqyid = model.DataModel.CusID;
                result = T_CusQYDomain.GetInstance().Sh(cusqyid, id);
            }
            catch { }
            Response.ContentType = "text/json";
            if (result > 0)
                Response.Write("{\"statusCode\":\"200\", \"message\":\"操作成功\",\"callbackType\":\"closeCurrentReloadTab\",\"forwardUrl\":\"/T_CusQY/Index\"}");
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
                var temp = T_CusQYDomain.GetInstance().GetModelById(id);
                if (temp != null && (temp.CusStatus == 1))
                {
                    Response.Write("{\"statusCode\":\"300\", \"message\":\"已审批通过的数据不能删除！\"}");
                    return;
                }
            }
            int result = T_CusQYDomain.GetInstance().Delete(id);
            Response.ContentType = "text/json";
            if (result > 0)
                Response.Write("{\"statusCode\":\"200\", \"message\":\"操作成功\",\"callbackType\":\"forward\",\"forwardUrl\":\"/T_CusQY/Index\"}");
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
                directory = Path.Combine(Server.MapPath("~/UploadFiles/"), "购货企业资质");
            }
            else
            {
                directory = Path.Combine(Server.MapPath("~/UploadFiles/"), "购货企业资质");
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
            return Json(new { status = "OK", filename = filename, path = "/UploadFiles/购货企业资质/" + filename });
        }
        private string GetRoleCode()
        {
            return Session["RoleCode"] == null ? "" : Session["RoleCode"].ToString();
        }
    }
}
