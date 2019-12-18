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
    public class T_XSHTController : Controller
    {
        public int resultCount = 0; // 总条数 

        [CheckLogin()]
        public ActionResult Index(T_XSHTModels evalModel)
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
            evalModel.DataModel = evalModel.DataModel ?? new T_XSHT();

            if (Request["strHTCode"] != null)
            {
                string str = Request["strHTCode"].ToString();
                if (!String.IsNullOrEmpty(str))
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

            evalModel.DataList = T_XSHTDomain.GetInstance().PageT_XSHT(evalModel.DataModel, evalModel.StartTime, evalModel.EndTime, currentPage, pagesize, out pagecount, out resultCount);
            evalModel.resultCount = resultCount;
            return View("~/Views/T_XSHT/Index.cshtml", evalModel);
        }

        [CheckLogin()]
        public ActionResult Save(System.Int32 id, string tag)
        {
            //加载购买商商企业列表
            T_CusQYModels cusQymode = new T_CusQYModels();

            cusQymode.DataModel = cusQymode.DataModel ?? new T_CusQY();

            cusQymode.DataList = T_CusQYDomain.GetInstance().GetAllT_CusQY(cusQymode.DataModel).Where(p => p.CusStatus == Convert.ToInt32("1")).ToList();

            ViewData["CusQY"] = new SelectList(cusQymode.DataList, "CusID", "CusMC");
            T_XSHTModels model = new T_XSHTModels();          
            model.DataModel = new T_XSHT();
            model.DataModel.HTLX = "销售合同";
            if (id != 0)
            {
                model.DataModel = T_XSHTDomain.GetInstance().GetModelById(id);
            }
            model.Tag = tag;
            return View("~/Views/T_XSHT/Save.cshtml", model);
        }

        [HttpPost]
        [CheckLogin()]
        public void Save(T_XSHTModels model)
        {
            int result = 0;
            try
            {
                if (model.Tag == "Add")
                {
                    model.DataModel.SHJG = 0;
                    result = T_XSHTDomain.GetInstance().AddModel(model.DataModel);
                }
                else if (model.Tag == "Edit")
                {
                    result = T_XSHTDomain.GetInstance().UpdateModel(model.DataModel, model.DataModel.HTID);
                }

            }
            catch { }
            Response.ContentType = "text/json";
            if (result > 0)
                Response.Write("{\"statusCode\":\"200\", \"message\":\"操作成功\",\"callbackType\":\"closeCurrentReloadTab\",\"forwardUrl\":\"/T_XSHT/Index\"}");
            else
                Response.Write("{\"statusCode\":\"300\", \"message\":\"操作失败\"}");
        }

        [CheckLogin()]
        public ActionResult XSHTSPIndex(System.Int32 id, string tag)
        {
            //加载购买商商企业列表
            T_CusQYModels cusQymode = new T_CusQYModels();

            cusQymode.DataModel = cusQymode.DataModel ?? new T_CusQY();

            cusQymode.DataList = T_CusQYDomain.GetInstance().GetAllT_CusQY(cusQymode.DataModel).Where(p => p.CusStatus == Convert.ToInt32("1")).ToList();

            ViewData["CusQY"] = new SelectList(cusQymode.DataList, "CusID", "CusMC");

            T_XSHTModels model = new T_XSHTModels();
            model.DataModel = new T_XSHT();
            if (id != 0)
            {
                model.DataModel = T_XSHTDomain.GetInstance().GetModelById(id);

                if (model.DataModel.HTFJ != "" && model.DataModel.ZLXY != "")
                {
                    ViewData["HTFJ"] = model.DataModel.HTFJ;
                    ViewData["ZLXY"] = model.DataModel.ZLXY;
                }
            }
            model.Tag = tag;
            return View("~/Views/T_XSHT/XSHTSPIndex.cshtml", model);
        }

        [HttpPost]
        [CheckLogin()]
        public void through(T_XSHTModels model, string id)
        {
            int result = 0;
            try
            {

                Int32 xsid = model.DataModel.HTID;
                T_XSHT temp = T_XSHTDomain.GetInstance().GetModelById(xsid);
                model.DataModel.HTQDDWID = temp.HTQDDWID;
                if (id == "1")
                {
                    model.DataModel.SHJG = 1;
                }
                else
                {
                    model.DataModel.SHJG = 2;
                }
                result = T_XSHTDomain.GetInstance().UpdateModel(model.DataModel, model.DataModel.HTID);
            }
            catch { }
            Response.ContentType = "text/json";
            if (result > 0)
                Response.Write("{\"statusCode\":\"200\", \"message\":\"操作成功\",\"callbackType\":\"closeCurrentReloadTab\",\"forwardUrl\":\"/T_XSHT/Index\"}");
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
                directory = Path.Combine(Server.MapPath("~/UploadFiles/"), "销售合同附件");
            }
            else
            {
                directory = Path.Combine(Server.MapPath("~/UploadFiles/"), "销售质量协议");
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
            int result = T_XSHTDomain.GetInstance().DeleteModelById(id);
            Response.ContentType = "text/json";
            if (result > 0)
                Response.Write("{\"statusCode\":\"200\", \"message\":\"操作成功\",\"callbackType\":\"forward\",\"forwardUrl\":\"/T_XSHT/Index\"}");
            else
                Response.Write("{\"statusCode\":\"300\", \"message\":\"操作失败\"}");
        }
    }
}
