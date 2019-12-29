﻿using MedicalApparatusManage.Common;
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
    public class T_XSHZController : Controller
    {
        public int resultCount = 0; // 总条数 
        // GET: T_XSHZ
        public ActionResult Index(T_XSMXModels evalModel)
        {
            try
            {
                evalModel.currentPage = int.Parse(Request["pageNum"].ToString());
            }
            catch { }
            string order = "";
            string qyid = "";
            try
            {
                order = Request["orderField"].ToString();
            }
            catch { }
            if (order.Trim() == "${param.orderField}")
            {
                order = "";
            }
            evalModel.DataModel = evalModel.DataModel ?? new T_XSMX();
            evalModel.DataModel.T_YLCP = evalModel.DataModel.T_YLCP ?? new T_YLCP();

            //加载产品生产企业
            T_SupQYModels qymode = new T_SupQYModels();
            qymode.DataModel = qymode.DataModel ?? new T_SupQY();
            qymode.DataList = T_SupQYDomain.GetInstance().GetAllT_SupQY(qymode.DataModel);
            ViewData["QY"] = new SelectList(qymode.DataList, "SupID", "SupMC");
            if (Request["QYList"] != null)
            {
                qyid = Request["QYList"].ToString();
                if (!String.IsNullOrEmpty(qyid))
                {
                    T_SupQY qycp = new T_SupQY();
                    qycp.SupID = int.Parse(qyid);
                    evalModel.DataModel.T_YLCP.T_SupQY1 = qycp;
                }
            }
            ViewData["QYList"] = qyid;

            Expression<Func<T_YLCP, bool>> where = PredicateBuilder.True<T_YLCP>();
            var lst = T_YLCPDomain.GetInstance().GetAllModels<int>(where);
            ViewData["XSHZ_YLCP"] = new SelectList(lst, "CPID", "CPMC");
            var cpid = "";
            if (Request["XSHZ_CPID"] != null)
            {
                cpid = Request["XSHZ_CPID"].ToString();
                if (!String.IsNullOrEmpty(cpid))
                {
                    evalModel.DataModel.CPID = int.Parse(cpid);
                }
            }
            ViewData["XSHZ_CPID"] = cpid;

            int pagesize = Convert.ToInt32(evalModel.pageSize);
            int pagecount = Convert.ToInt32(evalModel.pagecount);
            int currentPage = Convert.ToInt32(evalModel.currentPage);
            evalModel.DataModel = evalModel.DataModel ?? new T_XSMX();
            evalModel.DataList = T_XSMXDomain.GetInstance().PageT_XSMX(evalModel.DataModel, evalModel.StartTime, evalModel.EndTime, currentPage, pagesize, out pagecount, out resultCount);
            evalModel.resultCount = resultCount;

            var totalNum = 0;
            if (evalModel.DataList != null && evalModel.DataList.Count > 0)
            {
                evalModel.DataList.ForEach(p =>
                {
                    totalNum += (p.CPSL ?? 0);
                });
            }
            ViewBag.TotalNum = totalNum;
            return View("~/Views/T_XSHZ/Index.cshtml", evalModel);
        }
    }
}