using MedicalApparatusManage.Common;
using MedicalApparatusManage.Domain;
using MedicalApparatusManage.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Web;
using System.Web.Mvc;


namespace MedicalApparatusManage.Controllers
{
    public class T_YLCPController : Controller
    {
        public int resultCount = 0; // 总条数 
        public SysUser CurUser = null;

        [CheckLogin()]
        public ActionResult Index(T_YLCPModels evalModel)
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
            string strscqy = "";
            int pagesize = Convert.ToInt32(evalModel.pageSize);
            int pagecount = Convert.ToInt32(evalModel.pagecount);
            int currentPage = Convert.ToInt32(evalModel.currentPage);
            evalModel.DataModel = evalModel.DataModel ?? new T_YLCP();
            if (Request["strCPMC"] != null)
            {
                string str = Request["strCPMC"].ToString();
                if (!String.IsNullOrEmpty(str))
                {
                    evalModel.DataModel.CPMC = str.Trim();
                }
                ViewData["strCPMC"] = str;
            }

            if (Request["strCPSCQY"] != null)
            {
                strscqy = Request["strCPSCQY"].ToString();
                if (!String.IsNullOrEmpty(strscqy))
                {
                    evalModel.DataModel.CPSCQYID = int.Parse(strscqy);
                }
            }
            var list = T_SupQYDomain.GetInstance().GetAllT_SupQY(new T_SupQY() { SupStatus = 1 }).ToList();
            ViewBag.Persons = new SelectList(list, "SupID", "SupMC", "请选择");
            ViewData["strCPSCQY"] = strscqy;
            evalModel.DataList = T_YLCPDomain.GetInstance().PageT_YLCP(evalModel.DataModel, evalModel.StartTime, evalModel.EndTime, currentPage, pagesize, out pagecount, out resultCount);
            evalModel.resultCount = resultCount;
            return View("~/Views/T_YLCP/Index.cshtml", evalModel);
        }

        [CheckLogin()]
        public ActionResult Save(System.Int32 id, string tag)
        {
            T_YLCPModels model = new T_YLCPModels();
            CurUser = Session["UserModel"] as SysUser;

            //加载批发商企业列表
            T_SupQYModels supQymode = new T_SupQYModels();

            supQymode.DataModel = supQymode.DataModel ?? new T_SupQY();

            supQymode.DataList = T_SupQYDomain.GetInstance().GetAllT_SupQY(supQymode.DataModel).Where(p => p.SupStatus == Convert.ToInt32("1")).ToList();

            ViewData["SUPQY"] = new SelectList(supQymode.DataList, "SupID", "SupMC", "请选择");

            //加载产品类型列表
            T_CPLXModels cplxQymode = new T_CPLXModels();

            cplxQymode.DataModel = cplxQymode.DataModel ?? new T_CPLX();

            cplxQymode.DataList = T_CPLXDomain.GetInstance().GetAllT_CPLX(cplxQymode.DataModel);

            ViewData["CPLX"] = new SelectList(cplxQymode.DataList, "LXID", "LXMC", "请选择");

            //加载仓库列表
            T_CKModels ckmode = new T_CKModels();

            ckmode.DataModel = ckmode.DataModel ?? new T_CK();

            ckmode.DataList = T_CKDomain.GetInstance().GetAllT_CK(ckmode.DataModel);

            ViewData["CK"] = new SelectList(ckmode.DataList, "CKID", "CKMC");

            model.DataModel = new T_YLCP();

            model.DataModel.CPLRRQ = DateTime.Now;
            model.DataModel.CPLRR = CurUser.UserAccount;
            if (id != 0)
            {
                model.DataModel = T_YLCPDomain.GetInstance().GetModelById(id);
                model.YLCPZZList = T_YLCPZZDomain.GetInstance().GetCPZZByCpid(id);
            }
            else
            {
                model.DataModel.CPBH = T_YLCPDomain.GetInstance().GetCpOrderNum("CP", CurUser);
            }
            Expression<Func<T_PackingUnit, bool>> where = PredicateBuilder.True<T_PackingUnit>();
            var lstUnit = T_PackingUnitDomain.GetInstance().GetAllModels<int>(where);
            ViewBag.PUnit = new SelectList(lstUnit, "PUName", "PUName");
            model.Tag = tag;
            model.RoleCode = GetRoleCode();
            return View("~/Views/T_YLCP/Save.cshtml", model);
        }

