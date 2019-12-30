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
    public class T_PersonController : Controller
    {
        public int resultCount = 0; // 总条数 

        [CheckLogin()]
        public ActionResult Index(T_PersonModels evalModel)
        {
            string strName = "";
            string strCode = "";
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

            evalModel.DataModel = evalModel.DataModel ?? new T_Person();

            if (Request["strName"] != null)
            {
                strName = Request["strName"].ToString();
                if (!String.IsNullOrEmpty(strName))
                {
                    evalModel.DataModel.PsMZ = strName;
                }
                ViewData["strName"] = strName;
            }

            if (Request["strCode"] != null)
            {
                strCode = Request["strCode"].ToString();
                if (!String.IsNullOrEmpty(strCode))
                {
                    evalModel.DataModel.PsSFZ = strCode;
                }
                ViewData["strCode"] = strCode;
            }

            evalModel.DataList = T_PersonDomain.GetInstance().PageT_Person(evalModel.DataModel, evalModel.StartTime, evalModel.EndTime, currentPage, pagesize, out pagecount, out resultCount);
            evalModel.resultCount = resultCount;


            return View("~/Views/T_Person/Index.cshtml", evalModel);
        }

        [CheckLogin()]
        public ActionResult Save(System.Int32 id, string tag)
        {
            T_PersonModels model = new T_PersonModels();

            //加载批发商企业列表
            T_WhsQYModels whsQymode = new T_WhsQYModels();

            whsQymode.DataModel = whsQymode.DataModel ?? new T_WhsQY();

            whsQymode.DataList = T_WhsQYDomain.GetInstance().GetAllT_WhsQY(whsQymode.DataModel);

            ViewData["WHSQY"] = new SelectList(whsQymode.DataList, "WhsID", "WhsMC", "请选择");

            model.DataModel = new T_Person();
            if (id != 0)
            {
                model.DataModel = T_PersonDomain.GetInstance().GetModelById(id);
            }
            model.Tag = tag;

            //加载性别类型
            List<SelectListItem> list = new List<SelectListItem>();
            list.Add(new SelectListItem { Text = "男", Value = "男" });
            list.Add(new SelectListItem { Text = "女", Value = "女" });

            ViewData["XB"] = new SelectList(list, "Value", "Text", "请选择");

            //加载学历类型
            List<SelectListItem> xlList = new List<SelectListItem>();
            xlList.Add(new SelectListItem { Text = "大专", Value = "大专" });
            xlList.Add(new SelectListItem { Text = "本科", Value = "本科" });
            xlList.Add(new SelectListItem { Text = "硕士", Value = "硕士" });
            xlList.Add(new SelectListItem { Text = "博士", Value = "博士" });

            ViewData["XL"] = new SelectList(xlList, "Value", "Text", "请选择");

            return View("~/Views/T_Person/Save.cshtml", model);
        }

        [HttpPost]
        [CheckLogin()]
        public void Save(T_PersonModels model)
        {
            int result = 0;
            try
            {
                if (model.Tag == "Add")
                {
                    result = T_PersonDomain.GetInstance().AddModel(model.DataModel);
                }
                else if (model.Tag == "Edit")
                {
                    result = T_PersonDomain.GetInstance().UpdateModel(model.DataModel, model.DataModel.PsID);
                }

            }
            catch { }
            Response.ContentType = "text/json";
            if (result > 0)
                Response.Write("{\"statusCode\":\"200\", \"message\":\"操作成功\",\"callbackType\":\"closeCurrentReloadTab\",\"forwardUrl\":\"/T_Person/Index\"}");
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
                directory = Path.Combine(Server.MapPath("~/UploadFiles/"), "本企业人员身份证扫描件");
            }
            else
            {
                directory = Path.Combine(Server.MapPath("~/UploadFiles/"), "本企业人员资质");
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
            T_PersonModels model = new T_PersonModels();
            model.DataModel = new T_Person();
            if (id != 0)
            {
                model.DataModel = T_PersonDomain.GetInstance().GetModelById(id);
            }
            if (model.DataModel.PsSFZSM != null)
            {
                string filePath = Path.Combine(Server.MapPath("~/UploadFiles/"), "本企业人员身份证扫描件", model.DataModel.PsSFZSM);
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
            }
            if (model.DataModel.PsZZFJ != null)
            {
                string filePath1 = Path.Combine(Server.MapPath("~/UploadFiles/"), "本企业人员资质", model.DataModel.PsZZFJ);
                if (System.IO.File.Exists(filePath1))
                {
                    System.IO.File.Delete(filePath1);
                }
            }
            int result = T_PersonDomain.GetInstance().DeleteModelById(id);
            Response.ContentType = "text/json";
            if (result > 0)
                Response.Write("{\"statusCode\":\"200\", \"message\":\"操作成功\",\"callbackType\":\"forward\",\"forwardUrl\":\"/T_Person/Index\"}");
            else
                Response.Write("{\"statusCode\":\"300\", \"message\":\"操作失败\"}");
        }
    }
}
