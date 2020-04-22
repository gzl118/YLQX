using MedicalApparatusManage.Common;
using MedicalApparatusManage.Domain;
using MedicalApparatusManage.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace MedicalApparatusManage.Controllers
{
    public class T_CGDController : Controller
    {
        public int resultCount = 0; // 总条数 
        public SysUser CurUser = null;

        [CheckLogin()]
        public ActionResult Index(T_CGDModels evalModel)
        {
            SysUser UserModel = Session["UserModel"] as SysUser;
            try
            {

                ViewData["shUserId"] = UserModel.UserId;
                evalModel.currentPage = int.Parse(Request["pageNum"].ToString());
            }
            catch { }

            string strCGPerson = "请选择";


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
            evalModel.DataModel = evalModel.DataModel ?? new T_CGD();

            if (Request["strCGPerson"] != null)
            {
                strCGPerson = Request["strCGPerson"].ToString();
                if (!String.IsNullOrEmpty(strCGPerson))
                {
                    evalModel.DataModel.CGPERSON = strCGPerson;
                }
            }
            var strCGDMC = "";
            if (Request["strCGDMC"] != null)
            {
                strCGDMC = Request["strCGDMC"].ToString();
                if (!String.IsNullOrEmpty(strCGDMC))
                {
                    evalModel.DataModel.CGDMC = strCGDMC;
                }
            }
            var cpId = 0;
            if (Request["strCGCPMC"] != null)
            {
                string str = Request["strCGCPMC"].ToString();
                if (!String.IsNullOrEmpty(str))
                {
                    cpId = Convert.ToInt32(str);
                }
                ViewData["strCGCPMC"] = str;
            }
            var supId = 0;
            if (Request["strCGSupQY"] != null)
            {
                string str = Request["strCGSupQY"].ToString();
                if (!String.IsNullOrEmpty(str))
                {
                    supId = Convert.ToInt32(str);
                }
                ViewData["strCGSupQY"] = str;
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
            ViewData["strCGPerson"] = strCGPerson;

            T_YLCPModels ylcpQymode = new T_YLCPModels();
            ylcpQymode.DataModel = ylcpQymode.DataModel ?? new T_YLCP();
            ylcpQymode.DataList = T_YLCPDomain.GetInstance().GetAllT_YLCP(ylcpQymode.DataModel).Where(p => p.CPStatus == 1).ToList();
            ViewData["YLCP"] = new SelectList(ylcpQymode.DataList, "CPID", "CPMC");

            T_SupQYModels supmode = new T_SupQYModels();
            supmode.DataModel = supmode.DataModel ?? new T_SupQY();
            supmode.DataList = T_SupQYDomain.GetInstance().GetAllT_SupQY(supmode.DataModel).Where(p => p.SupStatus == 1).ToList();
            ViewData["SupQYList"] = new SelectList(supmode.DataList, "SupID", "SupMC");

            evalModel.DataList = T_CGDDomain.GetInstance().PageT_CGD(evalModel.DataModel, evalModel.StartTime, evalModel.EndTime, currentPage, pagesize, cpId, supId, cusId, out pagecount, out resultCount);
            evalModel.resultCount = resultCount;
            return View("~/Views/T_CGD/Index.cshtml", evalModel);
        }

        [CheckLogin()]
        public ActionResult Save(System.Int32 id, string tag)
        {
            T_CGDModels model = new T_CGDModels();
            model.DataModel = new T_CGD();
            CurUser = Session["UserModel"] as SysUser;
            //id是否为0，区分增加和修改功能
            if (id != 0)
            {
                model.CGMXList = T_CGMXDomain.GetInstance().GetT_CGMXByCgid(id);
                model.DataModel = T_CGDDomain.GetInstance().GetModelById(id);
            }
            else
            {
                model.DataModel.CGDH = T_CGDDomain.GetInstance().GetCgOrderNum("CP", CurUser);
                model.DataModel.CGCJR = CurUser.UserAccount;
                model.DataModel.CGCJRQ = DateTime.Now;
            }

            //加载产品列表
            //T_YLCPModels ylcpmode = new T_YLCPModels();

            //ylcpmode.DataModel = ylcpmode.DataModel ?? new T_YLCP();

            //ylcpmode.DataList = T_YLCPDomain.GetInstance().GetAllT_YLCP(ylcpmode.DataModel);

            //ViewData["YLCP"] = new SelectList(ylcpmode.DataList, "CPID", "CPMC");

            //加载企业列表
            T_SupQYModels supmode = new T_SupQYModels();

            supmode.DataModel = supmode.DataModel ?? new T_SupQY();

            supmode.DataList = T_SupQYDomain.GetInstance().GetAllT_SupQY(supmode.DataModel).Where(p => p.SupStatus == 1).ToList();

            ViewData["SupID"] = new SelectList(supmode.DataList, "SupID", "SupMC");

            //获取用户信息（包含单位ID）
            SysUser UserModel = Session["UserModel"] as SysUser;
            //获取本企业下的人员列表
            T_Person person = new T_Person();
            if (UserModel.UserCompanyID != null)
                person.PsQYID = (int)UserModel.UserCompanyID;
            ViewBag.Persons = new SelectList(T_PersonDomain.GetInstance().GetAllT_Person(person), "PsMZ", "PsMZ");
            //Expression<Func<T_PackingUnit, bool>> where = PredicateBuilder.True<T_PackingUnit>();
            //ViewBag.PackingUnit = new SelectList(T_PackingUnitDomain.GetInstance().GetAllModels<int>(where), "PUName", "PUName");
            model.Tag = tag;
            model.RoleCode = GetRoleCode();
            return View("~/Views/T_CGD/Save.cshtml", model);
        }

        [HttpPost]
        [CheckLogin()]
        public void Save(T_CGDModels model)
        {
            int result = 0;
            try
            {
                if (model.Tag == "Add")
                {
                    model.DataModel.ISHG = 0;
                    model.DataModel.ISCG = 0;
                    model.DataModel.ISSH = 0;
                    model.DataModel.IsFinish = 0;
                    var temp = T_CGDDomain.GetInstance().GetAllModels<string>(p => p.CGDH == model.DataModel.CGDH).FirstOrDefault();
                    if (temp != null && temp.CGID != 0)
                    {
                        var CurUser1 = Session["UserModel"] as SysUser;
                        model.DataModel.CGDH = T_CGDDomain.GetInstance().GetCgOrderNum("CP", CurUser1);
                    }
                    result = T_CGDDomain.GetInstance().AddModel(model.DataModel);
                }
                else if (model.Tag == "Edit")
                {
                    model.DataModel.ISSH = 0;
                    result = T_CGDDomain.GetInstance().UpdateModel(model.DataModel, model.DataModel.CGID);
                }
            }
            catch { }
            Response.ContentType = "text/json";
            if (result > 0)
                Response.Write("{\"statusCode\":\"200\", \"message\":\"操作成功\",\"callbackType\":\"closeCurrentReloadTab\",\"forwardUrl\":\"/T_CGD/Index\"}");
            else
                Response.Write("{\"statusCode\":\"300\", \"message\":\"操作失败\"}");
        }

        [CheckLogin()]
        public ActionResult CGDSPIndex(System.Int32 id, string tag)
        {
            T_CGDModels model = new T_CGDModels();
            model.DataModel = new T_CGD();
            T_CGMX cgmodel = new T_CGMX();
            if (id != 0)
            {
                model.DataModel = T_CGDDomain.GetInstance().GetModelById(id);
                model.CGMXList = T_CGMXDomain.GetInstance().GetAllT_CGMX(cgmodel);
                if (model.CGMXList.Count > 0)
                {
                    model.CGMXList = model.CGMXList.Where(p => p.CGID == id).ToList();
                }
            }
            model.Tag = tag;
            return View("~/Views/T_CGD/CGDSPIndex.cshtml", model);
        }

        [HttpPost]
        [CheckLogin()]
        public void through(T_CGDModels model, int id)
        {
            int result = 0;
            try
            {
                Int32 cgid = model.DataModel.CGID;
                result = T_CGDDomain.GetInstance().Sh(cgid, id);
            }
            catch { }
            Response.ContentType = "text/json";
            if (result > 0)
                Response.Write("{\"statusCode\":\"200\", \"message\":\"操作成功\",\"callbackType\":\"closeCurrentReloadTab\",\"forwardUrl\":\"/T_CGD/Index\"}");
            else
                Response.Write("{\"statusCode\":\"300\", \"message\":\"操作失败\"}");
        }

        [HttpPost]
        [CheckLogin()]
        public void Delete(System.Int32 id)
        {
            //var rCode = GetRoleCode();
            var temp = T_CGDDomain.GetInstance().GetModelById(id);
            if (temp != null)
            {
                if (temp.ISSH == 1)
                {
                    Response.Write("{\"statusCode\":\"300\", \"message\":\"已审批通过的数据不能删除！\"}");
                    return;
                }
                //如果采购单未被使用，超级管理员可删除。否则，任何人不能删除
                Expression<Func<T_YSD, bool>> where = p => (p.CGDH == temp.CGDH);
                var lst = T_YSDDomain.GetInstance().GetAllModels<int>(where);
                if (lst != null && lst.Count > 0)
                {
                    Response.Write("{\"statusCode\":\"300\", \"message\":\"该采购单已存在验收单，不能删除！\"}");
                    return;
                }
            }
            int result = T_CGDDomain.GetInstance().Delete(id);
            Response.ContentType = "text/json";
            if (result > 0)
                Response.Write("{\"statusCode\":\"200\", \"message\":\"操作成功\",\"callbackType\":\"forward\",\"forwardUrl\":\"/T_CGD/Index\"}");
            else
                Response.Write("{\"statusCode\":\"300\", \"message\":\"操作失败\"}");
        }
        private string GetRoleCode()
        {
            return Session["RoleCode"] == null ? "" : Session["RoleCode"].ToString();
        }
        [HttpPost]
        [CheckLogin()]
        public int SaveTPrice(int id, string tPrice, string dhid)
        {
            int result = 0;
            try
            {
                double d = 0;
                double.TryParse(tPrice, out d);
                result = T_CGDDomain.GetInstance().SaveTPrice(id, d, dhid);
            }
            catch { }
            return result;
        }

        /// <summary>
        /// 通过明细ID获取产品信息
        /// </summary>
        /// <param name="mxid"></param>
        /// <returns></returns>
        [HttpPost]
        [CheckLogin()]
        public JsonResult GetYLCPDetailsByMXID(int mxid)
        {
            var mxModel = T_CGMXDomain.GetInstance().GetModelById(mxid);
            if (mxModel != null)
            {
                T_YLCP cp = T_YLCPDomain.GetInstance().GetCpDetailsById(mxModel.CPID);
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
                        CPNUM = mxModel.CPNUM
                    });
                    return Json(resultStr);
                }
            }
            return Json("");
        }
        [HttpPost]
        [CheckLogin()]
        public JsonResult GetCGDH(string CGDMC)
        {
            var mxModel = T_CGDDomain.GetInstance().GetAllModels<int>(p => p.ISSH == 1 && p.IsFinish != 1 && (string.IsNullOrEmpty(CGDMC) || p.CGDMC.Contains(CGDMC))).OrderByDescending(p => p.CGDH).ToList();
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
                    result.Append("\"" + item.CGDH + "\",");
                    result.Append("\"" + item.CGDH + "\"");
                    result.Append("]");
                }
                result.Append("]");
                result1 = result.ToString();
            }
            return Json(result1);
        }
    }
}
