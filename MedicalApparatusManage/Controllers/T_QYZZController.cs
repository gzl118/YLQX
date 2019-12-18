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
    public class T_QYZZController : Controller
    {
        public int resultCount = 0; // 总条数 

        [CheckLogin()]
        public ActionResult Index(T_QYZZModels evalModel,string id)
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


            evalModel.DataModel = evalModel.DataModel ?? new T_QYZZ();

            if (Request["strZZMC"] != null)
            {
                string str = Request["strZZMC"].ToString();
                if (!String.IsNullOrEmpty(str))
                {
                    evalModel.DataModel.ZZMC = str.Trim();
                }
            }
            if (Request["strZZLX"] != null)
            {
                string str = Request["strZZLX"].ToString();
                if (!String.IsNullOrEmpty(str))
                {
                    evalModel.DataModel.ZZLX = str;
                }
            }

            if (id != null && id != "")
            {
                evalModel.DataList = T_QYZZDomain.GetInstance().PageT_QYZZ(evalModel.DataModel, evalModel.StartTime, evalModel.EndTime, currentPage, pagesize, out pagecount, out resultCount).Where(p => p.QYID == int.Parse(id)).ToList();
            }
            else 
            {
                evalModel.DataList = T_QYZZDomain.GetInstance().PageT_QYZZ(evalModel.DataModel, evalModel.StartTime, evalModel.EndTime, currentPage, pagesize, out pagecount, out resultCount);
            }
            evalModel.resultCount = resultCount;
            return View("~/Views/T_QYZZ/Index.cshtml", evalModel);
        }

        [CheckLogin()]
        public ActionResult Save(System.Int32 id, string tag)
        {
            T_QYZZModels model = new T_QYZZModels();

            //加载批发商企业列表
            T_SupQYModels supQymode = new T_SupQYModels();

            supQymode.DataModel = supQymode.DataModel ?? new T_SupQY();

            supQymode.DataList = T_SupQYDomain.GetInstance().GetAllT_SupQY(supQymode.DataModel).Where(p => p.SupStatus == Convert.ToInt32("1")).ToList();

            ViewData["SupQY"] = new SelectList(supQymode.DataList, "SupID", "SupMC", "请选择");

            model.DataModel = new T_QYZZ();
            if (id != 0)
            {
                model.DataModel = T_QYZZDomain.GetInstance().GetModelById(id);
            }
            model.Tag = tag;
            return View("~/Views/T_QYZZ/Save.cshtml", model);
        }

        [HttpPost]
        [CheckLogin()]
        public void Save(T_QYZZModels model)
        {
            int result = 0;
            try
            {
                if (model.Tag == "Add")
                {
                    result = T_QYZZDomain.GetInstance().AddModel(model.DataModel);
                }
                else if (model.Tag == "Edit")
                {
                    result = T_QYZZDomain.GetInstance().UpdateModel(model.DataModel, model.DataModel.ZZID);
                }

            }
            catch { }
            Response.ContentType = "text/json";
            if (result > 0)
                Response.Write("{\"statusCode\":\"200\", \"message\":\"操作成功\",\"callbackType\":\"closeCurrentReloadTab\",\"forwardUrl\":\"/T_QYZZ/Index\"}");
            else
                Response.Write("{\"statusCode\":\"300\", \"message\":\"操作失败\"}");
        }

        [CheckLogin()]
        public ActionResult Upload(HttpPostedFileBase Filedata)
        {
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
            string directory = Path.Combine(Server.MapPath("~/UploadFiles/"), "供货商资料");

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
            T_QYZZModels model = new T_QYZZModels();
            model.DataModel = new T_QYZZ();
            if (id != 0)
            {
                model.DataModel = T_QYZZDomain.GetInstance().GetModelById(id);
            }
            string filePath = Path.Combine(Server.MapPath("~/UploadFiles/"), "供货商资料", model.DataModel.ZZFJ);
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }
            int result = T_QYZZDomain.GetInstance().DeleteModelById(id);
            Response.ContentType = "text/json";
            if (result > 0)
                Response.Write("{\"statusCode\":\"200\", \"message\":\"操作成功\",\"callbackType\":\"forward\",\"forwardUrl\":\"/T_QYZZ/Index\"}");
            else
                Response.Write("{\"statusCode\":\"300\", \"message\":\"操作失败\"}");
        }
    }
}
