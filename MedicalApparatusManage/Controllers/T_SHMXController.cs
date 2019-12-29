using MedicalApparatusManage.Domain;
using MedicalApparatusManage.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MedicalApparatusManage.Controllers
{
    public class T_SHMXController : Controller
    {
        // GET: T_SHMX
        public ActionResult Index()
        {
            return View();
        }
        [CheckLogin()]
        public ActionResult SHMXTable(System.Int32 id, string ckdh, int canEdit, int isSH)
        {
            T_SHDModels model = new T_SHDModels();
            if (id != 0)
            {
                model.SHMXList = T_SHDDomain.GetInstance().GetT_SHMXByshid(id);
            }
            else
            {
                model.SHMXList = T_SHDDomain.GetInstance().GetT_SHMXByCkdh(ckdh);
            }
            model.RoleCode = GetRoleCode();
            model.DataModel = new T_SHD();
            model.DataModel.ISSH = isSH;
            model.DataModel.SHID = id;
            return View("~/Views/T_SHD/SHMXTable.cshtml", model);
        }
        [HttpPost]
        [CheckLogin()]
        public void Save(T_SHMXModels model)
        {
            int result = 0;
            string guid = string.Empty;
            try
            {
                if (model.Tag == "Add")
                {
                    model.DataModel.GUID = Guid.NewGuid().ToString("N");
                    guid = model.DataModel.GUID;
                    result = T_SHMXDomain.GetInstance().AddModelByCkdh(model.DataModel, model.SHDH);
                }
                else if (model.Tag == "Edit")
                {
                    result = T_SHMXDomain.GetInstance().UpdateModel(model.DataModel, model.DataModel.SHMXID);
                }

            }
            catch { }
            Response.ContentType = "text/json";
            if (result > 0)
                Response.Write("{\"statusCode\":\"200\", \"message\":\"操作成功\",\"callbackType\":\"closeCurrentReloadTab\",\"forwardUrl\":\"/T_SHD/Index\"}");
            else
                Response.Write("{\"statusCode\":\"300\", \"message\":\"操作失败\"}");
        }
        private string GetRoleCode()
        {
            return Session["RoleCode"] == null ? "" : Session["RoleCode"].ToString();
        }
        [HttpPost]
        [CheckLogin()]
        public void Delete(string guid)
        {
            int result = T_SHMXDomain.GetInstance().DeleteModelByGuid(guid);
            Response.ContentType = "text/json";
            if (result > 0)
            {
                Response.Write("{\"statusCode\":\"200\", \"message\":\"操作成功\",\"callbackType\":\"forward\",\"forwardUrl\":\"/T_SHD/Index\"}");
            }
            else
                Response.Write("{\"statusCode\":\"300\", \"message\":\"操作失败\"}");
        }

    }
}