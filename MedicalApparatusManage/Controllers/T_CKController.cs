using MedicalApparatusManage.Domain;
using MedicalApparatusManage.Models;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace MedicalApparatusManage.Controllers
{
    public class T_CKController : Controller
    {
        public int resultCount = 0; // 总条数 

        [CheckLogin()]
        public ActionResult Index(T_CKModels evalModel)
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
            evalModel.DataModel = evalModel.DataModel ?? new T_CK();
            if (Request["strCKName"] != null)
            {
                string str = Request["strCKName"].ToString();
                if (!String.IsNullOrEmpty(str))
                {
                    evalModel.DataModel.CKMC = str;
                }
            }
            var strCKPerson = "请选择";
            if (Request["strCKPerson"] != null)
            {
                strCKPerson = Request["strCKPerson"].ToString();
                if (!String.IsNullOrEmpty(strCKPerson))
                {
                    evalModel.DataModel.CKGLY = strCKPerson;
                }
            }
            SysUser UserModel = Session["UserModel"] as SysUser;
            T_Person person = new T_Person();
            person.PsQYID = (int)UserModel.UserCompanyID;
            ViewBag.Persons = new SelectList(T_PersonDomain.GetInstance().GetAllT_Person(person), "PsMZ", "PsMZ");
            ViewData["strCKPerson"] = strCKPerson;

            evalModel.DataList = T_CKDomain.GetInstance().PageT_CK(evalModel.DataModel, evalModel.StartTime, evalModel.EndTime, currentPage, pagesize, out pagecount, out resultCount);
            evalModel.resultCount = resultCount;
            return View("~/Views/T_CK/Index.cshtml", evalModel);
        }

        [CheckLogin()]
        public ActionResult Save(System.Int32 id, string tag)
        {
            T_CKModels model = new T_CKModels();
            model.DataModel = new T_CK();
            if (id != 0)
            {
                model.DataModel = T_CKDomain.GetInstance().GetModelById(id);
            }
            model.Tag = tag;
            T_Person person = new T_Person();
            SysUser UserModel = Session["UserModel"] as SysUser;
            person.PsQYID = (int)UserModel.UserCompanyID;
            ViewBag.Persons = new SelectList(T_PersonDomain.GetInstance().GetAllT_Person(person), "PsMZ", "PsMZ");
            return View("~/Views/T_CK/Save.cshtml", model);
        }

        [HttpPost]
        [CheckLogin()]
        public void Save(T_CKModels model)
        {
            int result = 0;
            try
            {
                if (model.Tag == "Add")
                {
                    result = T_CKDomain.GetInstance().AddModel(model.DataModel);
                }
                else if (model.Tag == "Edit")
                {
                    result = T_CKDomain.GetInstance().UpdateModel(model.DataModel, model.DataModel.CKID);
                }

            }
            catch { }
            Response.ContentType = "text/json";
            if (result > 0)
                Response.Write("{\"statusCode\":\"200\", \"message\":\"操作成功\",\"callbackType\":\"closeCurrentReloadTab\",\"forwardUrl\":\"/T_CK/Index\"}");
            else
                Response.Write("{\"statusCode\":\"300\", \"message\":\"操作失败\"}");
        }

        [HttpPost]
        [CheckLogin()]
        public void Delete(System.Int32 id)
        {
            int result = T_CKDomain.GetInstance().DeleteModelById(id);
            Response.ContentType = "text/json";
            if (result > 0)
                Response.Write("{\"statusCode\":\"200\", \"message\":\"操作成功\",\"callbackType\":\"forward\",\"forwardUrl\":\"/T_CK/Index\"}");
            else
                Response.Write("{\"statusCode\":\"300\", \"message\":\"操作失败\"}");
        }

        [HttpPost]
        [CheckLogin()]
        public JsonResult GetCK(string ckid)
        {
            T_CK ck = T_CKDomain.GetInstance().GetModelById(Convert.ToInt32(ckid));
            if (ck != null)
            {

                string resultStr = JsonConvert.SerializeObject(new { CKGLY = ck.CKGLY });
                return Json(resultStr);
            }
            else
            {
                return Json("");
            }
        }

        [HttpPost]
        public void GetCkByCPID(string id)
        {
            string result1 = "";
            Dictionary<string, string> dict = new Dictionary<string, string>();
            try
            {
                StringBuilder result = new StringBuilder();
                result.Append("[[\"\",\"请选择\"]");
                //result.Append("[");
                if (string.IsNullOrEmpty(id))
                {
                    result.Append("]");
                    result1 = result.ToString();
                }
                T_KC kc = new T_KC();
                kc.CPID = int.Parse(id);
                var list = T_KCDomain.GetInstance().GetAllT_KC(kc);
                if (list != null && list.Count > 0)
                {
                    Hashtable ht = new Hashtable();
                    foreach (var item in list)
                    {
                        if (!ht.ContainsKey(item.T_CK.CKID))
                            ht.Add(item.T_CK.CKID, item.T_CK.CKMC);

                    }
                    foreach (var key in ht.Keys)
                    {
                        result.Append(",[");
                        result.Append("\"" + key + "\",");
                        result.Append("\"" + ht[key].ToString() + "\"");
                        result.Append("]");
                    }
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

        [HttpPost]
        public void GetCkByCPPH(string cpid, string ckid)
        {
            string result1 = "";
            Dictionary<string, string> dict = new Dictionary<string, string>();
            try
            {
                StringBuilder result = new StringBuilder();
                result.Append("[[\"\",\"请选择\"]");
                //result.Append("[");
                if (string.IsNullOrEmpty(cpid) || string.IsNullOrEmpty(ckid))
                {
                    result.Append("]");
                    result1 = result.ToString();
                }
                T_KC kc = new T_KC();
                kc.CPID = int.Parse(cpid);
                kc.CKID = int.Parse(ckid);
                var list = T_KCDomain.GetInstance().GetAllT_KC(kc);
                foreach (var item in list)
                {
                    result.Append(",[");
                    result.Append("\"" + item.CPPH + "\",");
                    result.Append("\"" + item.CPPH + "\"");
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


        [HttpPost]
        public JsonResult GetCPByCPPH(string cpid, string ckid, string cpph)
        {
            if (string.IsNullOrEmpty(cpid) || string.IsNullOrEmpty(ckid) || string.IsNullOrEmpty(cpph))
            {
                return Json("");
            }
            T_KC kc = new T_KC();
            kc.CPID = int.Parse(cpid);
            kc.CKID = int.Parse(ckid);
            kc.CPPH = cpph;
            var list = T_KCDomain.GetInstance().GetAllT_KC(kc);
            if (list.Count > 0)
            {
                T_KC kcp = list[0];
                string resultStr = JsonConvert.SerializeObject(new
                {
                    CPBH = (kcp.T_YLCP != null && kcp.T_YLCP.CPBH != null) ? kcp.T_YLCP.CPBH : "",
                    SupMC = (kcp.T_SupQY1 != null && !string.IsNullOrEmpty(kcp.T_SupQY1.SupMC)) ? kcp.T_SupQY1.SupMC : "",
                    CPGG = (kcp.T_YLCP != null && kcp.T_YLCP.CPGG != null) ? kcp.T_YLCP.CPGG : "",
                    CPXH = (kcp.T_YLCP != null && kcp.T_YLCP.CPXH != null) ? kcp.T_YLCP.CPXH : "",
                    CPDW = (kcp.T_YLCP != null && kcp.T_YLCP.CPDW != null) ? kcp.T_YLCP.CPDW : "",
                    CPNUM = kcp.CPNUM,
                    CPSCRQ = kcp.CPSCRQ.HasValue ? kcp.CPSCRQ.Value.ToString("yyyy/MM/dd") : "",
                    CPYXQ = kcp.CPYXQ.HasValue ? kcp.CPYXQ.Value.ToString("yyyy/MM/dd") : "",
                    CPZCZ = (kcp.T_YLCP != null && kcp.T_YLCP.CPZCZ != null) ? kcp.T_YLCP.CPZCZ : "",
                    SupXKZBH = (kcp.T_SupQY1 != null && kcp.T_SupQY1.SupXKZBH != null) ? kcp.T_SupQY1.SupXKZBH : ""
                });
                return Json(resultStr);
            }
            else
            {
                return Json("");
            }

        }

        [HttpPost]
        public JsonResult GetCkByCkid(string ckid)
        {
            T_CK ck = T_CKDomain.GetInstance().GetModelById(int.Parse(ckid));
            string ckgly = (ck != null && ck.CKGLY != null) ? ck.CKGLY : "";
            return Json(JsonConvert.SerializeObject(new { CKGLY = ckgly }));
        }
    }
}
