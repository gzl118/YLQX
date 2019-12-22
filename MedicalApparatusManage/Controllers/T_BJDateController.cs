using MedicalApparatusManage.Common;
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
    public class T_BJDateController : Controller
    {
        // GET: T_BJDate
        [CheckLogin()]
        public ActionResult Index()
        {
            T_BJDateModels model = new T_BJDateModels();
            Expression<Func<T_BJDate, bool>> where = PredicateBuilder.True<T_BJDate>();
            var temp = T_BJDateDomain.GetInstance().GetAllModels<int>(where);
            model.DataList = temp;
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
        [HttpPost]
        [CheckLogin()]
        public void SaveNew(List<T_BJDate> list)
        {
            int result = 0;
            try
            {
                if (list != null && list.Count > 0)
                {
                    result = T_BJDateDomain.GetInstance().UpdateList(list);
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