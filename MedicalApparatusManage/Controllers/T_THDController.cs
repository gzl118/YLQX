using MedicalApparatusManage.Common;
using MedicalApparatusManage.Domain;
using MedicalApparatusManage.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;

namespace MedicalApparatusManage.Controllers
{
    public class T_THDController : Controller
    {
        public int resultCount = 0; // 总条数 

        [CheckLogin()]
        public ActionResult Index(T_THDModels evalModel)
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
            evalModel.DataModel = evalModel.DataModel ?? new T_THD();

            if (Request["strTHMC"] != null)
            {
                string str = Request["strTHMC"].ToString();
                if (!String.IsNullOrEmpty(str))
                {
                    evalModel.DataModel.THMC = str;
                }
            }

            if (Request["strTHSQPerson"] != null)
            {
                string str = Request["strTHSQPerson"].ToString();
                if (!String.IsNullOrEmpty(str))
                {
                    evalModel.DataModel.SQR = str;
                }
                ViewData["strTHSQPerson"] = str;
            }
            //获取本企业下的人员列表
            SysUser UserModel = Session["UserModel"] as SysUser;
            T_Person person = new T_Person();
            person.PsQYID = (int)UserModel.UserCompanyID;
            ViewBag.Persons = new SelectList(T_PersonDomain.GetInstance().GetAllT_Person(person), "PsMZ", "PsMZ");

            evalModel.DataList = T_THDDomain.GetInstance().PageT_THD(evalModel.DataModel, evalModel.StartTime, evalModel.EndTime, currentPage, pagesize, out pagecount, out resultCount).ToList();
            evalModel.resultCount = resultCount;
            return View("~/Views/T_THD/Index.cshtml", evalModel);
        }

        [CheckLogin()]
        public ActionResult Save(System.Int32 id, string tag)
        {
            //加载企业列表
            T_SupQYModels supmode = new T_SupQYModels();
            supmode.DataModel = supmode.DataModel ?? new T_SupQY();
            supmode.DataList = T_SupQYDomain.GetInstance().GetAllT_SupQY(supmode.DataModel).Where(p => p.SupStatus == 1).ToList();
            ViewData["SupID"] = new SelectList(supmode.DataList, "SupID", "SupMC");

            Expression<Func<T_YSD, bool>> where = PredicateBuilder.True<T_YSD>();
            where = where.And(p => p.IsTHFinish == 0);
            var yslist = T_YSDDomain.GetInstance().GetAllModels<int>(where);
            ViewData["YSD"] = new SelectList(yslist, "YSID", "YSDH");

            T_THDModels model = new T_THDModels();
            model.DataModel = new T_THD();
            var CurUser = Session["UserModel"] as SysUser;
            //获取本企业下的人员列表
            T_Person person = new T_Person();
            if (CurUser.UserCompanyID != null)
                person.PsQYID = (int)CurUser.UserCompanyID;
            ViewBag.Persons = new SelectList(T_PersonDomain.GetInstance().GetAllT_Person(person), "PsMZ", "PsMZ");

            //加载仓库列表
            T_CKModels ckmode = new T_CKModels();
            ckmode.DataModel = ckmode.DataModel ?? new T_CK();
            ckmode.DataList = T_CKDomain.GetInstance().GetAllT_CK(ckmode.DataModel);
            ViewData["CK"] = new SelectList(ckmode.DataList, "CKID", "CKMC");

            if (id != 0)
            {
                model.DataModel = T_THDDomain.GetInstance().GetModelById(id);
                model.THMXList = T_THMXDomain.GetInstance().GetT_THMXByYsid(id);
                if (model.DataModel.YSID != null && model.DataModel.YSID != 0)
                {
                    var temp = T_YSDDomain.GetInstance().GetModelById(model.DataModel.YSID);
                    if (temp != null)
                        model.YSDH = temp.YSDH;
                }
            }
            else
            {
                model.DataModel.THDH = T_THDDomain.GetInstance().GetTHOrderNum("TH", CurUser);
                model.DataModel.THCJR = CurUser.UserAccount;
                model.DataModel.THCJRQ = DateTime.Now;
            }
            model.Tag = tag;
            model.RoleCode = GetRoleCode();
            return View("~/Views/T_THD/Save.cshtml", model);
        }

