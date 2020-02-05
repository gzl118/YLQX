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
    public class T_YSDController : Controller
    {
        public int resultCount = 0; // 总条数 

        [CheckLogin()]
        public ActionResult Index(T_YSDModels evalModel)
        {
            SysUser UserModel = Session["UserModel"] as SysUser;
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
            string strYSPerson = "请选择";
            int pagesize = Convert.ToInt32(evalModel.pageSize);
            int pagecount = Convert.ToInt32(evalModel.pagecount);
            int currentPage = Convert.ToInt32(evalModel.currentPage);
            evalModel.DataModel = evalModel.DataModel ?? new T_YSD();
            evalModel.DataModel.IsCGYS = 0;

            if (Request["strYSPerson"] != null)
            {
                strYSPerson = Request["strYSPerson"].ToString();
                if (!String.IsNullOrEmpty(strYSPerson))
                {
                    evalModel.DataModel.YSR = strYSPerson;
                }
            }

            var cpId = 0;
            if (Request["strYSCPMC"] != null)
            {
                string str = Request["strYSCPMC"].ToString();
                if (!String.IsNullOrEmpty(str))
                {
                    cpId = Convert.ToInt32(str);
                }
                ViewData["strYSCPMC"] = str;
            }
            var cusId = 0;
            if (Request["strCusQY"] != null)
            {
                string str = Request["strCusQY"].ToString();
                if (!String.IsNullOrEmpty(str))
                {
                    cusId = Convert.ToInt32(str);
                }
                ViewData["strCusQY"] = str;
            }

            //获取本企业下的人员列表
            T_Person person = new T_Person();
            person.PsQYID = (int)UserModel.UserCompanyID;
            ViewBag.Persons = new SelectList(T_PersonDomain.GetInstance().GetAllT_Person(person), "PsMZ", "PsMZ");
            ViewData["strYSPerson"] = strYSPerson;

            T_YLCPModels ylcpQymode = new T_YLCPModels();
            ylcpQymode.DataModel = ylcpQymode.DataModel ?? new T_YLCP();
            ylcpQymode.DataList = T_YLCPDomain.GetInstance().GetAllT_YLCP(ylcpQymode.DataModel).Where(p => p.CPStatus == 1).ToList();
            ViewData["YLCP"] = new SelectList(ylcpQymode.DataList, "CPID", "CPMC");

            T_SupQYModels supmode = new T_SupQYModels();
            supmode.DataModel = supmode.DataModel ?? new T_SupQY();
            supmode.DataList = T_SupQYDomain.GetInstance().GetAllT_SupQY(supmode.DataModel).Where(p => p.SupStatus == 1).ToList();
            ViewData["SupQYList"] = new SelectList(supmode.DataList, "SupID", "SupMC");

            evalModel.DataList = T_YSDDomain.GetInstance().PageT_YSD(evalModel.DataModel, evalModel.StartTime, evalModel.EndTime, currentPage, pagesize, cpId, cusId, out pagecount, out resultCount);
            evalModel.resultCount = resultCount;
            return View("~/Views/T_YSD/Index.cshtml", evalModel);
        }

        [CheckLogin()]
        public ActionResult Save(System.Int32 id, string tag)
        {
            SysUser sysUser = Session["UserModel"] as SysUser;
            //采购单列表
            T_CGDModels cgdQymode = new T_CGDModels();
            cgdQymode.DataModel = cgdQymode.DataModel ?? new T_CGD();
            cgdQymode.DataList = T_CGDDomain.GetInstance().GetAllT_CGD(cgdQymode.DataModel).Where(p => p.ISSH == 1 && p.IsFinish == 0).OrderByDescending(p => p.CGDH).ToList();
            ViewData["CGD"] = new SelectList(cgdQymode.DataList, "CGDH", "CGDH");

            //加载企业列表
            T_SupQYModels supmode = new T_SupQYModels();
            supmode.DataModel = supmode.DataModel ?? new T_SupQY();
            supmode.DataList = T_SupQYDomain.GetInstance().GetAllT_SupQY(supmode.DataModel).Where(p => p.SupStatus == 1).ToList();
            ViewData["SupID"] = new SelectList(supmode.DataList, "SupID", "SupMC");

            //获取本企业下的人员列表
            T_Person person = new T_Person();
            person.PsQYID = (int)sysUser.UserCompanyID;
            ViewBag.Persons = new SelectList(T_PersonDomain.GetInstance().GetAllT_Person(person), "PsMZ", "PsMZ");

            T_YSDModels model = new T_YSDModels();
            model.DataModel = new T_YSD();
            if (id != 0)
            {
                model.DataModel = T_YSDDomain.GetInstance().GetModelById(id);
                model.YSMXList = T_YSMXDomain.GetInstance().GetT_YSMXByYsid(id);
            }
            else
            {
                model.DataModel.YSDH = T_YSDDomain.GetInstance().GetYsOrderNum("YS", sysUser);
                model.DataModel.YSCJR = sysUser.UserAccount;
                model.DataModel.YSCJRQ = DateTime.Now;
            }


            model.Tag = tag;
            model.RoleCode = GetRoleCode();
            return View("~/Views/T_YSD/Save.cshtml", model);
        }

        [HttpPost]
        [CheckLogin()]
        public void Save(T_YSDModels model)
        {
            int result = 0;
            try
            {
                if (model.Tag == "Add")
                {
                    model.DataModel.IsFinish = 0;
                    model.DataModel.IsCGFinish = 0;
                    model.DataModel.IsCGYS = 0;
                    result = T_YSDDomain.GetInstance().AddModel(model.DataModel);
                }
                else if (model.Tag == "Edit")
                {
                    result = T_YSDDomain.GetInstance().UpdateModel(model.DataModel, model.DataModel.YSID);
                }
                if (model.DataModel.IsCGFinish == 1)
                {
                    T_CGDDomain.GetInstance().UpdateFinish(model.DataModel.CGDH);
                }
            }
            catch { }
            Response.ContentType = "text/json";
            if (result > 0)
                Response.Write("{\"statusCode\":\"200\", \"message\":\"操作成功\",\"callbackType\":\"closeCurrentReloadTab\",\"forwardUrl\":\"/T_YSD/Index\"}");
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
            string directory = Path.Combine(Server.MapPath("~/UploadFiles/"), "验收报告");

            //上传文件记录上传到数据库中
            //上传顺序：先数据库中增加记录成功，再上传文件
            var fileUpload = new UploadFile();
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            string path = directory + "\\" + filename;
            Filedata.SaveAs(path);
            return Json(new { status = "OK", filename = filename, path = "/UploadFiles/验收报告/" + filename });
        }

        [HttpPost]
        [CheckLogin()]
        public void Delete(System.Int32 id)
        {
            //var rCode = GetRoleCode();
            //var temp = T_YSDDomain.GetInstance().GetModelById(id);
            //if (temp != null)
            //{
            //    Response.Write("{\"statusCode\":\"300\", \"message\":\"该数据不能删除！\"}");
            //    return;
            //}
            var ysdModel = new T_YSD();
            if (id != 0)
            {
                ysdModel = T_YSDDomain.GetInstance().GetModelById(id);
                if (ysdModel != null)
                {
                    //如果验收单未被使用，管理员、超级管理员可删除。否则，任何人不能删除
                    Expression<Func<T_RKD, bool>> where = p => (p.YSDH == ysdModel.YSDH);
                    var lst = T_RKDDomain.GetInstance().GetAllModels<int>(where);
                    if (lst != null && lst.Count > 0)
                    {
                        Response.Write("{\"statusCode\":\"300\", \"message\":\"该验收单单已存在入库单，不能删除！\"}");
                        return;
                    }
                    if (!string.IsNullOrEmpty(ysdModel.YSBG))
                    {
                        string filePath = Path.Combine(Server.MapPath("~/UploadFiles/"), "购货商资料", ysdModel.YSBG);
                        if (System.IO.File.Exists(filePath))
                        {
                            System.IO.File.Delete(filePath);
                        }
                    }
                }
            }
            int result = T_YSDDomain.GetInstance().Delete(id);
            Response.ContentType = "text/json";
            if (result > 0)
                Response.Write("{\"statusCode\":\"200\", \"message\":\"操作成功\",\"callbackType\":\"forward\",\"forwardUrl\":\"/T_YSD/Index\"}");
            else
                Response.Write("{\"statusCode\":\"300\", \"message\":\"操作失败\"}");
        }

        #region 验收明细相关

        [CheckLogin()]
        public ActionResult YSMXTable(System.Int32 id, string ysdh, int canEdit)
        {
            T_YSDModels model = new T_YSDModels();
            if (id != 0)
            {
                model.YSMXList = T_YSMXDomain.GetInstance().GetT_YSMXByYsid(id);
            }
            else
            {
                model.YSMXList = T_YSMXDomain.GetInstance().GetT_YSMXByYsdh(ysdh);
            }
            ViewData["canEdit"] = canEdit;
            model.RoleCode = GetRoleCode();
            model.DataModel = new T_YSD();
            model.DataModel.YSID = id;
            return View("~/Views/T_YSD/YSMXTable.cshtml", model);
        }


        [HttpPost]
        [CheckLogin()]
        public void SaveYSMX(T_YSMXModels model)
        {
            int result = 0;
            string guid = string.Empty;
            try
            {
                if (model.Tag == "Add")
                {
                    model.DataModel.GUID = Guid.NewGuid().ToString("N");
                    guid = model.DataModel.GUID;
                    result = T_YSMXDomain.GetInstance().AddModelByYsdh(model.DataModel, model.YSDH);
                }
                else if (model.Tag == "Edit")
                {
                    result = T_YSMXDomain.GetInstance().UpdateModel(model.DataModel, model.DataModel.MXID);
                }
            }
            catch { }
            Response.ContentType = "text/json";
            if (result > 0)
            {
                string resultStr = JsonConvert.SerializeObject(new { statusCode = "200", message = "操作成功", guid = guid });
                Response.Write(resultStr);
            }
            else
                Response.Write("{\"statusCode\":\"300\", \"message\":\"操作失败\"}");
        }

        [HttpPost]
        [CheckLogin()]
        public void DeleteYSMX(string Guid)
        {
            int result = T_YSMXDomain.GetInstance().DeleteModelByGuid(Guid);
            Response.ContentType = "text/json";
            if (result > 0)
                Response.Write("{\"statusCode\":\"200\", \"message\":\"操作成功\"}");
            else
                Response.Write("{\"statusCode\":\"300\", \"message\":\"操作失败\"}");
        }

        #endregion
        private string GetRoleCode()
        {
            return Session["RoleCode"] == null ? "" : Session["RoleCode"].ToString();
        }
        /// <summary>
        /// 通过验收明细ID获取产品信息
        /// </summary>
        /// <param name="mxid"></param>
        /// <returns></returns>
        [HttpPost]
        [CheckLogin()]
        public JsonResult GetYLCPDetailsByMXID(int mxid)
        {
            var mxModel = T_YSMXDomain.GetInstance().GetModelById(mxid);
            if (mxModel != null)
            {
                if (mxModel.CPID != null)
                {
                    T_YLCP cp = T_YLCPDomain.GetInstance().GetCpDetailsById((int)mxModel.CPID);
                    if (cp != null)
                    {
                        string resultStr = JsonConvert.SerializeObject(new
                        {
                            CPID = cp.CPID,
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
                            CPMC = cp.CPMC,
                            CPNUM = mxModel.CPNUM,
                            CPPH = mxModel.CPPH,
                            SCRQ = mxModel.CPSCRQ == null ? "" : mxModel.CPSCRQ.Value.ToString("yyyy/MM/dd"),
                            YQX = mxModel.CPYXQ == null ? "" : mxModel.CPYXQ.Value.ToString("yyyy/MM/dd"),
                            CCTJ = cp.CCTJ
                        });
                        return Json(resultStr);
                    }
                }
            }
            return Json("");
        }

        [CheckLogin()]
        public ActionResult NCGIndex(T_YSDModels evalModel)
        {
            SysUser UserModel = Session["UserModel"] as SysUser;
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
            string strYSPerson = "请选择";
            int pagesize = Convert.ToInt32(evalModel.pageSize);
            int pagecount = Convert.ToInt32(evalModel.pagecount);
            int currentPage = Convert.ToInt32(evalModel.currentPage);
            evalModel.DataModel = evalModel.DataModel ?? new T_YSD();
            evalModel.DataModel.IsCGYS = 1;

            if (Request["strYSPerson"] != null)
            {
                strYSPerson = Request["strYSPerson"].ToString();
                if (!String.IsNullOrEmpty(strYSPerson))
                {
                    evalModel.DataModel.YSR = strYSPerson;
                }
            }

            var cpId = 0;
            if (Request["strYSCPMC"] != null)
            {
                string str = Request["strYSCPMC"].ToString();
                if (!String.IsNullOrEmpty(str))
                {
                    cpId = Convert.ToInt32(str);
                }
                ViewData["strYSCPMC"] = str;
            }
            var cusId = 0;
            if (Request["strCusQY"] != null)
            {
                string str = Request["strCusQY"].ToString();
                if (!String.IsNullOrEmpty(str))
                {
                    cusId = Convert.ToInt32(str);
                }
                ViewData["strCusQY"] = str;
            }

            //获取本企业下的人员列表
            T_Person person = new T_Person();
            person.PsQYID = (int)UserModel.UserCompanyID;
            ViewBag.Persons = new SelectList(T_PersonDomain.GetInstance().GetAllT_Person(person), "PsMZ", "PsMZ");
            ViewData["strYSPerson"] = strYSPerson;

            T_YLCPModels ylcpQymode = new T_YLCPModels();
            ylcpQymode.DataModel = ylcpQymode.DataModel ?? new T_YLCP();
            ylcpQymode.DataList = T_YLCPDomain.GetInstance().GetAllT_YLCP(ylcpQymode.DataModel).Where(p => p.CPStatus == 1).ToList();
            ViewData["YLCP"] = new SelectList(ylcpQymode.DataList, "CPID", "CPMC");

            T_SupQYModels supmode = new T_SupQYModels();
            supmode.DataModel = supmode.DataModel ?? new T_SupQY();
            supmode.DataList = T_SupQYDomain.GetInstance().GetAllT_SupQY(supmode.DataModel).Where(p => p.SupStatus == 1).ToList();
            ViewData["SupQYList"] = new SelectList(supmode.DataList, "SupID", "SupMC");

            evalModel.DataList = T_YSDDomain.GetInstance().PageT_YSD(evalModel.DataModel, evalModel.StartTime, evalModel.EndTime, currentPage, pagesize, cpId, cusId, out pagecount, out resultCount);
            evalModel.resultCount = resultCount;
            return View("~/Views/T_YSD/NCGIndex.cshtml", evalModel);
        }
        [CheckLogin()]
        public ActionResult NCGSave(System.Int32 id, string tag)
        {
            SysUser sysUser = Session["UserModel"] as SysUser;

            //加载企业列表
            T_SupQYModels supmode = new T_SupQYModels();
            supmode.DataModel = supmode.DataModel ?? new T_SupQY();
            supmode.DataList = T_SupQYDomain.GetInstance().GetAllT_SupQY(supmode.DataModel).Where(p => p.SupStatus == 1).ToList();
            ViewData["SupID"] = new SelectList(supmode.DataList, "SupID", "SupMC");

            //获取本企业下的人员列表
            T_Person person = new T_Person();
            person.PsQYID = (int)sysUser.UserCompanyID;
            ViewBag.Persons = new SelectList(T_PersonDomain.GetInstance().GetAllT_Person(person), "PsMZ", "PsMZ");

            T_YSDModels model = new T_YSDModels();
            model.DataModel = new T_YSD();
            if (id != 0)
            {
                model.DataModel = T_YSDDomain.GetInstance().GetModelById(id);
                model.YSMXList = T_YSMXDomain.GetInstance().GetT_YSMXByYsid(id);
            }
            else
            {
                model.DataModel.YSDH = T_YSDDomain.GetInstance().GetYsOrderNum("YS", sysUser);
                model.DataModel.YSCJR = sysUser.UserAccount;
                model.DataModel.YSCJRQ = DateTime.Now;
            }


            model.Tag = tag;
            model.RoleCode = GetRoleCode();
            return View("~/Views/T_YSD/NCGSave.cshtml", model);
        }
        [HttpPost]
        [CheckLogin()]
        public void NCGSave(T_YSDModels model)
        {
            int result = 0;
            try
            {
                if (model.Tag == "Add")
                {
                    model.DataModel.IsFinish = 0;
                    model.DataModel.IsCGFinish = 0;
                    model.DataModel.IsCGYS = 1;
                    result = T_YSDDomain.GetInstance().AddModel(model.DataModel);
                }
                else if (model.Tag == "Edit")
                {
                    result = T_YSDDomain.GetInstance().UpdateModel(model.DataModel, model.DataModel.YSID);
                }
                if (model.DataModel.IsCGFinish == 1)
                {
                    T_CGDDomain.GetInstance().UpdateFinish(model.DataModel.CGDH);
                }
            }
            catch { }
            Response.ContentType = "text/json";
            if (result > 0)
                Response.Write("{\"statusCode\":\"200\", \"message\":\"操作成功\",\"callbackType\":\"closeCurrentReloadTab\",\"forwardUrl\":\"/T_YSD/Index\"}");
            else
                Response.Write("{\"statusCode\":\"300\", \"message\":\"操作失败\"}");
        }
        [HttpPost]
        [CheckLogin()]
        public void NCGDelete(System.Int32 id)
        {
            var ysdModel = new T_YSD();
            if (id != 0)
            {
                ysdModel = T_YSDDomain.GetInstance().GetModelById(id);
                if (ysdModel != null)
                {
                    //如果验收单未被使用，管理员、超级管理员可删除。否则，任何人不能删除
                    Expression<Func<T_RKD, bool>> where = p => (p.YSDH == ysdModel.YSDH);
                    var lst = T_RKDDomain.GetInstance().GetAllModels<int>(where);
                    if (lst != null && lst.Count > 0)
                    {
                        Response.Write("{\"statusCode\":\"300\", \"message\":\"该验收单单已存在入库单，不能删除！\"}");
                        return;
                    }
                    if (!string.IsNullOrEmpty(ysdModel.YSBG))
                    {
                        string filePath = Path.Combine(Server.MapPath("~/UploadFiles/"), "购货商资料", ysdModel.YSBG);
                        if (System.IO.File.Exists(filePath))
                        {
                            System.IO.File.Delete(filePath);
                        }
                    }
                }
            }
            int result = T_YSDDomain.GetInstance().Delete(id);
            Response.ContentType = "text/json";
            if (result > 0)
                Response.Write("{\"statusCode\":\"200\", \"message\":\"操作成功\",\"callbackType\":\"forward\",\"forwardUrl\":\"/T_YSD/NCGIndex\"}");
            else
                Response.Write("{\"statusCode\":\"300\", \"message\":\"操作失败\"}");
        }

        [HttpPost]
        [CheckLogin()]
        public JsonResult GetYSDH(string hflag, int? cpId, int? supId, string cpph)
        {
            var mxModel = T_YSDDomain.GetInstance().GetYSDByCondition(hflag, cpId, supId, cpph);
            var result1 = "";
            StringBuilder result = new StringBuilder();
            result.Append("[[\"\",\"请选择\"]");
            if (mxModel == null)
            {
                result.Append("]");
                result1 = result.ToString();
            }
            else
            {
                foreach (var item in mxModel)
                {
                    result.Append(",[");
                    result.Append("\"" + item.YSID + "\",");
                    result.Append("\"" + item.YSDH + "\"");
                    result.Append("]");
                }
                result.Append("]");
                result1 = result.ToString();
            }
            return Json(result1);
        }
    }
}
