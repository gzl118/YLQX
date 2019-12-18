using MedicalApparatusManage.Domain;
using MedicalApparatusManage.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MedicalApparatusManage.Controllers
{
    public class T_THMXController : Controller
    {
        public int resultCount = 0; // 总条数 

        [CheckLogin()]
        public ActionResult Index(T_THMXModels evalModel, string id)
        {
            try
            {
                if (id == null)
                {
                    id = "1";
                }
                ViewData["XSMXType"] = id;
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
            evalModel.DataModel = evalModel.DataModel ?? new T_THMX();
            evalModel.DataList = T_THMXDomain.GetInstance().PageT_THMX(evalModel.DataModel, evalModel.StartTime, evalModel.EndTime, currentPage, pagesize, out pagecount, out resultCount).Where(p => p.FLAG == int.Parse(id)).ToList();
            evalModel.resultCount = resultCount;
            return View("~/Views/T_THMX/Index.cshtml", evalModel);
        }

        [CheckLogin()]
        public ActionResult Save(System.Int32 id, string tag)
        {
            //加载退货单企业列表
            T_THDModels thdQymode = new T_THDModels();

            thdQymode.DataModel = thdQymode.DataModel ?? new T_THD();

            thdQymode.DataList = T_THDDomain.GetInstance().GetAllT_THD(thdQymode.DataModel);

            ViewData["THD"] = new SelectList(thdQymode.DataList, "THID", "THMC");

            //加载购买商商企业列表
            T_YLCPModels cpmode = new T_YLCPModels();

            cpmode.DataModel = cpmode.DataModel ?? new T_YLCP();

            cpmode.DataList = T_YLCPDomain.GetInstance().GetAllT_YLCP(cpmode.DataModel).Where(p => p.CPStatus == Convert.ToInt32("1")).ToList();

            ViewData["YLCP"] = new SelectList(cpmode.DataList, "CPID", "CPMC");

            //加载购买商商企业列表
            T_CKModels ckmode = new T_CKModels();

            ckmode.DataModel = ckmode.DataModel ?? new T_CK();

            ckmode.DataList = T_CKDomain.GetInstance().GetAllT_CK(ckmode.DataModel);

            ViewData["CK"] = new SelectList(ckmode.DataList, "CKID", "CKMC");

            T_THMXModels model = new T_THMXModels();
            model.DataModel = new T_THMX();
            if (id != 0)
            {
                model.DataModel = T_THMXDomain.GetInstance().GetModelById(id);
            }
            model.Tag = tag;
            return View("~/Views/T_THMX/Save.cshtml", model);
        }

        [HttpPost]
        [CheckLogin()]
        public void Save(T_THMXModels model)
        {
            int result = 0;
            try
            {
                if (model.Tag == "Add")
                {
                    model.DataModel.FLAG = 1;
                    result = T_THMXDomain.GetInstance().AddModel(model.DataModel);
                }
                else if (model.Tag == "Edit")
                {
                    model.DataModel.FLAG = 1;
                    result = T_THMXDomain.GetInstance().UpdateModel(model.DataModel, model.DataModel.MXID);
                }

            }
            catch { }
            Response.ContentType = "text/json";
            if (result > 0)
                Response.Write("{\"statusCode\":\"200\", \"message\":\"操作成功\",\"callbackType\":\"closeCurrentReloadTab\",\"forwardUrl\":\"/T_THMX/Index\"}");
            else
                Response.Write("{\"statusCode\":\"300\", \"message\":\"操作失败\"}");
        }

        [HttpPost]
        [CheckLogin()]
        public void XSMXSave(T_THMXModels model)
        {
            int result = 0;
            try
            {
                if (model.Tag == "Add")
                {
                    model.DataModel.FLAG = 2;
                    result = T_THMXDomain.GetInstance().AddModel(model.DataModel);
                    T_KC KCmodel = new T_KC();
                    KCmodel = T_KCDomain.GetInstance().GetKCid(model.DataModel.CKID);
                    KCmodel.CPNUM = KCmodel.CPNUM + 1;
                    T_KCDomain.GetInstance().UpdateModel(KCmodel, KCmodel.CKID);
                }
                else if (model.Tag == "Edit")
                {
                    model.DataModel.FLAG = 2;
                    result = T_THMXDomain.GetInstance().UpdateModel(model.DataModel, model.DataModel.CPID);
                }

            }
            catch { }
            Response.ContentType = "text/json";
            if (result > 0)
                Response.Write("{\"statusCode\":\"200\", \"message\":\"操作成功\",\"callbackType\":\"closeCurrentReloadTab\",\"forwardUrl\":\"/T_THMX/Index\"}");
            else
                Response.Write("{\"statusCode\":\"300\", \"message\":\"操作失败\"}");
        }

        [HttpPost]
        [CheckLogin()]
        public void Delete(System.Int32 id)
        {
            int result = T_THMXDomain.GetInstance().DeleteModelById(id);
            Response.ContentType = "text/json";
            if (result > 0)
                Response.Write("{\"statusCode\":\"200\", \"message\":\"操作成功\",\"callbackType\":\"forward\",\"forwardUrl\":\"/T_THMX/Index\"}");
            else
                Response.Write("{\"statusCode\":\"300\", \"message\":\"操作失败\"}");
        }
    }
}
