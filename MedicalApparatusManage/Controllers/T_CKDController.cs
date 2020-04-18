using MedicalApparatusManage.Common;
using MedicalApparatusManage.Domain;
using MedicalApparatusManage.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace MedicalApparatusManage.Controllers
{
    public class T_CKDController : Controller
    {
        public int resultCount = 0; // 总条数 
        public SysUser CurUser = null;

        [CheckLogin()]
        public ActionResult Index(T_CKDModels evalModel)
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
            evalModel.DataModel = evalModel.DataModel ?? new T_CKD();

            if (Request["strCKDName"] != null)  //出库单号
            {
                string str = Request["strCKDName"].ToString();
                if (!String.IsNullOrEmpty(str))
                {
                    evalModel.DataModel.CKDH = str.Trim();
                }
                ViewData["strCKDName"] = str;
            }
            if (Request["strCKDMC"] != null)  //出库单名称
            {
                string str = Request["strCKDMC"].ToString();
                if (!String.IsNullOrEmpty(str))
                {
                    evalModel.DataModel.CKMC = str.Trim();
                }
            }
            var ghId = 0;
            if (Request["strCKDGHQY"] != null)
            {
                var str = Request["strCKDGHQY"].ToString();
                if (!string.IsNullOrEmpty(str))
                {
                    ghId = Convert.ToInt32(str);
                }
                ViewData["strCKDGHQY"] = str;
            }
            var cpId = 0;  //产品名称
            if (Request["strCKDCPMC"] != null)
            {
                string str = Request["strCKDCPMC"].ToString();
                if (!String.IsNullOrEmpty(str))
                {
                    cpId = Convert.ToInt32(str);
                }
                ViewData["strCKDCPMC"] = str;
            }
            var scId = 0; //生产企业ID
            if (Request["strCKDSCQY"] != null)
            {
                string str = Request["strCKDSCQY"].ToString();
                if (!String.IsNullOrEmpty(str))
                {
                    scId = Convert.ToInt32(str);
                }
                ViewData["strCKDSCQY"] = str;
            }

            //购货企业列表
            T_CusQY cusqy = new T_CusQY();
            ViewBag.CUSQY = new SelectList(T_CusQYDomain.GetInstance().GetAllT_CusQY(cusqy).Where(p => p.CusStatus == 1).ToList(), "CusID", "CusMC");

            T_SupQYModels supmode = new T_SupQYModels();
            supmode.DataModel = supmode.DataModel ?? new T_SupQY();
            supmode.DataList = T_SupQYDomain.GetInstance().GetAllT_SupQY(supmode.DataModel).Where(p => p.SupStatus == 1).ToList();
            ViewData["SupQYList"] = new SelectList(supmode.DataList, "SupID", "SupMC");
            T_YLCPModels ylcpQymode = new T_YLCPModels();
            ylcpQymode.DataModel = ylcpQymode.DataModel ?? new T_YLCP();
            ylcpQymode.DataList = T_YLCPDomain.GetInstance().GetAllT_YLCP(ylcpQymode.DataModel).Where(p => p.CPStatus == 1).ToList();
            ViewData["YLCP"] = new SelectList(ylcpQymode.DataList, "CPID", "CPMC");

            evalModel.DataList = T_CKDDomain.GetInstance().PageT_CKD(evalModel.DataModel, evalModel.StartTime, evalModel.EndTime, currentPage, pagesize, cpId, scId, ghId, out pagecount, out resultCount);
            evalModel.resultCount = resultCount;
            return View("~/Views/T_CKD/Index.cshtml", evalModel);
        }

        [CheckLogin()]
        public ActionResult Save(System.Int32 id, string tag)
        {
            CurUser = Session["UserModel"] as SysUser;
            T_CKDModels model = new T_CKDModels();
            //加载销售单列表
            T_XSDModels xsdmode = new T_XSDModels();
            xsdmode.DataModel = xsdmode.DataModel ?? new T_XSD();
            xsdmode.DataList = T_XSDDomain.GetInstance().GetAllT_XSD(xsdmode.DataModel).Where(p => p.XSFLAG == Convert.ToInt32("1")).ToList();
            ViewData["XSD"] = new SelectList(xsdmode.DataList, "XSID", "XSDH");

            //加载仓库列表
            T_CKModels ckmode = new T_CKModels();
            ckmode.DataModel = ckmode.DataModel ?? new T_CK();
            ckmode.DataList = T_CKDomain.GetInstance().GetAllT_CK(ckmode.DataModel);
            ViewData["CK"] = new SelectList(ckmode.DataList, "CKID", "CKMC");

            //加载产品列表
            T_YLCPModels ylcpQymode = new T_YLCPModels();
            ylcpQymode.DataModel = ylcpQymode.DataModel ?? new T_YLCP();
            ylcpQymode.DataList = T_YLCPDomain.GetInstance().GetAllT_YLCP(ylcpQymode.DataModel).Where(p => p.CPStatus == Convert.ToInt32("1")).ToList();
            ViewData["YLCP"] = new SelectList(ylcpQymode.DataList, "CPID", "CPMC");

            //获取本企业下的人员列表
            T_Person person = new T_Person();
            person.PsQYID = (int)CurUser.UserCompanyID;
            ViewBag.Persons = new SelectList(T_PersonDomain.GetInstance().GetAllT_Person(person), "PsMZ", "PsMZ");

            model.DataModel = new T_CKD();
            if (id != 0)
            {
                model.DataModel = T_CKDDomain.GetInstance().GetModelById(id);
                model.CKMXList = T_CKMXDomain.GetInstance().GetT_CKMXByCkid(id);
                if (model.DataModel.XSID != null && model.DataModel.XSID != 0)
                {
                    var temp = T_XSDDomain.GetInstance().GetModelById(model.DataModel.XSID);
                    if (temp != null)
                        model.XSDH = temp.XSDH;
                }
            }
            else
            {
                model.DataModel.CKDH = T_CKDDomain.GetInstance().GetCkOrderNum(CurUser);
                model.DataModel.CKCJR = CurUser.UserAccount;
                model.DataModel.CKCJRQ = DateTime.Now;
            }
            model.Tag = tag;
            model.RoleCode = GetRoleCode();
            return View("~/Views/T_CKD/Save.cshtml", model);
        }
        private string GetRoleCode()
        {
            return Session["RoleCode"] == null ? "" : Session["RoleCode"].ToString();
        }

        [CheckLogin()]
        public ActionResult Details(System.Int32 id)
        {
            T_CKDModels model = new T_CKDModels();
            model.DataModel = new T_CKD();
            model.DataModel.CKID = id;
            var str = ExportExcelPR(id);
            var str2 = ExportExcelPR2(id);
            ViewData["ParaStr"] = str.Replace("\r\n", "<br />");
            ViewData["ParaStr2"] = str2.Replace("\r\n", "<br />");
            return View("~/Views/T_CKD/Details.cshtml", model);
        }

        [HttpPost]
        [CheckLogin()]
        public void Save(T_CKDModels model)
        {
            int result = 0;
            try
            {
                if (model.Tag == "Add")
                {
                    result = T_CKDDomain.GetInstance().AddModel(model.DataModel);
                }
                else if (model.Tag == "Edit")
                {
                    result = T_CKDDomain.GetInstance().UpdateModel(model.DataModel, model.DataModel.CKID);
                }
                if (model.DataModel.IsFinish == 1 && model.DataModel.XSID != 0)
                {
                    T_XSDDomain.GetInstance().UpdateFinish((int)model.DataModel.XSID);
                }
            }
            catch { }
            Response.ContentType = "text/json";
            if (result > 0)
                Response.Write("{\"statusCode\":\"200\", \"message\":\"操作成功\",\"callbackType\":\"closeCurrentReloadTab\",\"forwardUrl\":\"/T_CKD/Index\"}");
            else
                Response.Write("{\"statusCode\":\"300\", \"message\":\"操作失败\"}");
        }

        [HttpPost]
        [CheckLogin()]
        public void Delete(System.Int32 id)
        {
            var temp = T_CKDDomain.GetInstance().GetModelById(id);
            if (temp != null)
            {
                var lstMX = T_CKMXDomain.GetInstance().GetT_CKMXByCkid(temp.CKID);
                if (lstMX != null && lstMX.Count > 0)
                {
                    Response.Write("{\"statusCode\":\"300\", \"message\":\"该数据不能删除！\"}");
                    return;
                }
            }
            int result = T_CKDDomain.GetInstance().Delete(id);
            Response.ContentType = "text/json";
            if (result > 0)
            {
                Response.Write("{\"statusCode\":\"200\", \"message\":\"操作成功\",\"callbackType\":\"forward\",\"forwardUrl\":\"/T_CKD/Index\"}");
            }
            else
                Response.Write("{\"statusCode\":\"300\", \"message\":\"操作失败\"}");
        }

        #region 导出excel及打印功能

        [CheckLogin()]
        public void ExportExcel(System.Int32 id, System.Int32 isdis = 0)
        {
            var str = "";
            if (isdis == 0)
                str = ExportExcelPR(id);
            else
                str = ExportExcelPR2(id);
            var strStyle = @"<html><head><style type='text/css'>table tr td {
                font-family: 宋体;
                line-height: 15px;
                border:thin solid black;
                border-right: 0px;
                border-bottom: 0px;
                font-size: 13px;
            }</style></head><body>";
            StringBuilder builder = new StringBuilder();
            builder.Append(strStyle);
            builder.Append(str);
            builder.Append("</body></html>");
            //调用输出Excel表的方法
            ExportToExcel("application/vnd.ms-excel", "销售复核出库单.xls", builder.ToString());
        }

        [CheckLogin()]
        public void ExportToExcel(string FileType, string FileName, string ExcelContent)
        {
            System.Web.HttpContext.Current.Response.Charset = "UTF-8";
            System.Web.HttpContext.Current.Response.ContentEncoding = System.Text.Encoding.GetEncoding("UTF-8");
            System.Web.HttpContext.Current.Response.AppendHeader("Content-Disposition", "attachment;filename=" + HttpUtility.UrlEncode(FileName, System.Text.Encoding.GetEncoding("UTF-8")).ToString());
            System.Web.HttpContext.Current.Response.ContentType = FileType;
            System.IO.StringWriter tw = new System.IO.StringWriter();
            System.Web.HttpContext.Current.Response.Output.Write(ExcelContent.ToString());
            System.Web.HttpContext.Current.Response.Flush();
            System.Web.HttpContext.Current.Response.End();
        }

        [HttpPost]
        [CheckLogin()]
        public string ExportExcelPR(System.Int32 id)
        {
            //获取需要打印的数据
            T_CKD ckdinfo = new T_CKD();

            List<T_CKD> list = T_CKDDomain.GetInstance().GetListModelById(id);
            if (list.Count > 0)
            {
                ckdinfo = list[0];
            }

            int xsqyid = 0;
            if (ckdinfo.T_XSD != null && ckdinfo.T_XSD.KHID.HasValue)
            {
                xsqyid = ckdinfo.T_XSD.KHID.Value;
            }

            T_CusQY cusqy = T_CusQYDomain.GetInstance().GetModelById(xsqyid);
            //购货企业名称
            string xsqyName = (cusqy != null && !string.IsNullOrEmpty(cusqy.CusMC)) ? cusqy.CusMC : "";
            string xsqyKFDZ = (cusqy != null) ? cusqy.CusKFDZ : ""; //库房地址

            List<T_CKMX> ckmxList = T_CKMXDomain.GetInstance().GetListModelById(id);
            T_WhsQY qy = new T_WhsQY();
            Expression<Func<T_WhsQY, bool>> where = PredicateBuilder.True<T_WhsQY>();
            var lst = T_WhsQYDomain.GetInstance().GetAllModels<int>(where);
            if (lst != null && lst.Count > 0)
            {
                qy = lst[0];
            }
            //命名导出表格的StringBuilder变量
            StringBuilder sHtml = new StringBuilder(string.Empty);
            //打印表头
            sHtml.Append("<table style='border-collapse:collapse;' rull='all'>");
            sHtml.Append("<tr height=\"40\"><td colspan=\"14\" align=\"center\" style='font-size:24px;border:0px;'>" + qy.WhsMC + "销售复核出库单" + "</td></tr>");
            sHtml.Append("<tr height=\"40\"><td colspan=\"6\" align=\"left\" style='border:0px;'>购货单位：" + xsqyName + "</td><td align=\"left\"  colspan=\"4\" style='border:0px;'>&nbsp;&nbsp;日 期：" + DateTime.Now.ToString("yyyy-MM-dd") + "</td><td align=\"center\"  colspan=\"4\" style='border:0px;'>单据编号：" + DateTime.Now.ToString("yyyyMMddHHmmss") + "</td></tr>");
            sHtml.Append("<tr><td colspan=\"14\" height=\"30\" style='border:0px;'>地址：" + xsqyKFDZ + "</td></tr>");
            sHtml.Append("<tr align=\"center\" ><td style='width: 80px;' >产品名称</td><td style='width: 40px;' >规格</td><td style='width: 40px;' >型号</td><td style='width:98px;' >生产企业</td><td style='width: 35px;' >单位</td><td style='width: 35px;' >数量</td><td style='width: 40px;' >单价</td>"
                + "<td style='width: 60px;' >金额</td><td style='width: 70px;' >产品批号</td><td style='width: 65px;'>生产日期\r\n--------\r\n失效日期</td><td style='width: 65px;' >生产/经营\r\n许可证号</td><td style='width: 60px;' >注册证号</td><td style='width: 35px;' >储运条件</td><td style='width: 32px;border-right: thin solid black;' >备注</td></tr>");

            //合计
            double total = 0.0;
            double NumCpNum = 0.0;
            for (int i = 0; i < ckmxList.Count; i++)
            {
                T_CKMX ckmx = ckmxList[i];
                //产品名称
                string cpName = ckmx.T_YLCP.CPMC;
                //规格
                string cpGg = ckmx.T_YLCP.CPGG ?? "";
                var cpxh = ckmx.T_YLCP.CPXH ?? "";
                //单位
                string cpDw = ckmx.T_YLCP.CPDW ?? "";
                //数量
                double cpDj = ckmx.CPNUM ?? 0;
                //产品批号
                string scPh = ckmx.CPPH ?? "";
                //产品有效期
                string scRq = "";
                if (ckmx.CPYXQ != null)
                {
                    scRq = ckmx.CPYXQ.Value.ToString("yyyyMMdd");
                }
                var scrq = "";
                if (ckmx.CPSCRQ != null)
                {
                    scrq = ckmx.CPSCRQ.Value.ToString("yyyyMMdd");
                }
                //生产企业
                string cpScqy = "";
                if (ckmx.T_YLCP != null && ckmx.T_YLCP.T_SupQY1 != null && !string.IsNullOrEmpty(ckmx.T_YLCP.T_SupQY1.SupMC))
                {
                    cpScqy = ckmx.T_YLCP.T_SupQY1.SupMC;
                }
                //单价
                double cpPrice = ckmx.CPPRICE ?? 0.0;
                //产品总价
                double rowTotal = cpDj * cpPrice;

                total = total + rowTotal;
                NumCpNum = NumCpNum + cpDj;
                //经营许可证号
                string xkzbh = "";
                if (ckmx.T_YLCP != null && ckmx.T_YLCP.T_SupQY1 != null && !string.IsNullOrEmpty(ckmx.T_YLCP.T_SupQY1.SupXKZBH))
                {
                    xkzbh = ckmx.T_YLCP.T_SupQY1.SupXKZBH;
                }
                //注册证号
                string cpzczh = ckmx.T_YLCP.CPZCZ;
                var cytj = ckmx.CYTJ;
                sHtml.Append("<tr align=\"center\"><td >" + cpName
                            + "</td><td style='vnd.ms-excel.numberformat:@' >" + cpGg + "</td><td style='vnd.ms-excel.numberformat:@' >" + cpxh + "</td><td >" + cpScqy
                            + "</td><td >" + cpDw + "</td><td style='vnd.ms-excel.numberformat:@' >" + cpDj.ToString()
                            + "</td><td style='vnd.ms-excel.numberformat:@' >" + cpPrice.ToString("0.00") + "</td><td style='vnd.ms-excel.numberformat:@'  >" + rowTotal.ToString("0.00") + "</td><td  style='vnd.ms-excel.numberformat:@' >" + scPh
                            + "</td><td style='vnd.ms-excel.numberformat:@'>" + scrq + "\r\n" + "--------" + "\r\n" + scRq + " </td>"
                            + "<td style='vnd.ms-excel.numberformat:@' >" + xkzbh
                            + "</td><td style='vnd.ms-excel.numberformat:@' >" + cpzczh
                            + "</td><td >" + cytj
                            + "</td><td style='border-right: thin solid black;' ></td></tr>");
            }
            //打印表尾
            sHtml.Append("<tr id='trtotal' height=\"40\" align=\"center\"><td colspan=\"9\" style='border-bottom: thin solid black;font-size:16px;'>合计金额：（大写）" + RmbHelper.CmycurD(total.ToString()) + "</td><td colspan=\"5\" style='border-bottom: thin solid black;border-right: thin solid black;font-size:16px;'>（小写）" + total.ToString("0.00") + "</td></tr>");
            var xsry = ckdinfo.T_XSD == null ? "" : ckdinfo.T_XSD.XSRY;
            sHtml.Append("<tr height=\"40\" align=\"center\"><td colspan=\"3\" align=\"left\" style='border:0px;'>销售员：&nbsp;" + xsry + "</td><td align=\"center\" colspan=\"4\" style='border:0px;'>复核员：&nbsp;" + ckdinfo.FHR + "</td ><td align =\"center\" colspan=\"4\" style='border:0px;'>出库员：&nbsp;" + ckdinfo.CKCHR + "</td ><td align =\"center\" colspan=\"3\" style='border:0px;'>收货人：&nbsp;</td></tr>");
            sHtml.Append("<tr><td colspan=\"14\" style='border:0px;'>公司地址：" + qy.WhsZCDZ + "</td></tr>");
            sHtml.Append("</table>");
            return sHtml.ToString();
        }

        public string ExportExcelPR2(System.Int32 id)
        {
            //获取需要打印的数据
            T_CKD ckdinfo = new T_CKD();

            List<T_CKD> list = T_CKDDomain.GetInstance().GetListModelById(id);
            if (list.Count > 0)
            {
                ckdinfo = list[0];
            }

            int xsqyid = 0;
            if (ckdinfo.T_XSD != null && ckdinfo.T_XSD.KHID.HasValue)
            {
                xsqyid = ckdinfo.T_XSD.KHID.Value;
            }

            T_CusQY cusqy = T_CusQYDomain.GetInstance().GetModelById(xsqyid);
            //购货企业名称
            string xsqyName = (cusqy != null && !string.IsNullOrEmpty(cusqy.CusMC)) ? cusqy.CusMC : "";
            string xsqyKFDZ = (cusqy != null) ? cusqy.CusKFDZ : ""; //库房地址

            List<T_CKMX> ckmxList = T_CKMXDomain.GetInstance().GetListModelById(id);
            T_WhsQY qy = new T_WhsQY();
            Expression<Func<T_WhsQY, bool>> where = PredicateBuilder.True<T_WhsQY>();
            var lst = T_WhsQYDomain.GetInstance().GetAllModels<int>(where);
            if (lst != null && lst.Count > 0)
            {
                qy = lst[0];
            }
            //命名导出表格的StringBuilder变量
            StringBuilder sHtml = new StringBuilder(string.Empty);
            //打印表头
            sHtml.Append("<table style='border-collapse:collapse;' rull='all'>");
            sHtml.Append("<tr height=\"40\"><td colspan=\"12\" align=\"center\" style='font-size:24px;border:0px;'>" + qy.WhsMC + "销售复核出库单" + "</td></tr>");
            sHtml.Append("<tr height=\"40\"><td colspan=\"5\" align=\"left\" style='border:0px;'>购货单位：" + xsqyName + "</td><td align=\"left\"  colspan=\"3\" style='border:0px;'>日 期：" + DateTime.Now.ToString("yyyy-MM-dd") + "</td><td align=\"center\"  colspan=\"4\" style='border:0px;'>单据编号：" + DateTime.Now.ToString("yyyyMMddHHmmss") + "</td></tr>");
            sHtml.Append("<tr><td colspan=\"12\" height=\"30\" style='border:0px;'>地址：" + xsqyKFDZ + "</td></tr>");
            sHtml.Append("<tr align=\"center\" ><td style='width: 100px;' >产品名称</td><td style='width: 40px;' >规格</td><td style='width: 40px;' >型号</td><td style='width:128px;' >生产企业</td><td style='width: 35px;' >单位</td><td style='width: 35px;' >数量</td><td style='width: 70px;' >产品批号</td><td style='width: 65px;'>生产日期\r\n--------\r\n失效日期</td><td style='width: 65px;' >生产/经营\r\n许可证号</td><td style='width: 60px;' >注册证号</td><td style='width: 35px;' >储运条件</td><td style='width: 32px;border-right: thin solid black;' >备注</td></tr>");

            //合计
            double total = 0.0;
            double NumCpNum = 0.0;
            for (int i = 0; i < ckmxList.Count; i++)
            {
                T_CKMX ckmx = ckmxList[i];
                //产品名称
                string cpName = ckmx.T_YLCP.CPMC;
                //规格
                string cpGg = ckmx.T_YLCP.CPGG ?? "";
                var cpxh = ckmx.T_YLCP.CPXH ?? "";
                //单位
                string cpDw = ckmx.T_YLCP.CPDW ?? "";
                //数量
                double cpDj = ckmx.CPNUM ?? 0;
                //产品批号
                string scPh = ckmx.CPPH ?? "";
                //产品有效期
                string scRq = "";
                if (ckmx.CPYXQ != null)
                {
                    scRq = ckmx.CPYXQ.Value.ToString("yyyyMMdd");
                }
                var scrq = "";
                if (ckmx.CPSCRQ != null)
                {
                    scrq = ckmx.CPSCRQ.Value.ToString("yyyyMMdd");
                }
                //生产企业
                string cpScqy = "";
                if (ckmx.T_YLCP != null && ckmx.T_YLCP.T_SupQY1 != null && !string.IsNullOrEmpty(ckmx.T_YLCP.T_SupQY1.SupMC))
                {
                    cpScqy = ckmx.T_YLCP.T_SupQY1.SupMC;
                }
                //单价
                double cpPrice = ckmx.CPPRICE ?? 0.0;
                //产品总价
                double rowTotal = cpDj * cpPrice;

                total = total + rowTotal;
                NumCpNum = NumCpNum + cpDj;
                //经营许可证号
                string xkzbh = "";
                if (ckmx.T_YLCP != null && ckmx.T_YLCP.T_SupQY1 != null && !string.IsNullOrEmpty(ckmx.T_YLCP.T_SupQY1.SupXKZBH))
                {
                    xkzbh = ckmx.T_YLCP.T_SupQY1.SupXKZBH;
                }
                //注册证号
                string cpzczh = ckmx.T_YLCP.CPZCZ;
                var cytj = ckmx.CYTJ;
                sHtml.Append("<tr align=\"center\"><td >" + cpName
                            + "</td><td style='vnd.ms-excel.numberformat:@' >" + cpGg + "</td><td style='vnd.ms-excel.numberformat:@' >" + cpxh + "</td><td >" + cpScqy
                            + "</td><td >" + cpDw + "</td><td style='vnd.ms-excel.numberformat:@'  >" + rowTotal.ToString("0.00") + "</td><td  style='vnd.ms-excel.numberformat:@' >" + scPh
                            + "</td><td style='vnd.ms-excel.numberformat:@'>" + scrq + "\r\n" + "--------" + "\r\n" + scRq + " </td>"
                            + "<td style='vnd.ms-excel.numberformat:@' >" + xkzbh
                            + "</td><td style='vnd.ms-excel.numberformat:@' >" + cpzczh
                            + "</td><td >" + cytj
                            + "</td><td style='border-right: thin solid black;' ></td></tr>");
            }
            //打印表尾
            sHtml.Append("<tr id='trtotal' height=\"40\" align=\"center\"><td colspan=\"7\" style='border-bottom: thin solid black;font-size:16px;'>合计金额：（大写）" + RmbHelper.CmycurD(total.ToString()) + "</td><td colspan=\"5\" style='border-bottom: thin solid black;border-right: thin solid black;font-size:16px;'>（小写）" + total.ToString("0.00") + "</td></tr>");
            var xsry = ckdinfo.T_XSD == null ? "" : ckdinfo.T_XSD.XSRY;
            sHtml.Append("<tr height=\"40\" align=\"center\"><td colspan=\"3\" align=\"left\" style='border:0px;'>销售员：&nbsp;" + xsry + "</td><td align=\"center\" colspan=\"3\" style='border:0px;'>复核员：&nbsp;" + ckdinfo.FHR + "</td ><td align =\"center\" colspan=\"3\" style='border:0px;'>出库员：&nbsp;" + ckdinfo.CKCHR + "</td ><td align =\"center\" colspan=\"3\" style='border:0px;'>收货人：&nbsp;</td></tr>");
            sHtml.Append("<tr><td colspan=\"12\" style='border:0px;'>公司地址：" + qy.WhsZCDZ + "</td></tr>");
            sHtml.Append("</table>");
            return sHtml.ToString();
        }

        /// <summary>
        /// 将产品金额小写转换成大写
        /// </summary>
        /// <param name="par">小写金额</param>
        /// <returns>处理后的大写金额</returns>
        public static string MoneySmallToBig(string par)
        {
            String[] Scale = { "分", "角", "元", "拾", "佰", "仟", "万", "拾", "佰", "仟", "亿", "拾", "佰", "仟", "兆", "拾", "佰", "仟" };
            String[] Base = { "零", "壹", "贰", "叁", "肆", "伍", "陆", "柒", "捌", "玖" };
            String Temp = par;
            string result = null;
            int index = Temp.IndexOf(".", 0, Temp.Length);//判断是否有小数点
            if (index != -1)
            {
                Temp = Temp.Remove(Temp.IndexOf("."), 1);
                for (int i = Temp.Length; i > 0; i--)
                {
                    int Data = Convert.ToInt16(Temp[Temp.Length - i]);
                    result += Base[Data - 48];
                    result += Scale[i - 1];
                }
            }
            else
            {
                for (int i = Temp.Length; i > 0; i--)
                {
                    int Data = Convert.ToInt16(Temp[Temp.Length - i]);
                    result += Base[Data - 48];
                    result += Scale[i + 1];
                }
            }
            return result;
        }
        #endregion
    }
}