        [HttpPost]
        [CheckLogin()]
        public void Save(T_YLCPModels model)
        {
            int result = 0;
            try
            {
                if (model.Tag == "Add")
                {
                    model.DataModel.CPStatus = 0;
                    result = T_YLCPDomain.GetInstance().AddModel(model.DataModel);
                }
                else if (model.Tag == "Edit")
                {
                    model.DataModel.CPStatus = 0;
                    result = T_YLCPDomain.GetInstance().UpdateModel(model.DataModel, model.DataModel.CPID);

                }
                //产品资质操作
                T_YLCPDomain.GetInstance().OperCpzz(model.DataModel.CPID, model.CPZZFiles);
            }
            catch { }
            Response.ContentType = "text/json";
            if (result > 0)
                Response.Write("{\"statusCode\":\"200\", \"message\":\"操作成功\",\"callbackType\":\"closeCurrentReloadTab\",\"forwardUrl\":\"/T_YLCP/Index\"}");
            else
                Response.Write("{\"statusCode\":\"300\", \"message\":\"操作失败\"}");
        }

        [HttpPost]
        [CheckLogin()]
        public JsonResult GetYLCP(string cpid)
        {
            T_YLCP cp = T_YLCPDomain.GetInstance().GetModelById(Convert.ToInt32(cpid));
            if (cp != null)
            {
                string scqyMc = "";
                if (cp.CPSCQYID.HasValue)
                {
                    var scqy = T_SupQYDomain.GetInstance().GetModelById(cp.CPSCQYID);
                    if (scqy != null && !string.IsNullOrEmpty(scqy.SupMC))
                    {
                        scqyMc = scqy.SupMC;
                    }
                }

                string resultStr = JsonConvert.SerializeObject(new { CPGG = cp.CPGG, CPPrice = cp.CPPrice, CPDW = cp.CPDW, SCQYMC = scqyMc, SCQYID = cp.CPSCQYID });
                //string str = "{\"CPGG\":\"" + cp.CPGG + "\"}";
                return Json(resultStr);
            }
            else
            {
                return Json("");
            }
        }

        [HttpPost]
        [CheckLogin()]
        public JsonResult GetYLCPDetails(string cpid)
        {
            T_YLCP cp = T_YLCPDomain.GetInstance().GetCpDetailsById(Convert.ToInt32(cpid));
            if (cp != null)
            {
                string resultStr = JsonConvert.SerializeObject(new
                {
                    CPBH = cp.CPBH,
                    SCQYMC = (cp.T_SupQY1 != null && !string.IsNullOrEmpty(cp.T_SupQY1.SupMC)) ? cp.T_SupQY1.SupMC : "",
                    CPGG = cp.CPGG,
                    CPXH = cp.CPXH,
                    CPDW = cp.CPDW,
                    XKZH = (cp.T_SupQY1 != null && !string.IsNullOrEmpty(cp.T_SupQY1.SupXKZBH)) ? cp.T_SupQY1.SupXKZBH : "",
                    ZCZH = cp.CPZCZ,
                    SCQYID = cp.CPSCQYID,
                    CPPrice = cp.CPPrice,
                    SUPQYID = cp.CPGYSID,
                    SUPQYMC = (cp.T_SupQY != null && !string.IsNullOrEmpty(cp.T_SupQY.SupMC)) ? cp.T_SupQY.SupMC : "",
                    XSJG = cp.XSJG,
                    CPMC = cp.CPMC
                });
                return Json(resultStr);
            }
            else
            {
                return Json("");
            }
        }

        [CheckLogin()]
        public ActionResult CPAuditIndex(System.Int32 id, string tag)
        {
            T_YLCPModels model = new T_YLCPModels();
            CurUser = Session["UserModel"] as SysUser;
            //加载批发商企业列表
            T_SupQYModels supQymode = new T_SupQYModels();

            supQymode.DataModel = supQymode.DataModel ?? new T_SupQY();

            supQymode.DataList = T_SupQYDomain.GetInstance().GetAllT_SupQY(supQymode.DataModel);

            ViewData["SUPQY"] = new SelectList(supQymode.DataList, "SupID", "SupMC", "请选择");

            //加载产品类型列表
            T_CPLXModels cplxQymode = new T_CPLXModels();

            cplxQymode.DataModel = cplxQymode.DataModel ?? new T_CPLX();

            cplxQymode.DataList = T_CPLXDomain.GetInstance().GetAllT_CPLX(cplxQymode.DataModel);

            ViewData["CPLX"] = new SelectList(cplxQymode.DataList, "LXID", "LXMC", "请选择");

            model.DataModel = new T_YLCP();

            model.DataModel.CPLRRQ = DateTime.Now;
            model.DataModel.CPLRR = CurUser.UserAccount;
            if (id != 0)
            {
                model.DataModel = T_YLCPDomain.GetInstance().GetModelById(id);
                ViewData["CPTPLJ"] = model.DataModel.CPTP;
                ViewData["CPFJLJ"] = model.DataModel.CPFJ;
            }
            model.Tag = tag;
            return View("~/Views/T_YLCP/CPAuditIndex.cshtml", model);
        }

