using MedicalApparatusManage.Domain;
using MedicalApparatusManage.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MedicalApparatusManage.Controllers
{
    public class T_XSDController : Controller
    {
        public int resultCount = 0; // 总条数 
        public SysUser CurUser = null;

        [CheckLogin()]
        public ActionResult Index(T_XSDModels evalModel)
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
            evalModel.DataModel = evalModel.DataModel ?? new T_XSD();
            string strCUSQY = "请选择";
            if (Request["strXSDMC"] != null)
            {
                string str = Request["strXSDMC"].ToString();
                if(!String.IsNullOrEmpty(str))
                {
                    evalModel.DataModel.XSDH = str;
                }
            }
            if (Request["strCUSQY"] != null)
            {
                strCUSQY = Request["strCUSQY"].ToString();
                if (!String.IsNullOrEmpty(strCUSQY))
                {
                    evalModel.DataModel.KHID = Convert.ToInt16(strCUSQY);
                }
            }
            //购货企业列表
            T_CusQY cusqy = new T_CusQY();
            ViewBag.CUSQY = new SelectList(T_CusQYDomain.GetInstance().GetAllT_CusQY(cusqy), "CusID", "CusMC");
            ViewData["strCUSQY"] = strCUSQY;
            evalModel.DataList = T_XSDDomain.GetInstance().PageT_XSD(evalModel.DataModel, evalModel.StartTime, evalModel.EndTime, currentPage, pagesize, out pagecount, out resultCount);
            evalModel.resultCount = resultCount;
            return View("~/Views/T_XSD/Index.cshtml", evalModel);
        }

        [CheckLogin()]
        public ActionResult Save(System.Int32 id, string tag)
        {
            //加载购买商商企业列表
            T_CusQYModels cusQymode = new T_CusQYModels();

            cusQymode.DataModel = cusQymode.DataModel ?? new T_CusQY();

            cusQymode.DataList = T_CusQYDomain.GetInstance().GetAllT_CusQY(cusQymode.DataModel).Where(p => p.CusStatus == Convert.ToInt32("1")).ToList();

            ViewData["CusQY"] = new SelectList(cusQymode.DataList, "CusID", "CusMC");

            //加载销售人员列表
            T_PersonModels perQymode = new T_PersonModels();

            perQymode.DataModel = perQymode.DataModel ?? new T_Person();

            perQymode.DataList = T_PersonDomain.GetInstance().GetAllT_Person(perQymode.DataModel);

            ViewData["Person"] = new SelectList(perQymode.DataList, "PsID", "PsMZ");

            //加载销售合同列表
            //T_XSHTModels xshtQymode = new T_XSHTModels();

            //xshtQymode.DataModel = xshtQymode.DataModel ?? new T_XSHT();

            //xshtQymode.DataList = T_XSHTDomain.GetInstance().GetAllT_XSHT(xshtQymode.DataModel);

            //ViewData["XSHT"] = new SelectList(xshtQymode.DataList, "HTID", "HTMC");

            //加载产品列表
            T_YLCPModels ylcpmode = new T_YLCPModels();

            ylcpmode.DataModel = ylcpmode.DataModel ?? new T_YLCP();

            ylcpmode.DataList = T_YLCPDomain.GetInstance().GetAllT_YLCP(ylcpmode.DataModel).Where(p => p.CPStatus == Convert.ToInt32("1")).ToList();

            ViewData["YLCP"] = new SelectList(ylcpmode.DataList, "CPID", "CPMC");

            T_XSDModels model = new T_XSDModels();
            model.DataModel = new T_XSD();
            CurUser = Session["UserModel"] as SysUser;
            if (id != 0)
            {
                model.DataModel = T_XSDDomain.GetInstance().GetModelById(id);
                model.XSMXList = T_XSMXDomain.GetInstance().GetT_XSMXByXsid(id);
            }
            else
            {
                model.DataModel.XSDH = T_XSDDomain.GetInstance().GetXsOrderNum(CurUser);
                model.DataModel.XSCJR = CurUser.UserAccount;
                model.DataModel.XSCJRQ = DateTime.Now;
            }
            string roleId = Session["RoleId"].ToString();
            ViewData["IsLeader"] = false;
            if (roleId == "1" || roleId == "2")
            {
                ViewData["IsLeader"] = true;
            }
            model.Tag = tag;
            return View("~/Views/T_XSD/Save.cshtml", model);
        }

        [HttpPost]
        [CheckLogin()]
        public void Save(T_XSDModels model)
        {
            int result = 0;
            try
            {
                if (model.Tag == "Add")
                {
                    model.DataModel.XSFLAG = 0;
                    result = T_XSDDomain.GetInstance().AddModel(model.DataModel);
                }
                else if (model.Tag == "Edit")
                {
                    result = T_XSDDomain.GetInstance().UpdateModel(model.DataModel, model.DataModel.XSID);
                }
            }
            catch { }
            Response.ContentType = "text/json";
            if (result > 0)
                Response.Write("{\"statusCode\":\"200\", \"message\":\"操作成功\",\"callbackType\":\"closeCurrentReloadTab\",\"forwardUrl\":\"/T_XSD/Index\"}");
            else
                Response.Write("{\"statusCode\":\"300\", \"message\":\"操作失败\"}");
        }

        [CheckLogin()]
        public ActionResult XSDSPIndex(System.Int32 id, string tag)
        {

            //加载购买商商企业列表
            T_CusQYModels cusQymode = new T_CusQYModels();

            cusQymode.DataModel = cusQymode.DataModel ?? new T_CusQY();

            cusQymode.DataList = T_CusQYDomain.GetInstance().GetAllT_CusQY(cusQymode.DataModel).Where(p => p.CusStatus == Convert.ToInt32("1")).ToList();

            ViewData["CusQY"] = new SelectList(cusQymode.DataList, "CusID", "CusMC");

            //加载销售人员列表
            T_PersonModels perQymode = new T_PersonModels();

            perQymode.DataModel = perQymode.DataModel ?? new T_Person();

            perQymode.DataList = T_PersonDomain.GetInstance().GetAllT_Person(perQymode.DataModel);

            ViewData["Person"] = new SelectList(perQymode.DataList, "PsID", "PsMZ");

            T_XSDModels model = new T_XSDModels();
            model.DataModel = new T_XSD();
            T_XSMX xsmodel = new T_XSMX();
            if (id != 0)
            {
                model.DataModel = T_XSDDomain.GetInstance().GetModelById(id);
                model.XSMXList = T_XSMXDomain.GetInstance().GetAllT_XSMX(xsmodel);
                if (model.XSMXList.Count > 0)
                {
                    model.XSMXList = model.XSMXList.Where(p => p.XSID == id).ToList();
                }
            }
            model.Tag = tag;
            return View("~/Views/T_XSD/XSDSPIndex.cshtml", model);
        }

        [HttpPost]
        [CheckLogin()]
        public void through(T_XSDModels model, string id)
        {
            int result = 0;
            try
            {
                Int32 xsdid = model.DataModel.XSID;
                result = T_XSDDomain.GetInstance().Sh(xsdid, id == "1" ? 1 : 2);
            }
            catch { }
            Response.ContentType = "text/json";
            if (result > 0)
                Response.Write("{\"statusCode\":\"200\", \"message\":\"操作成功\",\"callbackType\":\"closeCurrentReloadTab\",\"forwardUrl\":\"/T_XSD/Index\"}");
            else
                Response.Write("{\"statusCode\":\"300\", \"message\":\"操作失败\"}");
        }

        [HttpPost]
        [CheckLogin()]
        public void Delete(System.Int32 id)
        {
            int result = T_XSDDomain.GetInstance().DeleteModelById(id);
            Response.ContentType = "text/json";
            if (result > 0)
                Response.Write("{\"statusCode\":\"200\", \"message\":\"操作成功\",\"callbackType\":\"forward\",\"forwardUrl\":\"/T_XSD/Index\"}");
            else
                Response.Write("{\"statusCode\":\"300\", \"message\":\"操作失败\"}");
        }
    }
}
