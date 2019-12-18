using MedicalApparatusManage.Domain;
using MedicalApparatusManage.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MedicalApparatusManage.Controllers
{
    public class T_BJDateController : Controller
    {
        // GET: T_BJDate
        [CheckLogin()]
        public ActionResult Index()
        {
            T_BJDateModels model = new T_BJDateModels();
            model.DataModel = T_BJDateDomain.GetInstance().GetFirstBJDate();
            return View("~/Views/T_BJDate/Index.cshtml", model);
        }

        [HttpPost]
        [CheckLogin()]
        public void Save(T_BJDateModels model)
        {
            int result = 0;
            try
            {
                if (model.DataModel.ID > 0)
                {
                    result = T_BJDateDomain.GetInstance().UpdateModel(model.DataModel, model.DataModel.ID);
                }
            }
            catch { }
            Response.ContentType = "text/json";
            if (result > 0)
                Response.Write("{\"statusCode\":\"200\", \"message\":\"操作成功\"}");
            else
                Response.Write("{\"statusCode\":\"300\", \"message\":\"操作失败\"}");
        }
    }
}