        [HttpPost]
        [CheckLogin()]
        public void through(T_YLCPModels model, string id)
        {
            int result = 0;
            try
            {
                Int32 cpid = model.DataModel.CPID;
                result = T_YLCPDomain.GetInstance().CpSh(cpid, id == "1" ? 1 : 2);
            }
            catch { }
            Response.ContentType = "text/json";
            if (result > 0)
                Response.Write("{\"statusCode\":\"200\", \"message\":\"操作成功\",\"callbackType\":\"closeCurrentReloadTab\",\"forwardUrl\":\"/T_YLCP/Index\"}");
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
                directory = Path.Combine(Server.MapPath("~/UploadFiles/"), "供货商产品图片");
            }
            else
            {
                directory = Path.Combine(Server.MapPath("~/UploadFiles/"), "供货商产品附件");
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
            //var rCode = GetRoleCode();
            //if (rCode != "1")
            //{
            var temp = T_YLCPDomain.GetInstance().GetModelById(id);
            if (temp != null && (temp.CPStatus == 1))
            {
                Response.Write("{\"statusCode\":\"300\", \"message\":\"已审批通过的数据不能删除！\"}");
                return;
            }
            //}
            Expression<Func<T_CGMX, bool>> whereCGD = p => (p.CPID == id);
            var lstCGMX = T_CGMXDomain.GetInstance().GetAllModels<int>(whereCGD);
            if (lstCGMX != null && lstCGMX.Count > 0)
            {
                Response.Write("{\"statusCode\":\"300\", \"message\":\"该产品已存在与采购单，不能删除！\"}");
                return;
            }
            T_YLCPModels model = new T_YLCPModels();
            model.DataModel = new T_YLCP();
            if (id != 0)
            {
                model.DataModel = T_YLCPDomain.GetInstance().GetModelById(id);
            }
            if (!string.IsNullOrEmpty(model.DataModel.CPTP))
            {
                string filePath = Path.Combine(Server.MapPath("~/UploadFiles/"), "供货商产品图片", model.DataModel.CPTP);
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
            }
            if (!string.IsNullOrEmpty(model.DataModel.CPFJ))
            {
                string filePath1 = Path.Combine(Server.MapPath("~/UploadFiles/"), "供货商产品附件", model.DataModel.CPFJ);
                if (System.IO.File.Exists(filePath1))
                {
                    System.IO.File.Delete(filePath1);
                }
            }
            int result = T_YLCPDomain.GetInstance().DeleteModelById(id);
            Response.ContentType = "text/json";
            if (result > 0)
                Response.Write("{\"statusCode\":\"200\", \"message\":\"操作成功\",\"callbackType\":\"forward\",\"forwardUrl\":\"/T_YLCP/Index\"}");
            else
                Response.Write("{\"statusCode\":\"300\", \"message\":\"操作失败\"}");
        }

        [HttpPost]
        public void GetCpByQy(string id)
        {
            string result1 = "";
            Dictionary<string, string> dict = new Dictionary<string, string>();
            try
            {
                StringBuilder result = new StringBuilder();
                result.Append("[[\"\",\"请选择\"]");
                //result.Append("[");
                if (string.IsNullOrEmpty(id))
                {
                    result.Append("]");
                    result1 = result.ToString();
                }
                T_YLCP ylcp = new T_YLCP();
                ylcp.CPGYSID = int.Parse(id);
                ylcp.CPStatus = 1;
                var list = T_YLCPDomain.GetInstance().GetAllT_YLCP(ylcp);
                foreach (var item in list)
                {
                    result.Append(",[");
                    result.Append("\"" + item.CPID + "\",");
                    result.Append("\"" + item.CPMC + "\"");
                    result.Append("]");
                }
                result.Append("]");
                result1 = result.ToString();
            }
            catch (Exception ex)
            {
            }
            Response.ContentType = "text/json";
            Response.Write(result1);
        }
        private string GetRoleCode()
        {
            return Session["RoleCode"] == null ? "" : Session["RoleCode"].ToString();
        }
    }
}
