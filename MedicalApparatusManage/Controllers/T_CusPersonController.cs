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
    public class T_CusPersonController : Controller
    {
        public int resultCount = 0; // 总条数 

        [CheckLogin()]
        public ActionResult Index(T_CusPersonModels evalModel)
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
            evalModel.DataModel = evalModel.DataModel ?? new T_CusPerson();

            if (Request["strName"] != null)
            {
                string str = Request["strName"].ToString();
                if (!String.IsNullOrEmpty(str))
                {
                    evalModel.DataModel.PsMZ = str;
                }
            }

            if (Request["strCode"] != null)
            {
                string str = Request["strCode"].ToString();
                if (!String.IsNullOrEmpty(str))
                {
                    evalModel.DataModel.PsSFZ = str;
                }
            }
           
            evalModel.DataList = T_CusPersonDomain.GetInstance().PageT_CusPerson(evalModel.DataModel, evalModel.StartTime, evalModel.EndTime, currentPage, pagesize, out pagecount, out resultCount);
            evalModel.resultCount = resultCount;
            return View("~/Views/T_CusPerson/Index.cshtml", evalModel);
        }

        [CheckLogin()]
        public ActionResult Save(System.Int32 id, string tag)
        {
            T_CusPersonModels model = new T_CusPersonModels();
            model.DataModel = new T_CusPerson();

            //加载购买商商企业列表
            T_CusQYModels cusQymode = new T_CusQYModels();

            cusQymode.DataModel = cusQymode.DataModel ?? new T_CusQY();

            cusQymode.DataList = T_CusQYDomain.GetInstance().GetAllT_CusQY(cusQymode.DataModel).Where(p=>p.CusStatus == Convert.ToInt32("1")).ToList();

            ViewData["CusQY"] = new SelectList(cusQymode.DataList, "CusID", "CusMC");

            if (id != 0)
            {
                model.DataModel = T_CusPersonDomain.GetInstance().GetModelById(id);
            }
            model.Tag = tag;
            return View("~/Views/T_CusPerson/Save.cshtml", model);
        }

        [HttpPost]
        [CheckLogin()]
        public void Save(T_CusPersonModels model)
        {
            int result = 0;
            try
            {
                if (model.Tag == "Add")
                {
                    result = T_CusPersonDomain.GetInstance().AddModel(model.DataModel);
                }
                else if (model.Tag == "Edit")
                {
                    result = T_CusPersonDomain.GetInstance().UpdateModel(model.DataModel, model.DataModel.PsID);
                }

            }
            catch { }
            Response.ContentType = "text/json";
            if (result > 0)
                Response.Write("{\"statusCode\":\"200\", \"message\":\"操作成功\",\"callbackType\":\"closeCurrentReloadTab\",\"forwardUrl\":\"/T_CusPerson/Index\"}");
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
                directory = Path.Combine(Server.MapPath("~/UploadFiles/"), "购货者身份证扫描件");
            }
            else
            {
                directory = Path.Combine(Server.MapPath("~/UploadFiles/"), "购货者职称附件");
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
            return Json(new { status = "OK", filename = filename });
        } 

        [HttpPost]
        [CheckLogin()]
        public void Delete(System.Int32 id)
        {
            T_CusPersonModels model = new T_CusPersonModels();
            model.DataModel = new T_CusPerson();
            if (id != 0)
            {
                model.DataModel = T_CusPersonDomain.GetInstance().GetModelById(id);
            }
            string filePath = Path.Combine(Server.MapPath("~/UploadFiles/"), "购货者身份证扫描件", model.DataModel.PsSFZSM);
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }
            string filePath1 = Path.Combine(Server.MapPath("~/UploadFiles/"), "购货者职称附件", model.DataModel.PsZZFJ);
            if (System.IO.File.Exists(filePath1))
            {
                System.IO.File.Delete(filePath1);
            }
            int result = T_CusPersonDomain.GetInstance().DeleteModelById(id);
            Response.ContentType = "text/json";
            if (result > 0)
                Response.Write("{\"statusCode\":\"200\", \"message\":\"操作成功\",\"callbackType\":\"forward\",\"forwardUrl\":\"/T_CusPerson/Index\"}");
            else
                Response.Write("{\"statusCode\":\"300\", \"message\":\"操作失败\"}");
        }
    }
}