        [HttpPost]
        [CheckLogin()]
        public void Save(T_THDModels model)
        {
            int result = 0;
            try
            {
                if (model.Tag == "Add")
                {
                    model.DataModel.FLAG = 1;
                    model.DataModel.ISSH = 0;
                    result = T_THDDomain.GetInstance().AddModel(model.DataModel);
                }
                else if (model.Tag == "Edit")
                {
                    model.DataModel.FLAG = 1;
                    result = T_THDDomain.GetInstance().UpdateModel(model.DataModel, model.DataModel.THID);
                }

            }
            catch { }
            Response.ContentType = "text/json";
            if (result > 0)
                Response.Write("{\"statusCode\":\"200\", \"message\":\"操作成功\",\"callbackType\":\"closeCurrentReloadTab\",\"forwardUrl\":\"/T_THD/Index\"}");
            else
                Response.Write("{\"statusCode\":\"300\", \"message\":\"操作失败\"}");
        }

        [HttpPost]
        [CheckLogin()]
        public void XSSave(T_THDModels model)
        {
            int result = 0;
            try
            {
                if (model.Tag == "Add")
                {
                    model.DataModel.FLAG = 2;
                    result = T_THDDomain.GetInstance().AddModel(model.DataModel);
                }
                else if (model.Tag == "Edit")
                {
                    model.DataModel.FLAG = 2;
                    result = T_THDDomain.GetInstance().UpdateModel(model.DataModel, model.DataModel.THID);
                }

            }
            catch { }
            Response.ContentType = "text/json";
            if (result > 0)
                Response.Write("{\"statusCode\":\"200\", \"message\":\"操作成功\",\"callbackType\":\"closeCurrentReloadTab\",\"forwardUrl\":\"/T_THD/Index\"}");
            else
                Response.Write("{\"statusCode\":\"300\", \"message\":\"操作失败\"}");
        }

        [HttpPost]
        [CheckLogin()]
        public void Delete(System.Int32 id)
        {
            var temp = T_THDDomain.GetInstance().GetModelById(id);
            if (temp != null)
            {
                if (temp.ISSH == 1)
                {
                    Response.Write("{\"statusCode\":\"300\", \"message\":\"已审批通过的数据不能删除！\"}");
                    return;
                }
            }
            int result = T_THDDomain.GetInstance().Delete(id);
            Response.ContentType = "text/json";
            if (result > 0)
                Response.Write("{\"statusCode\":\"200\", \"message\":\"操作成功\",\"callbackType\":\"forward\",\"forwardUrl\":\"/T_THD/Index\"}");
            else
                Response.Write("{\"statusCode\":\"300\", \"message\":\"操作失败\"}");
        }
        [HttpPost]
        [CheckLogin()]
        public void through(T_THDModels model, int id)
        {
            int result = 0;
            try
            {
                Int32 cgid = model.DataModel.THID;
                result = T_THDDomain.GetInstance().Sh(cgid, id);
                if (id == 1 && model.DataModel.IsFinish == 1 && model.DataModel.YSID != 0)
                {
                    T_YSDDomain.GetInstance().UpdateFinish((int)model.DataModel.YSID);
                }
            }
            catch { }
            Response.ContentType = "text/json";
            if (result > 0)
                Response.Write("{\"statusCode\":\"200\", \"message\":\"操作成功\",\"callbackType\":\"closeCurrentReloadTab\",\"forwardUrl\":\"/T_THD/Index\"}");
            else
                Response.Write("{\"statusCode\":\"300\", \"message\":\"操作失败\"}");
        }
        private string GetRoleCode()
        {
            return Session["RoleCode"] == null ? "" : Session["RoleCode"].ToString();
        }
        [CheckLogin()]
        public ActionResult THMXTable(System.Int32 id, string thdh, int canEdit, int isSH)
        {
            T_THDModels model = new T_THDModels();
            if (id != 0)
            {
                model.THMXList = T_THMXDomain.GetInstance().GetT_THMXBythid(id);
            }
            else
            {
                model.THMXList = T_THMXDomain.GetInstance().GetT_THMXBythdh(thdh);
            }
            ViewData["canEdit"] = canEdit;
            model.RoleCode = GetRoleCode();
            model.DataModel = new T_THD();
            model.DataModel.ISSH = isSH;
            //model.DataModel.THID = id;
            return View("~/Views/T_THD/THMXTable.cshtml", model);
        }
    }
}
