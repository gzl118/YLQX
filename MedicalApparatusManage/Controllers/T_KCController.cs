using MedicalApparatusManage.Domain;
using MedicalApparatusManage.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MedicalApparatusManage.Controllers
{
    public class T_KCController : Controller
    {
        public int resultCount = 0; // 总条数 

        [CheckLogin()]
        public ActionResult Index(T_KCModels evalModel)
        {
            try
            {
                evalModel.currentPage = int.Parse(Request["pageNum"].ToString());
            }
            catch { }
            string ckid = "";
            string qyid = "";
            string cpid = "";
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

            //加载购买商商企业列表
            T_YLCPModels ylcpQymode = new T_YLCPModels();

            ylcpQymode.DataModel = ylcpQymode.DataModel ?? new T_YLCP();

            ylcpQymode.DataList = T_YLCPDomain.GetInstance().GetAllT_YLCP(ylcpQymode.DataModel).Where(p => p.CPStatus == Convert.ToInt32("1")).ToList();

            ViewData["YLCP"] = new SelectList(ylcpQymode.DataList, "CPID", "CPMC");

            //加载购买商商企业列表
            T_CKModels ckmode = new T_CKModels();

            ckmode.DataModel = ckmode.DataModel ?? new T_CK();

            ckmode.DataList = T_CKDomain.GetInstance().GetAllT_CK(ckmode.DataModel);

            ViewData["CK"] = new SelectList(ckmode.DataList, "CKID", "CKMC");

            //加载产品生产企业
            T_SupQYModels qymode = new T_SupQYModels();

            qymode.DataModel = qymode.DataModel ?? new T_SupQY();

            qymode.DataList = T_SupQYDomain.GetInstance().GetAllT_SupQY(qymode.DataModel).Where(p => p.SupStatus == 1).ToList();
            ViewData["QY"] = new SelectList(qymode.DataList, "SupID", "SupMC");


            int pagesize = Convert.ToInt32(evalModel.pageSize);
            int pagecount = Convert.ToInt32(evalModel.pagecount);
            int currentPage = Convert.ToInt32(evalModel.currentPage);
            evalModel.DataModel = evalModel.DataModel ?? new T_KC();
            if (Request["CKList"] != null)
            {
                ckid = Request["CKList"].ToString();
                if (!String.IsNullOrEmpty(ckid))
                {
                    evalModel.DataModel.CKID = Convert.ToInt16(ckid);
                }
            }

            if (Request["CPList"] != null)
            {
                cpid = Request["CPList"].ToString();
                if (!String.IsNullOrEmpty(cpid))
                {
                    evalModel.DataModel.CPID = Convert.ToInt16(cpid);
                }
            }
            if (Request["QYList"] != null)
            {
                qyid = Request["QYList"].ToString();
                if (!String.IsNullOrEmpty(qyid))
                {
                    evalModel.DataModel.ScqyID = Convert.ToInt16(qyid);
                }
            }
            var supid = "";
            if (Request["strKCSupQY"] != null)
            {
                supid = Request["strKCSupQY"].ToString();
                if (!String.IsNullOrEmpty(supid))
                {
                    evalModel.DataModel.SupID = Convert.ToInt16(supid);
                }
            }

            ViewData["CKList"] = ckid;
            ViewData["CPList"] = cpid;
            ViewData["QYList"] = qyid;
            ViewData["strKCSupQY"] = supid;
            evalModel.DataList = T_KCDomain.GetInstance().PageT_KC(evalModel.DataModel, evalModel.StartTime, evalModel.EndTime, currentPage, pagesize, out pagecount, out resultCount);
            evalModel.resultCount = resultCount;
            return View("~/Views/T_KC/Index.cshtml", evalModel);
        }

        [CheckLogin()]
        public ActionResult Save(System.Int32 id, string tag)
        {
            //加载购买商商企业列表
            T_YLCPModels ylcpQymode = new T_YLCPModels();

            ylcpQymode.DataModel = ylcpQymode.DataModel ?? new T_YLCP();

            ylcpQymode.DataList = T_YLCPDomain.GetInstance().GetAllT_YLCP(ylcpQymode.DataModel).Where(p => p.CPStatus == Convert.ToInt32("1")).ToList();

            ViewData["YLCP"] = new SelectList(ylcpQymode.DataList, "CPID", "CPMC");

            //加载购买商商企业列表
            T_CKModels ckmode = new T_CKModels();

            ckmode.DataModel = ckmode.DataModel ?? new T_CK();

            ckmode.DataList = T_CKDomain.GetInstance().GetAllT_CK(ckmode.DataModel);

            ViewData["CK"] = new SelectList(ckmode.DataList, "CKID", "CKMC");

            T_KCModels model = new T_KCModels();
            model.DataModel = new T_KC();
            if (id != 0)
            {
                model.DataModel = T_KCDomain.GetInstance().GetModelById(id);
            }
            model.Tag = tag;
            return View("~/Views/T_KC/Save.cshtml", model);
        }

        [HttpPost]
        [CheckLogin()]
        public void Save(T_KCModels model)
        {
            int result = 0;
            try
            {
                if (model.Tag == "Add")
                {
                    result = T_KCDomain.GetInstance().AddModel(model.DataModel);
                }
                else if (model.Tag == "Edit")
                {
                    result = T_KCDomain.GetInstance().UpdateModel(model.DataModel, model.DataModel.CKID);
                }

            }
            catch { }
            Response.ContentType = "text/json";
            if (result > 0)
                Response.Write("{\"statusCode\":\"200\", \"message\":\"操作成功\",\"callbackType\":\"closeCurrentReloadTab\",\"forwardUrl\":\"/T_KC/Index\"}");
            else
                Response.Write("{\"statusCode\":\"300\", \"message\":\"操作失败\"}");
        }

        [HttpPost]
        [CheckLogin()]
        public void Delete(System.Int32 id)
        {
            int result = T_KCDomain.GetInstance().DeleteModelById(id);
            Response.ContentType = "text/json";
            if (result > 0)
                Response.Write("{\"statusCode\":\"200\", \"message\":\"操作成功\",\"callbackType\":\"forward\",\"forwardUrl\":\"/T_KC/Index\"}");
            else
                Response.Write("{\"statusCode\":\"300\", \"message\":\"操作失败\"}");
        }
    }
}
