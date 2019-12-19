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
            //model.DataModel = T_BJDateDomain.GetInstance().GetFirstBJDate();
            Expression<Func<T_BJDate, bool>> where = PredicateBuilder.True<T_BJDate>();
            var temp = T_BJDateDomain.GetInstance().GetAllModels<int>(where);
            //var tModel = new BjModel();
            //if (temp != null && temp.Count > 0)
            //{
            //    temp.ForEach(p =>
            //    {
            //        switch (p.BJCode)
            //        {
            //            case 1001:
            //                tModel.YYZHAlarmValue = p.BJDATE;
            //                break;
            //            case 1002:
            //                tModel.SCXKZAlarmValue = p.BJDATE;
            //                break;
            //            case 1003:
            //                tModel.JYXKZAlarmValue = p.BJDATE;
            //                break;
            //            case 1004:
            //                tModel.CPZCZAlarmValue = p.BJDATE;
            //                break;
            //            case 1005:
            //                tModel.SQSAlarmValue = p.BJDATE;
            //                break;
            //        }
            //    });
            //}
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
        public void SaveNew(T_BJDateModels model)
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
    public class BjModel
    {
        /// <summary>
        /// 营业执照报警期限,Code=1001
        /// </summary>
        public int? YYZHAlarmValue { get; set; }

        /// <summary>
        /// 生产许可证报警期限,Code=1002
        /// </summary>
        public int? SCXKZAlarmValue { get; set; }
        /// <summary>
        /// 经营许可证报警期限,Code=1003
        /// </summary>
        public int? JYXKZAlarmValue { get; set; }
        /// <summary>
        /// 产品注册证报警期限,Code=1004
        /// </summary>
        public int? CPZCZAlarmValue { get; set; }
        /// <summary>
        /// 授权书报警期限,Code=1005
        /// </summary>
        public int? SQSAlarmValue { get; set; }
    }
}