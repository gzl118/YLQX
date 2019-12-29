using MedicalApparatusManage.Domain;
using MedicalApparatusManage.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace MedicalApparatusManage.Controllers
{
    public class T_XSMXController : Controller
    {
        public int resultCount = 0; // 总条数 

        [CheckLogin()]
        public ActionResult Index(T_XSMXModels evalModel, string id)
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
            ViewBag.XSDID = id;
            int pagesize = Convert.ToInt32(evalModel.pageSize);
            int pagecount = Convert.ToInt32(evalModel.pagecount);
            int currentPage = Convert.ToInt32(evalModel.currentPage);
            evalModel.DataModel = evalModel.DataModel ?? new T_XSMX();
            evalModel.DataList = T_XSMXDomain.GetInstance().PageT_XSMX(evalModel.DataModel, evalModel.StartTime, evalModel.EndTime, currentPage, pagesize, out pagecount, out resultCount).Where(p => p.XSID == int.Parse(id)).ToList();
            evalModel.resultCount = resultCount;
            return View("~/Views/T_XSMX/Index.cshtml", evalModel);
        }

        [CheckLogin()]
        public ActionResult Save(System.Int32 id, string tag)
        {
            T_XSMXModels model = new T_XSMXModels();
            model.DataModel = new T_XSMX();
            Int32 did = id;
            if (tag != "Add")
            {
                model.DataModel = T_XSMXDomain.GetInstance().GetModelById(id);
                did = model.DataModel.XSID ?? 0;
            }
            //加载销售单列表
            T_XSDModels xsdmode = new T_XSDModels();

            xsdmode.DataModel = xsdmode.DataModel ?? new T_XSD();

            // xsdmode.DataList = T_XSDDomain.GetInstance().GetAllT_XSD(xsdmode.DataModel);



            //加载产品列表
            T_YLCPModels ylcpmode = new T_YLCPModels();

            ylcpmode.DataModel = ylcpmode.DataModel ?? new T_YLCP();

            ylcpmode.DataList = T_YLCPDomain.GetInstance().GetAllT_YLCP(ylcpmode.DataModel).Where(p => p.CPStatus == Convert.ToInt32("1")).ToList();

            ViewData["YLCP"] = new SelectList(ylcpmode.DataList, "CPID", "CPMC");



            T_XSD rkd = T_XSDDomain.GetInstance().GetModelById(did);
            xsdmode.DataList = new List<T_XSD>();
            xsdmode.DataList.Add(rkd);
            ViewData["XSD"] = new SelectList(xsdmode.DataList, "XSID", "XSMC");


            //if (id != 0)
            //{
            //    model.DataModel = T_XSMXDomain.GetInstance().GetModelById(id);
            //}
            model.Tag = tag;
            return View("~/Views/T_XSMX/Save.cshtml", model);
        }

        [HttpPost]
        [CheckLogin()]
        public void Save(T_XSMXModels model)
        {
            int result = 0;
            string guid = string.Empty;
            try
            {
                if (model.Tag == "Add")
                {
                    model.DataModel.GUID = Guid.NewGuid().ToString("N");
                    guid = model.DataModel.GUID;
                    result = T_XSMXDomain.GetInstance().AddModelByXsdh(model.DataModel, model.XSDH);
                }
                else if (model.Tag == "Edit")
                {
                    result = T_XSMXDomain.GetInstance().UpdateModel(model.DataModel, model.DataModel.MXID);
                }
            }
            catch { }
            Response.ContentType = "text/json";
            if (result > 0)
            {
                string resultStr = JsonConvert.SerializeObject(new { statusCode = "200", message = "操作成功", guid = guid });
                Response.Write(resultStr);
            }
            //Response.Write("{\"statusCode\":\"200\", \"message\":\"操作成功\",\"callbackType\":\"closeCurrentReloadTab\",\"forwardUrl\":\"/T_XSD/Index\"}");
            else
                Response.Write("{\"statusCode\":\"300\", \"message\":\"操作失败\"}");
        }

        [HttpPost]
        [CheckLogin()]
        public void Delete(string guid)
        {
            int result = T_XSMXDomain.GetInstance().DeleteModelByGuid(guid);
            Response.ContentType = "text/json";
            if (result > 0)
                Response.Write("{\"statusCode\":\"200\", \"message\":\"操作成功\",\"callbackType\":\"forward\",\"forwardUrl\":\"/T_XSD/Index\"}");
            else
                Response.Write("{\"statusCode\":\"300\", \"message\":\"操作失败\"}");
        }

        [HttpPost]
        public void GetYLCP(string xsdid)
        {
            string result1 = "";
            Dictionary<string, string> dict = new Dictionary<string, string>();
            try
            {
                StringBuilder result = new StringBuilder();
                result.Append("[[\"\",\"请选择\"]");
                //result.Append("[");
                if (string.IsNullOrEmpty(xsdid))
                {
                    result.Append("]");
                    result1 = result.ToString();
                }
                T_XSMX xsmx = new T_XSMX();
                xsmx.XSID = int.Parse(xsdid);
                var list = T_XSMXDomain.GetInstance().GetAllT_XSMX(xsmx);
                foreach (var item in list)
                {
                    result.Append(",[");
                    result.Append("\"" + item.T_YLCP.CPID + "\",");
                    result.Append("\"" + item.T_YLCP.CPMC + "\"");
                    result.Append("]");
                }
                result.Append("]");
                result1 = result.ToString();
            }
            catch (Exception ex)
            {
            }
            Response.ContentType = "text/json";
            Response.Write(result1);
        }

        [CheckLogin()]
        public ActionResult XSMXTable(System.Int32 id, string xsdh, int canEdit, int isSH)
        {
            T_XSDModels model = new T_XSDModels();
            model.DataModel = new T_XSD();
            model.DataModel.XSFLAG = isSH;
            if (id != 0)
            {
                model.XSMXList = T_XSMXDomain.GetInstance().GetT_XSMXByXsid(id);
            }
            else
            {
                model.XSMXList = T_XSMXDomain.GetInstance().GetT_XSMXByXsdh(xsdh);
            }
            ViewData["canEdit"] = canEdit;
            return View("~/Views/T_XSMX/XSMXTable.cshtml", model);
        }
    }
}
