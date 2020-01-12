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
    public class T_BJDController : Controller
    {
        public int resultCount = 0; // 总条数 

        [CheckLogin()]
        public ActionResult Index(T_BJDModels evalModel)
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
            evalModel.DataModel = evalModel.DataModel ?? new T_BJD();
            evalModel.DataList = T_BJDDomain.GetInstance().PageT_BJD(evalModel.DataModel, evalModel.StartTime, evalModel.EndTime, currentPage, pagesize, out pagecount, out resultCount);
            evalModel.resultCount = resultCount;
            return View("~/Views/T_BJD/Index.cshtml", evalModel);
        }

        [CheckLogin()]
        public ActionResult Save(System.Int32 id, string tag)
        {
            T_BJDModels model = new T_BJDModels();

            model.DataModel = new T_BJD();
            if (id != 0)
            {
                model.DataModel = T_BJDDomain.GetInstance().GetModelById(id);
            }
            T_SJZDFL pFl = T_SJZDFLDomain.GetInstance().GetModeByName("1");

            //加载类型
            var list = T_SJZDLXDomain.GetInstance().GetAllT_SJZDLX(new T_SJZDLX());
            ViewData["LX"] = new SelectList(list, "LXID", "LXMC");

            model.Tag = tag;
            return View("~/Views/T_BJD/Save.cshtml", model);
        }

        [HttpPost]
        [CheckLogin()]
        public void Save(T_BJDModels model)
        {
            int result = 0;
            try
            {
                if (model.Tag == "Add")
                {
                    result = T_BJDDomain.GetInstance().AddModel(model.DataModel);
                }
                else if (model.Tag == "Edit")
                {
                    result = T_BJDDomain.GetInstance().UpdateModel(model.DataModel, model.DataModel.BJID);
                }

            }
            catch { }
            Response.ContentType = "text/json";
            if (result > 0)
                Response.Write("{\"statusCode\":\"200\", \"message\":\"操作成功\",\"callbackType\":\"closeCurrentReloadTab\",\"forwardUrl\":\"/T_BJD/Index\"}");
            else
                Response.Write("{\"statusCode\":\"300\", \"message\":\"操作失败\"}");
        }

        [HttpPost]
        [CheckLogin()]
        public void Delete(System.Int32 id)
        {
            int result = T_BJDDomain.GetInstance().DeleteModelById(id);
            Response.ContentType = "text/json";
            if (result > 0)
                Response.Write("{\"statusCode\":\"200\", \"message\":\"操作成功\",\"callbackType\":\"forward\",\"forwardUrl\":\"/T_BJD/Index\"}");
            else
                Response.Write("{\"statusCode\":\"300\", \"message\":\"操作失败\"}");
        }

        public void GetAlarmInfo()
        {
            int result = T_BJDDomain.GetInstance().GetCount();
            Response.Write(result);
        }
        /// <summary>
        /// 获取提示信息
        /// </summary>
        public string GetTipInfo()
        {
            var strHtml = new StringBuilder();
            Expression<Func<ActivityInfo, bool>> whereActivityInfo = p => (p.EndTime != null && DateTime.Now <= p.EndTime.Value);
            whereActivityInfo = whereActivityInfo.And(p => p.StartTime != null && DateTime.Now >= p.StartTime.Value);
            var ActivityInfoCount = ActivityInfoDomain.GetInstance().GetAllModels<int>(whereActivityInfo).Count; //有效的公告数量

            var AlarmCount = T_BJDDomain.GetInstance().GetCount(); //报警数量
            if (AlarmCount > 0)
            {
                strHtml.Append(string.Format(strTemplete, "alarmManage", "/T_BJD/Index/", "预警通知", AlarmCount, "报警"));
            }
            if (ActivityInfoCount > 0)
            {
                strHtml.Append(string.Format(strTemplete, "ActivityInfoManage", "/ActivityInfo/Index/", "公告管理", ActivityInfoCount, "公告"));
            }
            var RoleCode = GetRoleCode();
            if (RoleCode == "2")
            {
                #region 
                Expression<Func<T_SupQY, bool>> whereSupQY = p => p.SupStatus == 0;
                var SupQYCount = T_SupQYDomain.GetInstance().GetAllModels<int>(whereSupQY).Count; //待审批的供货企业数量

                Expression<Func<T_YLCP, bool>> whereYLCP = p => p.CPStatus == 0;
                var YLCPCount = T_YLCPDomain.GetInstance().GetAllModels<int>(whereYLCP).Count; //待审批的医疗产品数量

                Expression<Func<T_CusQY, bool>> whereCusQY = p => p.CusStatus == 0;
                var CusQYCount = T_CusQYDomain.GetInstance().GetAllModels<int>(whereCusQY).Count; //待审批的购货企业数量

                Expression<Func<T_CGD, bool>> whereCGD = p => p.ISSH == 0;
                var CGDCount = T_CGDDomain.GetInstance().GetAllModels<int>(whereCGD).Count; //待审批的采购单数量

                Expression<Func<T_RKD, bool>> whereRKD = p => p.ISSH == 0;
                var RKDCount = T_RKDDomain.GetInstance().GetAllModels<int>(whereRKD).Count; //待审批的入库单数量

                Expression<Func<T_XSD, bool>> whereXSD = p => p.XSFLAG == 0;
                var XSDCount = T_XSDDomain.GetInstance().GetAllModels<int>(whereXSD).Count; //待审批的销售单数量

                Expression<Func<T_SHD, bool>> whereSHD = p => p.ISSH == 0;
                var SHDCount = T_SHDDomain.GetInstance().GetAllModels<int>(whereSHD).Count; //待审批的损耗单数量

                if (SupQYCount > 0)
                {
                    strHtml.Append(string.Format(strTemplete, "SupQYManage", "/T_SupQY/Index/", "供货企业", SupQYCount, "供货企业待审批"));
                }
                if (YLCPCount > 0)
                {
                    strHtml.Append(string.Format(strTemplete, "YLCPManage", "/T_YLCP/Index/", "产品信息", YLCPCount, "产品待审批"));
                }
                if (CusQYCount > 0)
                {
                    strHtml.Append(string.Format(strTemplete, "CusQYManage", "/T_CusQY/Index/", "购货企业", CusQYCount, "购货企业待审批"));
                }
                if (CGDCount > 0)
                {
                    strHtml.Append(string.Format(strTemplete, "CGDManage", "/T_CGD/Index/", "采购管理", CGDCount, "采购单待审批"));
                }
                if (RKDCount > 0)
                {
                    strHtml.Append(string.Format(strTemplete, "RKDManage", "/T_RKD/Index/", "入库管理", RKDCount, "入库单待审批"));
                }
                if (XSDCount > 0)
                {
                    strHtml.Append(string.Format(strTemplete, "XSDManage", "/T_XSD/Index/", "销售管理", XSDCount, "销售单待审批"));
                }
                if (SHDCount > 0)
                {
                    strHtml.Append(string.Format(strTemplete, "SHDManage", "/T_SHD/Index/", "损耗管理", SHDCount, "损耗单待审批"));
                }
                #endregion
            }
            return strHtml.ToString();
        }
        private string GetRoleCode()
        {
            return Session["RoleCode"] == null ? "" : Session["RoleCode"].ToString();
        }
        private string strTemplete = "<div style='margin-bottom:5px;'><a href='javascript:void(0);' onclick='showMsg(\"{0}\",\"{1}\",\"{2}\")' >您有{3}条{4}信息</a></div><br/>";
    }
}
