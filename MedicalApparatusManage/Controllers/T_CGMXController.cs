using MedicalApparatusManage.Domain;
using MedicalApparatusManage.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MedicalApparatusManage.Controllers
{
    public class T_CGMXController : Controller
    {
        public int resultCount = 0; // 总条数 

        [CheckLogin()]
        public ActionResult Index(T_CGMXModels evalModel, string id)
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
            ViewBag.CGDID = id;
            evalModel.DataModel = evalModel.DataModel ?? new T_CGMX();
            evalModel.DataList = T_CGMXDomain.GetInstance().PageT_CGMX(evalModel.DataModel, evalModel.StartTime, evalModel.EndTime, currentPage, pagesize, out pagecount, out resultCount).Where(p => p.CGID == int.Parse(id)).ToList();
            evalModel.resultCount = resultCount;
            return View("~/Views/T_CGMX/Index.cshtml", evalModel);
        }

        [CheckLogin()]
        public ActionResult Save(System.Int32 id, string tag)
        {
            T_CGMXModels model = new T_CGMXModels();
            model.DataModel = new T_CGMX();
            Int32 did = id;
            if (tag != "Add")
            {
                model.DataModel = T_CGMXDomain.GetInstance().GetModelById(id);
                did = model.DataModel.CGID;
            }

            //加载产品列表
            T_YLCPModels ylcpmode = new T_YLCPModels();

            ylcpmode.DataModel = ylcpmode.DataModel ?? new T_YLCP();

            ylcpmode.DataList = T_YLCPDomain.GetInstance().GetAllT_YLCP(ylcpmode.DataModel);

            ViewData["YLCP"] = new SelectList(ylcpmode.DataList, "CPID", "CPMC");

            //加载企业列表
            T_SupQYModels supmode = new T_SupQYModels();

            supmode.DataModel = supmode.DataModel ?? new T_SupQY();

            supmode.DataList = T_SupQYDomain.GetInstance().GetAllT_SupQY(supmode.DataModel).Where(p => p.SupStatus == 1).ToList();

            ViewData["SupID"] = new SelectList(supmode.DataList, "SupID", "SupMC");

            //加载采购单列表
            T_CGDModels cgdQymode = new T_CGDModels();

            cgdQymode.DataModel = cgdQymode.DataModel ?? new T_CGD();
            T_CGD cgd = T_CGDDomain.GetInstance().GetModelById(did);
            cgdQymode.DataList = new List<T_CGD>();
            cgdQymode.DataList.Add(cgd);
            ViewData["CGD"] = new SelectList(cgdQymode.DataList, "CGID", "CGDMC");



            //if (id != 0)
            //{
            //    model.DataModel = T_CGMXDomain.GetInstance().GetModelById(id);
            //}
            model.Tag = tag;
            return View("~/Views/T_CGMX/Save.cshtml", model);
        }

        [HttpPost]
        [CheckLogin()]
        public void Save(T_CGMXModels model)
        {
            int result = 0;
            string guid = string.Empty;
            try
            {
                if (model.Tag == "Add")
                {
                    model.DataModel.GUID = Guid.NewGuid().ToString("N");
                    guid = model.DataModel.GUID;
                    result = T_CGMXDomain.GetInstance().AddModelByCgdh(model.DataModel, model.CGDH);
                }
                else if (model.Tag == "Edit")
                {
                    result = T_CGMXDomain.GetInstance().UpdateModel(model.DataModel, model.DataModel.MXID);
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
        public void Delete(string Guid)
        {
            int result = T_CGMXDomain.GetInstance().DeleteModelByGuid(Guid);
            Response.ContentType = "text/json";
            if (result > 0)
                Response.Write("{\"statusCode\":\"200\", \"message\":\"操作成功\",\"callbackType\":\"forward\",\"forwardUrl\":\"/T_CGD/Index\"}");
            else
                Response.Write("{\"statusCode\":\"300\", \"message\":\"操作失败\"}");
        }

        [CheckLogin()]
        public ActionResult CGMXTable(System.Int32 id, string cgdh, int canEdit)
        {
            T_CGDModels model = new T_CGDModels();
            model.DataModel = new T_CGD();
            if (id != 0)
            {
                model.CGMXList = T_CGMXDomain.GetInstance().GetT_CGMXByCgid(id);
            }
            else
            {
                model.CGMXList = T_CGMXDomain.GetInstance().GetT_YSMXByCgdh(cgdh);
            }
            ViewData["canEdit"] = canEdit;
            string roleId = Session["RoleId"].ToString();
            ViewData["IsLeader"] = false;
            if (roleId == "1" || roleId == "2")
            {
                ViewData["IsLeader"] = true;
            }
            return View("~/Views/T_CGMX/CGMXTable.cshtml", model);
        }
    }
}
