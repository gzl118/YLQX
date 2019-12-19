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
    public class T_WhsQYController : Controller
    {
        public int resultCount = 0; // 总条数 

        [CheckLogin()]
        public ActionResult Index(T_WhsQYModels evalModel)
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
            evalModel.DataModel = evalModel.DataModel ?? new T_WhsQY();

            if (Request["strWhsMC"] != null)
            {
                string str = Request["strWhsMC"].ToString();
                if (!String.IsNullOrEmpty(str))
                {
                    evalModel.DataModel.WhsMC = str;
                }
            }

            evalModel.DataList = T_WhsQYDomain.GetInstance().PageT_WhsQY(evalModel.DataModel, evalModel.StartTime, evalModel.EndTime, currentPage, pagesize, out pagecount, out resultCount);
            evalModel.resultCount = resultCount;
            GetRoleCode(evalModel);
            return View("~/Views/T_WhsQY/Index.cshtml", evalModel);
        }

        [CheckLogin()]
        public ActionResult Save(System.Int32 id, string tag)
        {
            T_WhsQYModels model = new T_WhsQYModels();
            model.DataModel = new T_WhsQY();
            if (id != 0)
            {
                model.DataModel = T_WhsQYDomain.GetInstance().GetModelById(id);
                model.WhsQYZZList = T_WhsQYZZDomain.GetInstance().GetQYZZByQyid(id);
            }
            //经营范围（下）
            T_JYFWModels jyfwmodels = new T_JYFWModels();
            jyfwmodels.DataModel = jyfwmodels.DataModel ?? new T_JYFW();
            model.JYFWList = T_JYFWDomain.GetInstance().GetAllT_JYFW(jyfwmodels.DataModel).ToList();
            model.Tag = tag;
            GetRoleCode(model);
            return View("~/Views/T_WhsQY/Save.cshtml", model);
        }

        [HttpPost]
        [CheckLogin()]
        public void Save(T_WhsQYModels model)
        {
            int result = 0;
            try
            {
                if (model.Tag == "Add")
                {
                    result = T_WhsQYDomain.GetInstance().AddModel(model.DataModel);
                }
                else if (model.Tag == "Edit")
                {
                    result = T_WhsQYDomain.GetInstance().UpdateModel(model.DataModel, model.DataModel.WhsID);
                }
                //企业资质操作
                T_WhsQYDomain.GetInstance().OperQyzz(model.DataModel.WhsID, model.QYZZFiles);
            }
            catch { }
            Response.ContentType = "text/json";
            if (result > 0)
                Response.Write("{\"statusCode\":\"200\", \"message\":\"操作成功\",\"callbackType\":\"closeCurrentReloadTab\",\"forwardUrl\":\"/T_WhsQY/Index\"}");
            else
                Response.Write("{\"statusCode\":\"300\", \"message\":\"操作失败\"}");
        }

        [HttpPost]
        [CheckLogin()]
        public void Delete(System.Int32 id)
        {
            int result = T_WhsQYDomain.GetInstance().DeleteModelById(id);
            Response.ContentType = "text/json";
            if (result > 0)
                Response.Write("{\"statusCode\":\"200\", \"message\":\"操作成功\",\"callbackType\":\"forward\",\"forwardUrl\":\"/T_WhsQY/Index\"}");
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
                directory = Path.Combine(Server.MapPath("~/UploadFiles/"), "本企业资质");
            }
            else
            {
                directory = Path.Combine(Server.MapPath("~/UploadFiles/"), "本企业资质");
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
            return Json(new { status = "OK", filename = filename, path = "/UploadFiles/本企业资质/" + filename });
        }
        private void GetRoleCode(T_WhsQYModels evalModel)
        {
            evalModel.RoleCode = Session["RoleCode"] == null ? "" : Session["RoleCode"].ToString();
        }
    }
}
