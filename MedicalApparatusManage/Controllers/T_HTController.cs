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
    public class T_HTController : Controller
    {
        public int resultCount = 0; // 总条数 

        [CheckLogin()]
        public ActionResult Index(T_HTModels evalModel)
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
            evalModel.DataModel = evalModel.DataModel ?? new T_HT();

            if (Request["strHTCode"] != null)
            {
                string str = Request["strHTCode"].ToString();
                if(!String.IsNullOrEmpty(str))
                {
                    evalModel.DataModel.HTBH = str.Trim();
                }
            }

            if (Request["strHTMC"] != null)
            {
                string str = Request["strHTMC"].ToString();
                if (!String.IsNullOrEmpty(str))
                {
                    evalModel.DataModel.HTMC = str.Trim();
                }
            }

            evalModel.DataList = T_HTDomain.GetInstance().PageT_HT(evalModel.DataModel, evalModel.StartTime, evalModel.EndTime, currentPage, pagesize, out pagecount, out resultCount);
            evalModel.resultCount = resultCount;
            return View("~/Views/T_HT/Index.cshtml", evalModel);
        }

        [CheckLogin()]
        public ActionResult Save(System.Int32 id, string tag)
        {

            //加载购买商商企业列表
            //T_CGDModels cgdQymode = new T_CGDModels();

            //cgdQymode.DataModel = cgdQymode.DataModel ?? new T_CGD();

            //cgdQymode.DataList = T_CGDDomain.GetInstance().GetAllT_CGD(cgdQymode.DataModel);

            //ViewData["CGD"] = new SelectList(cgdQymode.DataList, "CGID", "CGDMC");

            //加载企业列表
            T_SupQYModels supmode = new T_SupQYModels();

            supmode.DataModel = supmode.DataModel ?? new T_SupQY();

            supmode.DataList = T_SupQYDomain.GetInstance().GetAllT_SupQY(supmode.DataModel).Where(p => p.SupStatus == 1).ToList();

            ViewData["SupID"] = new SelectList(supmode.DataList, "SupID", "SupMC");

            T_HTModels model = new T_HTModels();
            model.DataModel = new T_HT();
            if (id != 0)
            {
                model.DataModel = T_HTDomain.GetInstance().GetModelById(id);
            }
            model.Tag = tag;
            return View("~/Views/T_HT/Save.cshtml", model);
        }

        [HttpPost]
        [CheckLogin()]
        public void Save(T_HTModels model)
        {
            int result = 0;
            try
            {
                if (model.Tag == "Add")
                {
                    model.DataModel.SHJG = 0;
                    result = T_HTDomain.GetInstance().AddModel(model.DataModel);
                }
                else if (model.Tag == "Edit")
                {
                    result = T_HTDomain.GetInstance().UpdateModel(model.DataModel, model.DataModel.HTID);
                }

            }
            catch { }
            Response.ContentType = "text/json";
            if (result > 0)
                Response.Write("{\"statusCode\":\"200\", \"message\":\"操作成功\",\"callbackType\":\"closeCurrentReloadTab\",\"forwardUrl\":\"/T_HT/Index\"}");
            else
                Response.Write("{\"statusCode\":\"300\", \"message\":\"操作失败\"}");
        }

        [CheckLogin()]
        public ActionResult HTSPIndex(System.Int32 id, string tag)
        {
            T_HTModels model = new T_HTModels();
            model.DataModel = new T_HT();
            if (id != 0)
            {
                model.DataModel = T_HTDomain.GetInstance().GetModelById(id);
                //加载企业列表
                T_SupQYModels supmode = new T_SupQYModels();

                supmode.DataModel = supmode.DataModel ?? new T_SupQY();

                supmode.DataList = T_SupQYDomain.GetInstance().GetAllT_SupQY(supmode.DataModel).Where(p => p.SupStatus == 1).ToList();

                ViewData["SupID"] = new SelectList(supmode.DataList, "SupID", "SupMC");
            }
            model.Tag = tag;
            return View("~/Views/T_HT/HTSPIndex.cshtml", model);
        }

        [HttpPost]
        [CheckLogin()]
        public void through(T_HTModels model, string id)
        {
            int result = 0;
            try
            {
                Int32 xsdid = model.DataModel.HTID;
                T_HT temp = T_HTDomain.GetInstance().GetModelById(xsdid);

                model.DataModel.HTQDDWID = temp.HTQDDWID;

                if (id == "1")
                {
                    model.DataModel.SHJG = 1;
                }
                else
                {
                    model.DataModel.SHJG = 2;
                }
                result = T_HTDomain.GetInstance().UpdateModel(model.DataModel, model.DataModel.HTID);
            }
            catch { }
            Response.ContentType = "text/json";
            if (result > 0)
                Response.Write("{\"statusCode\":\"200\", \"message\":\"操作成功\",\"callbackType\":\"closeCurrentReloadTab\",\"forwardUrl\":\"/T_HT/Index\"}");
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
                directory = Path.Combine(Server.MapPath("~/UploadFiles/"), "采购合同附件");
            }
            else
            {
                directory = Path.Combine(Server.MapPath("~/UploadFiles/"), "采购质量协议");
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
            T_HTModels model = new T_HTModels();
            model.DataModel = new T_HT();
            if (id != 0)
            {
                model.DataModel = T_HTDomain.GetInstance().GetModelById(id);
            }
            string filePath = Path.Combine(Server.MapPath("~/UploadFiles/"), "采购合同附件", model.DataModel.HTFJ);
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }
            string filePath1 = Path.Combine(Server.MapPath("~/UploadFiles/"), "采购质量协议", model.DataModel.ZLXY);
            if (System.IO.File.Exists(filePath1))
            {
                System.IO.File.Delete(filePath1);
            }
            int result = T_HTDomain.GetInstance().DeleteModelById(id);
            Response.ContentType = "text/json";
            if (result > 0)
                Response.Write("{\"statusCode\":\"200\", \"message\":\"操作成功\",\"callbackType\":\"forward\",\"forwardUrl\":\"/T_HT/Index\"}");
            else
                Response.Write("{\"statusCode\":\"300\", \"message\":\"操作失败\"}");
        }
    }
}
