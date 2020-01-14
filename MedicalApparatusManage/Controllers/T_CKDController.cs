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

            if (Request["strCKDName"] != null)
            {
                string str = Request["strCKDName"].ToString();
                if (!String.IsNullOrEmpty(str))
                {
                    evalModel.DataModel.CKDH = str.Trim();
                }
                ViewData["strCKDName"] = str;
            }

            evalModel.DataList = T_CKDDomain.GetInstance().PageT_CKD(evalModel.DataModel, evalModel.StartTime, evalModel.EndTime, currentPage, pagesize, out pagecount, out resultCount);
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
            ViewData["ParaStr"] = ExportExcelPR(id);
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

            }
            catch { }
            Response.ContentType = "text/json";
            if (result > 0)
                Response.Write("{\"statusCode\":\"200\", \"message\":\"操作成功\",\"callbackType\":\"closeCurrentReloadTab\",\"forwardUrl\":\"/T_CKD/Index\"}");
            else
                Response.Write("{\"statusCode\":\"300\", \"message\":\"操作失败\"}");
        }

        //[CheckLogin()]
        //public void ExportExcel()
        //{
        //    Int32 xsdid = 12;
        //    T_XSD pxsd = T_XSDDomain.GetInstance().GetModelById(xsdid);
        //    string qyMC = pxsd.T_CusQY.CusMC;





        //    StringBuilder sb = new StringBuilder();
        //    sb.Append("出库单名称").Append("\t");
        //    sb.Append("出库申请人").Append("\t");
        //    sb.Append("申请日期").Append("\t");
        //    sb.Append("复核人").Append("\t");
        //    sb.Append("复核日期").Append("\t");
        //    sb.Append("仓库出货人").Append("\t");
        //    sb.Append("出货日期").Append("\t");
        //    sb.Append("备注").Append("\n"); 
        //    T_CKD ckdinfo = new T_CKD();
        //    List<T_CKD> ckdlist = T_CKDDomain.GetInstance().GetAllT_CKD(ckdinfo);
        //    foreach (var item in ckdlist)
        //    {
        //        sb.Append(item.CKMC).Append("\t");
        //        sb.Append(item.CKSQR).Append("\t");
        //        sb.Append(item.SQRQ).Append("\t");
        //        sb.Append(item.FHR).Append("\t");
        //        sb.Append(item.FHRQ).Append("\t");
        //        sb.Append(item.CKCHR).Append("\t");
        //        sb.Append(item.CHRQ).Append("\t");
        //        sb.Append(item.CHBZ).Append("\n");
        //    }
        //    Response.Clear();
        //    Response.Buffer = true;
        //    Response.Charset = "GB2312";
        //    Response.ContentEncoding = Encoding.Default;
        //    Response.ContentType = "application/ms-excel";
        //    Response.AppendHeader("Content-Disposition", "attachment; filename=card.xls");
        //    //加上这句　             
        //    Response.Write(sb.ToString());
        //    Response.End();
        //}
        [HttpPost]
        [CheckLogin()]
        public void Delete(System.Int32 id)
        {
            //var rCode = GetRoleCode();
            var temp = T_CKDDomain.GetInstance().GetModelById(id);
            if (temp != null)
            {
                Response.Write("{\"statusCode\":\"300\", \"message\":\"该数据不能删除！\"}");
                return;
            }
            int result = T_CKDDomain.GetInstance().Delete(id);
            Response.ContentType = "text/json";
            if (result > 0)
                Response.Write("{\"statusCode\":\"200\", \"message\":\"操作成功\",\"callbackType\":\"forward\",\"forwardUrl\":\"/T_CKD/Index\"}");
            else
                Response.Write("{\"statusCode\":\"300\", \"message\":\"操作失败\"}");
        }

        #region 导出excel及打印功能

        [CheckLogin()]
        public void ExportExcel(System.Int32 id)
        {
            #region 
            //接收需要导出的数据
            //T_CKD ckdinfo = new T_CKD();

            //List<T_CKD> list = T_CKDDomain.GetInstance().GetListModelById(id);
            //if (list.Count > 0)
            //{
            //    ckdinfo = list[0];
            //}

            //int xsqyid = 0;
            //if (ckdinfo.T_XSD != null && ckdinfo.T_XSD.KHID.HasValue)
            //{
            //    xsqyid = ckdinfo.T_XSD.KHID.Value;
            //}

            //T_CusQY cusqy = T_CusQYDomain.GetInstance().GetModelById(xsqyid);
            ////购货企业名称
            ////购货企业名称
            //string xsqyName = (cusqy != null && !string.IsNullOrEmpty(cusqy.CusMC)) ? cusqy.CusMC : "";
            //string xsqyKFDZ = (cusqy != null) ? cusqy.CusKFDZ : ""; //库房地址

            //List<T_CKMX> ckmxList = T_CKMXDomain.GetInstance().GetListModelById(id);
            //T_WhsQY qy = new T_WhsQY();
            //Expression<Func<T_WhsQY, bool>> where = PredicateBuilder.True<T_WhsQY>();
            //var lst = T_WhsQYDomain.GetInstance().GetAllModels<int>(where);
            //if (lst != null && lst.Count > 0)
            //{
            //    qy = lst[0];
            //}

            ////命名导出表格的StringBuilder变量
            //StringBuilder sHtml = new StringBuilder(string.Empty);
            ////打印表头
            //sHtml.Append("<table border=\"0\" width=\"100%\">");
            //sHtml.Append("<tr height=\"40\"><td colspan=\"10\" align=\"center\" style='font-size:24px'><b>" + qy.WhsMC + "销售复核出库单" + "</b></td></tr>");
            //sHtml.Append("<tr height=\"40\"><td colspan=\"8\" align=\"left\">购货单位：" + xsqyName + "</td><td align=\"right\">日 期：" + DateTime.Now.ToString("yyyy-MM-dd") + "</td><td align=\"right\">单据编号：" + DateTime.Now.ToString("yyyyMMddHHmmssffff") + "</td></tr>");
            //sHtml.Append("<tr><td colspan=\"10\">地址：" + xsqyKFDZ + "<td></tr>");
            //sHtml.Append("</table>");
            //sHtml.Append("<table border=\"1\" width=\"100%\">");
            ////sHtml.Append("<tr height=\"40\"><td colspan=\"10\" align=\"center\" style='font-size:24px'><b>出库单" + "</b></td></tr>");
            ////sHtml.Append("<tr height=\"40\"><td colspan=\"8\" align=\"left\">&nbsp;购买单位：" + xsqyName + "</td><td align=\"right\">日期：" + DateTime.Now.ToString("yyyy-MM-dd") + "</td><td align=\"right\">单据编号：" + DateTime.Now.ToString("yyyyMMddHHmmssffff") + "</td></tr>");
            ////打印列名
            //sHtml.Append("<tr height=\"30\" align=\"center\" ><td>产品名称</td><td>产品规格（型号）</td><td>生产企业</td><td>生产日期</td><td>单位</td><td>数量</td><td>单价</td>"
            //    + "<td>金额</td><td>产品批号</td><td>产品有效期</td><td>经营许可证号</td><td>注册证号</td><td>储运条件</td></tr>");

            ////合计
            //double total = 0.0;
            //double NumCpNum = 0.0;
            //for (int i = 0; i < ckmxList.Count; i++)
            //{
            //    T_CKMX ckmx = ckmxList[i];
            //    //产品名称
            //    string cpName = ckmx.T_YLCP.CPMC;
            //    //规格
            //    string cpGg = ckmx.T_YLCP.CPGG ?? "";
            //    //单位
            //    string cpDw = ckmx.T_YLCP.CPDW ?? "";
            //    //数量
            //    double cpDj = ckmx.CPNUM ?? 0;
            //    //产品批号
            //    string scPh = ckmx.CPPH ?? "";
            //    //产品有效期
            //    string scRq = "";
            //    if (ckmx.CPYXQ != null)
            //    {
            //        scRq = ckmx.CPYXQ.Value.ToString("yyyyMMdd");
            //    }
            //    var scrq = "";
            //    if (ckmx.CPSCRQ != null)
            //    {
            //        scrq = ckmx.CPSCRQ.Value.ToString("yyyyMMdd");
            //    }
            //    //生产企业
            //    string cpScqy = "";
            //    if (ckmx.T_YLCP != null && ckmx.T_YLCP.T_SupQY1 != null && !string.IsNullOrEmpty(ckmx.T_YLCP.T_SupQY1.SupMC))
            //    {
            //        cpScqy = ckmx.T_YLCP.T_SupQY1.SupMC;
            //    }
            //    //单价
            //    double cpPrice = ckmx.T_YLCP.XSJG ?? 0.0;
            //    //产品总价
            //    double rowTotal = cpDj * cpPrice;

            //    total = total + rowTotal;
            //    NumCpNum = NumCpNum + cpDj;
            //    //经营许可证号
            //    string xkzbh = "";
            //    if (ckmx.T_YLCP != null && ckmx.T_YLCP.T_SupQY1 != null && !string.IsNullOrEmpty(ckmx.T_YLCP.T_SupQY1.SupXKZBH))
            //    {
            //        xkzbh = ckmx.T_YLCP.T_SupQY1.SupXKZBH;
            //    }
            //    //注册证号
            //    string cpzczh = ckmx.T_YLCP.CPZCZ;
            //    var cytj = ckmx.CYTJ;
            //    sHtml.Append("<tr height=\"30\" align=\"center\"><td>" + cpName
            //                + "</td><td>" + cpGg + "</td><td>" + cpScqy
            //                 + "</td><td>" + scrq
            //                + "</td><td>" + cpDw + "</td><td>" + cpDj.ToString()
            //                + "</td><td>" + cpPrice.ToString("0.000") + "</td><td>" + rowTotal.ToString("0.000") + "</td><td>" + scPh + "</td><td>" + scRq
            //                + "</td><td>" + xkzbh
            //                + "</td><td>" + cpzczh
            //                + "</td><td>" + cytj
            //                + "</td></tr>");
            //}
            ////打印表尾
            //sHtml.Append("<tr height=\"40\" align=\"center\"><td colspan=\"5\">合计金额：（大写）" + MoneySmallToBig(total.ToString()) + "</td><td colspan=\"8\">（小写）" + total.ToString("0.000") + "</td></tr>");
            //sHtml.Append("</table>");
            //sHtml.Append("<table  border=\"0\" width=\"100%\">");
            //sHtml.Append("<tr height=\"40\" align=\"center\"><td colspan=\"8\" align=\"left\">销售员：&nbsp;&nbsp</td><td align=\"right\">复核员：&nbsp;&nbsp</td><td align=\"right\">出库员：&nbsp;&nbsp</td><td align=\"center\">收货员：&nbsp;&nbsp</td></tr>");
            //sHtml.Append("</table>");
            #endregion
            var str = ExportExcelPR(id);
            //调用输出Excel表的方法
            ExportToExcel("application/vnd.ms-excel", "销售复核出库单.xls", str);
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
            sHtml.Append("<table border=\"0\" width=\"100%\">");
            sHtml.Append("<tr height=\"40\"><td colspan=\"13\" align=\"center\" style='font-size:24px'><b>" + qy.WhsMC + "销售复核出库单" + "</b></td></tr>");
            sHtml.Append("<tr height=\"40\"><td colspan=\"6\" align=\"left\">购货单位：" + xsqyName + "</td><td align=\"center\"  colspan=\"4\">日 期：" + DateTime.Now.ToString("yyyy-MM-dd") + "</td><td align=\"right\"  colspan=\"3\">单据编号：" + DateTime.Now.ToString("yyyyMMddHHmmssffff") + "</td></tr>");
            sHtml.Append("<tr><td colspan=\"10\" height=\"30\">地址：" + xsqyKFDZ + "<td></tr>");
            sHtml.Append("</table>");
            sHtml.Append("<table border=\"1\" width=\"100%\" style='border-collapse:collapse;border:1px solid black;'>");
            //sHtml.Append("<tr height=\"40\"><td colspan=\"10\" align=\"center\" style='font-size:24px'><b>出库单" + "</b></td></tr>");
            //sHtml.Append("<tr height=\"40\"><td colspan=\"8\" align=\"left\">&nbsp;购买单位：" + xsqyName + "</td><td align=\"right\">日期：" + DateTime.Now.ToString("yyyy-MM-dd") + "</td><td align=\"right\">单据编号：" + DateTime.Now.ToString("yyyyMMddHHmmssffff") + "</td></tr>");
            //打印列名
            sHtml.Append("<tr height=\"30\" align=\"center\" ><td>产品名称</td><td>产品规格（型号）</td><td>生产企业</td><td>生产日期</td><td>单位</td><td>数量</td><td>单价</td>"
                + "<td>金额</td><td>产品批号</td><td>产品有效期</td><td>经营许可证号</td><td>注册证号</td><td>储运条件</td></tr>");

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
                double cpPrice = ckmx.T_YLCP.XSJG ?? 0.0;
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
                sHtml.Append("<tr height=\"30\" align=\"center\"><td>" + cpName
                            + "</td><td>" + cpGg + "</td><td>" + cpScqy
                             + "</td><td>" + scrq
                            + "</td><td>" + cpDw + "</td><td>" + cpDj.ToString()
                            + "</td><td>" + cpPrice.ToString("0.000") + "</td><td>" + rowTotal.ToString("0.000") + "</td><td>" + scPh + "</td><td>" + scRq
                            + "</td><td>" + xkzbh
                            + "</td><td>" + cpzczh
                            + "</td><td>" + cytj
                            + "</td></tr>");
            }
            //打印表尾
            sHtml.Append("<tr height=\"40\" align=\"center\"><td colspan=\"5\">合计金额：（大写）" + MoneySmallToBig(total.ToString()) + "</td><td colspan=\"8\">（小写）" + total.ToString("0.000") + "</td></tr>");
            sHtml.Append("</table>");
            sHtml.Append("<table  border=\"0\" width=\"100%\">");
            var xsry = ckdinfo.T_XSD == null ? "" : ckdinfo.T_XSD.XSRY;
            sHtml.Append("<tr height=\"40\" align=\"center\"><td colspan=\"2\" align=\"left\">销售员：&nbsp;" + xsry + "</td><td align=\"left\" colspan=\"4\">复核员：&nbsp;" + ckdinfo.FHR + "</td ><td align =\"left\" colspan=\"4\">出库员：&nbsp;" + ckdinfo.CKCHR + "</td ><td align =\"center\" colspan=\"3\">收货员：&nbsp;</td></tr>");
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
