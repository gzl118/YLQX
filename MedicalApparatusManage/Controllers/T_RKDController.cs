using MedicalApparatusManage.Common;
using MedicalApparatusManage.Domain;
using MedicalApparatusManage.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace MedicalApparatusManage.Controllers
{
    public class T_RKDController : Controller
    {
        public int resultCount = 0; // 总条数 
        public SysUser CurUser = null;

        [CheckLogin()]
        public ActionResult Index(T_RKDModels evalModel)
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
            evalModel.DataModel = evalModel.DataModel ?? new T_RKD();

            var cpId = 0;
            if (Request["strRKCPMC"] != null)
            {
                string str = Request["strRKCPMC"].ToString();
                if (!String.IsNullOrEmpty(str))
                {
                    cpId = Convert.ToInt32(str);
                }
                ViewData["strRKCPMC"] = str;
            }

            string strYSPerson = "请选择";
            if (Request["strYSPerson"] != null)
            {
                strYSPerson = Request["strYSPerson"].ToString();
                if (!String.IsNullOrEmpty(strYSPerson))
                {
                    evalModel.DataModel.CKGLRY = strYSPerson;
                }
            }
            ViewData["strYSPerson"] = strYSPerson;
            var supId = 0;
            if (Request["strCGSupQY"] != null)
            {
                string str = Request["strCGSupQY"].ToString();
                if (!String.IsNullOrEmpty(str))
                {
                    supId = Convert.ToInt32(str);
                }
                ViewData["strCGSupQY"] = str;
            }
            var cusId = 0;
            if (Request["strCusQY"] != null)
            {
                string str = Request["strCusQY"].ToString();
                if (!String.IsNullOrEmpty(str))
                {
                    cusId = Convert.ToInt32(str);
                }
                ViewData["strCusQY"] = str;
            }

            SysUser UserModel = Session["UserModel"] as SysUser;
            T_Person person = new T_Person();
            person.PsQYID = (int)UserModel.UserCompanyID;
            ViewBag.Persons = new SelectList(T_PersonDomain.GetInstance().GetAllT_Person(person), "PsMZ", "PsMZ");

            T_YLCPModels ylcpQymode = new T_YLCPModels();
            ylcpQymode.DataModel = ylcpQymode.DataModel ?? new T_YLCP();
            ylcpQymode.DataList = T_YLCPDomain.GetInstance().GetAllT_YLCP(ylcpQymode.DataModel).Where(p => p.CPStatus == 1).ToList();
            ViewData["YLCP"] = new SelectList(ylcpQymode.DataList, "CPID", "CPMC");
            T_SupQYModels supmode = new T_SupQYModels();
            supmode.DataModel = supmode.DataModel ?? new T_SupQY();
            supmode.DataList = T_SupQYDomain.GetInstance().GetAllT_SupQY(supmode.DataModel).Where(p => p.SupStatus == 1).ToList();
            ViewData["SupQYList"] = new SelectList(supmode.DataList, "SupID", "SupMC");

            evalModel.DataList = T_RKDDomain.GetInstance().PageT_RKD(evalModel.DataModel, evalModel.StartTime, evalModel.EndTime, currentPage, pagesize, cpId, supId, cusId, out pagecount, out resultCount);
            evalModel.resultCount = resultCount;
            return View("~/Views/T_RKD/Index.cshtml", evalModel);
        }

        [CheckLogin()]
        public ActionResult Save(System.Int32 id, string tag)
        {

            //加载验收单
            List<T_YSD> ysdList = new List<T_YSD>();
            ysdList = T_YSDDomain.GetInstance().GetAllT_YSD(new T_YSD()).OrderByDescending(p => p.YSDH).ToList();
            ViewData["YSD"] = new SelectList(ysdList, "YSDH", "YSDH");

            //加载仓库列表
            T_CKModels ckmode = new T_CKModels();
            ckmode.DataModel = ckmode.DataModel ?? new T_CK();
            ckmode.DataList = T_CKDomain.GetInstance().GetAllT_CK(ckmode.DataModel);
            ViewData["CK"] = new SelectList(ckmode.DataList, "CKID", "CKMC");

            //加载企业列表
            T_SupQYModels supmode = new T_SupQYModels();
            supmode.DataModel = supmode.DataModel ?? new T_SupQY();
            supmode.DataList = T_SupQYDomain.GetInstance().GetAllT_SupQY(supmode.DataModel).Where(p => p.SupStatus == 1).ToList();
            ViewData["SupID"] = new SelectList(supmode.DataList, "SupID", "SupMC");


            T_RKDModels model = new T_RKDModels();
            model.DataModel = new T_RKD();
            CurUser = Session["UserModel"] as SysUser;

            if (id != 0)
            {
                model.DataModel = T_RKDDomain.GetInstance().GetModelById(id);
                model.RKMXList = T_RKMXDomain.GetInstance().GetT_RKMXByRkid(id);
            }
            else
            {
                model.DataModel.RKDH = T_RKDDomain.GetInstance().GetRkOrderNum(CurUser);
                model.DataModel.RKCJR = CurUser.UserAccount;
                model.DataModel.RKCJRQ = DateTime.Now;
            }

            //获取本企业下的人员列表
            T_Person person = new T_Person();
            person.PsQYID = (int)CurUser.UserCompanyID;
            ViewBag.Persons = new SelectList(T_PersonDomain.GetInstance().GetAllT_Person(person), "PsMZ", "PsMZ");
            model.Tag = tag;
            model.RoleCode = GetRoleCode();
            return View("~/Views/T_RKD/Save.cshtml", model);
        }

        [HttpPost]
        [CheckLogin()]
        public void Save(T_RKDModels model)
        {
            int result = 0;
            try
            {
                if (model.Tag == "Add")
                {
                    model.DataModel.ISSH = 0;
                    model.DataModel.RKMC = model.DataModel.RKMC + DateTime.Now.ToLongDateString();

                    var temp = T_RKDDomain.GetInstance().GetAllModels<string>(p => p.RKDH == model.DataModel.RKDH).FirstOrDefault();
                    if (temp != null && temp.RKID != 0)
                    {
                        var CurUser1 = Session["UserModel"] as SysUser;
                        model.DataModel.RKDH = T_RKDDomain.GetInstance().GetRkOrderNum(CurUser1);
                    }

                    result = T_RKDDomain.GetInstance().AddModel(model.DataModel);
                }
                else if (model.Tag == "Edit")
                {
                    model.DataModel.ISSH = 0;
                    result = T_RKDDomain.GetInstance().UpdateModel(model.DataModel, model.DataModel.RKID);
                }
            }
            catch { }
            Response.ContentType = "text/json";
            if (result > 0)
                Response.Write("{\"statusCode\":\"200\", \"message\":\"操作成功\",\"callbackType\":\"closeCurrentReloadTab\",\"forwardUrl\":\"/T_RKD/Index\"}");
            else
                Response.Write("{\"statusCode\":\"300\", \"message\":\"操作失败\"}");
        }

        [HttpPost]
        [CheckLogin()]
        public void Delete(System.Int32 id)
        {
            //var rCode = GetRoleCode();
            var temp = T_RKDDomain.GetInstance().GetModelById(id);
            if (temp != null)
            {
                if (temp.ISSH == 1)
                {
                    Response.Write("{\"statusCode\":\"300\", \"message\":\"已审批通过的数据不能删除！\"}");
                    return;
                }
            }
            int result = T_RKDDomain.GetInstance().Delete(id);
            Response.ContentType = "text/json";
            if (result > 0)
                Response.Write("{\"statusCode\":\"200\", \"message\":\"操作成功\",\"callbackType\":\"forward\",\"forwardUrl\":\"/T_RKD/Index\"}");
            else
                Response.Write("{\"statusCode\":\"300\", \"message\":\"操作失败\"}");
        }


        [CheckLogin()]
        public ActionResult Details(System.Int32 id)
        {
            T_RKDModels model = new T_RKDModels();
            model.DataModel = T_RKDDomain.GetInstance().GetModelById(id);
            model.RKMXList = T_RKMXDomain.GetInstance().GetT_RKMXByRkid(id);
            CurUser = Session["UserModel"] as SysUser;
            T_Person person = new T_Person();
            person.PsQYID = (int)CurUser.UserCompanyID;
            List<T_Person> personList = T_PersonDomain.GetInstance().GetAllT_Person(person);

            ////申请人
            //if (!string.IsNullOrEmpty(model.DataModel.SQPE))
            //{
            //    var sqr = personList.Where(p => p.PsID.ToString() == model.DataModel.SQPE).FirstOrDefault();
            //    model.SQRMC = (sqr != null && !string.IsNullOrEmpty(sqr.PsMZ)) ? sqr.PsMZ : "";
            //}
            ////仓库管理人
            //if (!string.IsNullOrEmpty(model.DataModel.CKGLRY))
            //{
            //    var ckr = personList.Where(p => p.PsID.ToString() == model.DataModel.CKGLRY).FirstOrDefault();
            //    model.CKRMC = (ckr != null && !string.IsNullOrEmpty(ckr.PsMZ)) ? ckr.PsMZ : "";
            //}
            ViewData["ParaStr"] = ExportExcelPR(id);
            return View("~/Views/T_RKD/Details.cshtml", model);
        }

        #region excel导出、打印

        [CheckLogin()]
        public void ExportExcel(System.Int32 id)
        {
            #region 
            //接收需要导出的数据
            //T_RKD ckdinfo = new T_RKD();

            //List<T_RKD> list = T_RKDDomain.GetInstance().GetListModelById(id);
            //if (list.Count > 0)
            //{
            //    ckdinfo = list[0];
            //}

            ////验收单号
            //string rkysdh = ckdinfo.YSDH;

            ////供货企业名称
            //T_RKMX rkdinfo = new T_RKMX();
            //List<T_RKMX> rkmxList = T_RKMXDomain.GetInstance().GetListModelById(id);
            //if (rkmxList.Count > 0)
            //{
            //    rkdinfo = rkmxList[0];
            //}
            //string ghqy = rkdinfo.T_SupQY == null ? "" : rkdinfo.T_SupQY.SupMC;


            ////命名导出表格的StringBuilder变量
            //StringBuilder sHtml = new StringBuilder(string.Empty);
            ////打印表头
            //sHtml.Append("<table border=\"0\" width=\"100%\">"); // width=\"100%\"
            //sHtml.Append("<tr height=\"40\"><td colspan=\"10\" align=\"center\" style='font-size:24px'><b>入库单" + "</b></td></tr>");
            //sHtml.Append("<tr height=\"40\"><td colspan=\"8\" align=\"left\">供货企业：" + ghqy + "</td><td align=\"right\">日期：" + DateTime.Now.ToString("yyyy-MM-dd") + "</td><td align=\"right\">入库单号：" + DateTime.Now.ToString("yyyyMMddHHmmssffff") + "</td></tr>");
            //sHtml.Append("</table>");
            //sHtml.Append("<table border=\"1\" width=\"100%\">");
            ////sHtml.Append("<tr height=\"40\"><td colspan=\"10\" align=\"center\" style='font-size:24px'><b>出库单" + "</b></td></tr>");
            ////sHtml.Append("<tr height=\"40\"><td colspan=\"8\" align=\"left\">&nbsp;购买单位：" + xsqyName + "</td><td align=\"right\">日期：" + DateTime.Now.ToString("yyyy-MM-dd") + "</td><td align=\"right\">单据编号：" + DateTime.Now.ToString("yyyyMMddHHmmssffff") + "</td></tr>");
            ////打印列名
            ////sHtml.Append("<tr height=\"30\" align=\"center\" ><td style='width:150px;'>产品名称</td><td style='width:60px;'>产品规格</td><td style='width:150px;'>生产厂家</td><td  style='width:60px;'>生产日期</td><td  style='width:30px;'>单位</td><td style='width:30px;'>数量</td><td style='width:60px;'>单价</td>"
            ////    + "<td style='width:80px;'>金额</td><td style='width:80px;'>产品批号</td><td style='width:60px;'>产品有效期</td><td style='width:100px;'>经营许可证号</td><td style='width:100px;'>注册证号</td></tr>");
            //sHtml.Append("<tr height=\"30\" align=\"center\" ><td>产品名称</td><td>产品规格</td><td>生产厂家</td><td>生产日期</td><td>单位</td><td>数量</td><td>单价</td>"
            //    + "<td>金额</td><td>产品批号</td><td>产品有效期</td><td>经营许可证号</td><td>注册证号</td></tr>");
            ////合计
            //double total = 0.0;
            //for (int i = 0; i < rkmxList.Count; i++)
            //{
            //    T_RKMX rkmx = rkmxList[i];
            //    //产品名称
            //    string cpName = rkmx.T_YLCP.CPMC;
            //    //规格
            //    string cpGg = rkmx.T_YLCP.CPGG ?? "";
            //    //单位
            //    string cpDw = rkmx.T_YLCP.CPDW ?? "";
            //    //数量
            //    double cpDj = rkmx.CPNUM ?? 0;
            //    //产品批号
            //    string scPh = rkmx.CPPH ?? "";
            //    //产品有效期
            //    string yxq = "";
            //    if (rkmx.CPYXQ != null)
            //    {
            //        yxq = rkmx.CPYXQ.Value.ToString("yyyyMMdd");
            //    }
            //    var scrq = "";
            //    if (rkmx.CPSCRQ != null)
            //    {
            //        scrq = rkmx.CPSCRQ.Value.ToString("yyyyMMdd");
            //    }

            //    //生产企业
            //    string cpScqy = rkdinfo.T_SupQY1 == null ? "" : rkdinfo.T_SupQY1.SupMC;
            //    //单价
            //    double cpPrice = rkmx.T_YLCP.CPPrice ?? 0.0;
            //    //产品总价
            //    double rowTotal = cpDj * cpPrice;

            //    total = total + rowTotal;
            //    //注册证号
            //    string cpzczh = rkmx.T_YLCP.CPZCZ;
            //    //经营许可证号
            //    string xkzbh = rkmx.T_SupQY.SupXKZBH;
            //    sHtml.Append("<tr height=\"30\" align=\"center\"><td>" + cpName
            //                + "</td><td>" + cpGg + "</td><td>" + cpScqy
            //                 + "</td><td>" + scrq
            //                + "</td><td>" + cpDw + "</td><td>" + cpDj.ToString()
            //                + "</td><td>" + cpPrice.ToString() + "</td><td>" + rowTotal.ToString() + "</td><td>" + scPh + "</td><td>" + yxq
            //                + "</td><td>" + xkzbh
            //                + "</td><td>" + cpzczh
            //                + "</td></tr>");
            //}
            ////打印表尾
            //sHtml.Append("<tr height=\"40\" align=\"center\"><td colspan=\"5\">合计金额：（大写）" + MoneySmallToBig(total.ToString()) + "</td><td colspan=\"8\">（小写）" + total.ToString("0.000") + "</td></tr>");
            //sHtml.Append("</table>");
            //sHtml.Append("<table  border=\"0\" width=\"100%\">");
            //sHtml.Append("<tr height=\"40\" align=\"center\"><td colspan=\"7\" align=\"left\">制单人：&nbsp;&nbsp</td><td align=\"right\">审核人：&nbsp;&nbsp</td><td align=\"right\">采购员：&nbsp;&nbsp</td><td align=\"center\">质检员：&nbsp;&nbsp</td></tr>");
            //sHtml.Append("</table>");

            #endregion
            var str = ExportExcelPR(id);
            //调用输出Excel表的方法
            ExportToExcel("application/vnd.ms-excel", "入库单.xls", str);
        }

        [CheckLogin()]
        public void ExportToExcel(string FileType, string FileName, string ExcelContent)
        {
            System.Web.HttpContext.Current.Response.Charset = "UTF-8";
            System.Web.HttpContext.Current.Response.ContentEncoding = System.Text.Encoding.GetEncoding("UTF-8");
            var fName = HttpUtility.UrlEncode(FileName, System.Text.Encoding.GetEncoding("UTF-8")).ToString();
            System.Web.HttpContext.Current.Response.AppendHeader("Content-Disposition", "attachment;filename=" + fName);
            System.Web.HttpContext.Current.Response.ContentType = FileType;
            System.IO.StringWriter tw = new System.IO.StringWriter();
            System.Web.HttpContext.Current.Response.Output.Write(ExcelContent.ToString());
            System.Web.HttpContext.Current.Response.Flush();
            System.Web.HttpContext.Current.Response.End();
        }

        [CheckLogin()]
        public string ExportExcelPR(System.Int32 id)
        {
            //获取需要打印的数据
            T_RKD ckdinfo = new T_RKD();

            List<T_RKD> list = T_RKDDomain.GetInstance().GetListModelById(id);
            if (list.Count > 0)
            {
                ckdinfo = list[0];
            }

            //验收单号
            string rkysdh = ckdinfo.YSDH;

            //供货企业名称
            T_RKMX rkdinfo = new T_RKMX();
            List<T_RKMX> rkmxList = T_RKMXDomain.GetInstance().GetListModelById(id);
            if (rkmxList.Count > 0)
            {
                rkdinfo = rkmxList[0];
            }
            string ghqy = rkdinfo.T_SupQY == null ? "" : rkdinfo.T_SupQY.SupMC;


            //命名导出表格的StringBuilder变量
            StringBuilder sHtml = new StringBuilder(string.Empty);
            //打印表头
            sHtml.Append("<table border=\"0\" width=\"100%\">"); //
            sHtml.Append("<tr height=\"40\"><td colspan=\"12\" align=\"center\" style='font-size:24px'><b>入库单" + "</b></td></tr>");
            sHtml.Append("<tr height=\"40\"><td colspan=\"6\" align=\"left\">供货企业：" + ghqy + "</td><td align=\"center\" colspan=\"4\">日期：" + DateTime.Now.ToString("yyyy-MM-dd") + "</td><td align=\"right\"  colspan=\"2\">入库单号：" + DateTime.Now.ToString("yyyyMMddHHmmssffff") + "</td></tr>");
            sHtml.Append("</table>");
            sHtml.Append("<table border=\"1\" width=\"100%\"  style='border-collapse:collapse;border:1px solid black;'>"); // width=\"100%\"
            sHtml.Append("<tr height=\"30\" align=\"center\" ><td>产品名称</td><td>产品规格</td><td>生产厂家</td><td>生产日期</td><td>单位</td><td>数量</td><td>单价</td>"
                + "<td>金额</td><td>产品批号</td><td>产品有效期</td><td>经营许可证号</td><td>注册证号</td></tr>");

            //合计
            double total = 0.0;
            for (int i = 0; i < rkmxList.Count; i++)
            {
                T_RKMX rkmx = rkmxList[i];
                //产品名称
                string cpName = rkmx.T_YLCP.CPMC;
                //规格
                string cpGg = rkmx.T_YLCP.CPGG ?? "";
                //单位
                string cpDw = rkmx.T_YLCP.CPDW ?? "";
                //数量
                double cpDj = rkmx.CPNUM ?? 0;
                //产品批号
                string scPh = rkmx.CPPH ?? "";
                //产品有效期
                string yxq = "";
                if (rkmx.CPYXQ != null)
                {
                    yxq = rkmx.CPYXQ.Value.ToString("yyyyMMdd");
                }
                var scrq = "";
                if (rkmx.CPSCRQ != null)
                {
                    scrq = rkmx.CPSCRQ.Value.ToString("yyyyMMdd");
                }

                //生产企业
                string cpScqy = rkdinfo.T_SupQY1 == null ? "" : rkdinfo.T_SupQY1.SupMC;
                //单价
                double cpPrice = rkmx.T_YLCP.CPPrice ?? 0.0;
                //产品总价
                double rowTotal = cpDj * cpPrice;

                total = total + rowTotal;
                //注册证号
                string cpzczh = rkmx.T_YLCP.CPZCZ;
                //经营许可证号
                string xkzbh = rkmx.T_SupQY.SupXKZBH;
                sHtml.Append("<tr height=\"30\" align=\"center\"><td>" + cpName
                            + "</td><td>" + cpGg + "</td><td>" + cpScqy
                             + "</td><td>" + scrq
                            + "</td><td>" + cpDw + "</td><td>" + cpDj.ToString()
                            + "</td><td>" + cpPrice.ToString("0.00") + "</td><td>" + rowTotal.ToString("0.00") + "</td><td>" + scPh + "</td><td>" + yxq
                            + "</td><td>" + xkzbh
                            + "</td><td>" + cpzczh
                            + "</td></tr>");
            }
            //打印表尾
            sHtml.Append("<tr height=\"40\" align=\"center\"><td colspan=\"5\">合计金额：（大写）" + MoneySmallToBig(total.ToString()) + "</td><td colspan=\"7\">（小写）" + total.ToString("0.00") + "</td></tr>");
            sHtml.Append("</table>");
            sHtml.Append("<table  border=\"0\" width=\"100%\">"); // width =\"100%\"
            Expression<Func<T_YSD, bool>> where = p => p.YSDH == ckdinfo.YSDH;
            var ysd = T_YSDDomain.GetInstance().GetAllModels<int>(where).FirstOrDefault();
            var cgdModel = new T_CGD();
            if (ysd != null && ysd.YSID != 0 && !string.IsNullOrEmpty(ysd.CGDH))
            {
                Expression<Func<T_CGD, bool>> where1 = p => p.CGDH == ysd.CGDH;
                cgdModel = T_CGDDomain.GetInstance().GetAllModels<int>(where1).FirstOrDefault();
            }

            sHtml.Append("<tr height=\"40\" align=\"center\"><td colspan=\"2\" align=\"left\">制单人：&nbsp;" + ckdinfo.RKCJR + "</td><td align=\"left\" colspan=\"4\">审核人：&nbsp;" + cgdModel.SHR + "</td><td align=\"left\" colspan=\"3\">采购员：&nbsp;" + cgdModel.CGPERSON + "</td><td align=\"left\" colspan=\"3\">质检员：&nbsp;</td></tr>");
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
        private string GetRoleCode()
        {
            return Session["RoleCode"] == null ? "" : Session["RoleCode"].ToString();
        }
        [HttpPost]
        [CheckLogin()]
        public void through(T_RKDModels model, int id)
        {
            int result = 0;
            try
            {
                Int32 cgid = model.DataModel.RKID;
                result = T_RKDDomain.GetInstance().Sh(cgid, id);
                if (id == 1 && model.DataModel.IsFinish == 1)  //IsFinish=1表示验收单已经完结
                {
                    T_YSDDomain.GetInstance().UpdateFinish(model.DataModel.YSDH);
                }
            }
            catch { }
            Response.ContentType = "text/json";
            if (result > 0)
                Response.Write("{\"statusCode\":\"200\", \"message\":\"操作成功\",\"callbackType\":\"closeCurrentReloadTab\",\"forwardUrl\":\"/T_RKD/Index\"}");
            else
                Response.Write("{\"statusCode\":\"300\", \"message\":\"操作失败\"}");
        }
    }
}
