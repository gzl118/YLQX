using MedicalApparatusManage.Domain;
using MedicalApparatusManage.Models;
using System;
using System.Collections.Generic;
using System.Linq;
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

            //获取本企业下的人员列表
            T_Person person = new T_Person();
            person.PsQYID = (int)UserModel.UserCompanyID;
            ViewBag.Persons = new SelectList(T_PersonDomain.GetInstance().GetAllT_Person(person), "PsID", "PsMZ");
            ViewData["strCGPerson"] = strCGPerson;

            evalModel.DataList = T_CGDDomain.GetInstance().PageT_CGD(evalModel.DataModel, evalModel.StartTime, evalModel.EndTime, currentPage, pagesize, out pagecount, out resultCount);
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
            person.PsQYID = (int)UserModel.UserCompanyID;
            ViewBag.Persons = new SelectList(T_PersonDomain.GetInstance().GetAllT_Person(person), "PsMZ", "PsMZ");
            string roleId = Session["RoleId"].ToString();
            ViewData["IsLeader"] = false;
            if (roleId == "1" || roleId == "2")
            {
                ViewData["IsLeader"] = true;
            }
            model.Tag = tag;
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
                    result = T_CGDDomain.GetInstance().AddModel(model.DataModel);
                }
                else if (model.Tag == "Edit")
                {
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
        public void through(T_CGDModels model, string id)
        {
            int result = 0;
            try
            {
                Int32 cgid = model.DataModel.CGID;
                result = T_CGDDomain.GetInstance().Sh(cgid, id == "1" ? 1 : 2);
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
            int result = T_CGDDomain.GetInstance().DeleteModelById(id);
            Response.ContentType = "text/json";
            if (result > 0)
                Response.Write("{\"statusCode\":\"200\", \"message\":\"操作成功\",\"callbackType\":\"forward\",\"forwardUrl\":\"/T_CGD/Index\"}");
            else
                Response.Write("{\"statusCode\":\"300\", \"message\":\"操作失败\"}");
        }
    }
}
