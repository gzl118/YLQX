using MedicalApparatusManage.Common;
using MedicalApparatusManage.Domain;
using MedicalApparatusManage.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace MedicalApparatusManage.Controllers
{
    public class T_SHDController : Controller
    {
        public int resultCount = 0; // 总条数 
        public SysUser CurUser = null;
        // GET: T_SHD
        public ActionResult Index(T_SHDModels evalModel)
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
            evalModel.DataModel = evalModel.DataModel ?? new T_SHD();

            if (Request["strSHDH"] != null)
            {
                string str = Request["strSHDH"].ToString();
                if (!String.IsNullOrEmpty(str))
                {
                    evalModel.DataModel.SHDH = str.Trim();
                }
                ViewData["strSHDH"] = str;
            }

            if (Request["strSHSQPerson"] != null)
            {
                string str = Request["strSHSQPerson"].ToString();
                if (!String.IsNullOrEmpty(str))
                {
                    evalModel.DataModel.SQR = str.Trim();
                }
                ViewData["strSHSQPerson"] = str;
            }


            //获取本企业下的人员列表
            SysUser UserModel = Session["UserModel"] as SysUser;
            T_Person person = new T_Person();
            person.PsQYID = (int)UserModel.UserCompanyID;
            ViewBag.Persons = new SelectList(T_PersonDomain.GetInstance().GetAllT_Person(person), "PsMZ", "PsMZ");

            evalModel.DataList = T_SHDDomain.GetInstance().PageT_SHD(evalModel.DataModel, evalModel.StartTime, evalModel.EndTime, currentPage, pagesize, out pagecount, out resultCount);
            evalModel.resultCount = resultCount;
            return View("~/Views/T_SHD/Index.cshtml", evalModel);
        }
        [CheckLogin()]
        public ActionResult Save(System.Int32 id, string tag)
        {
            CurUser = Session["UserModel"] as SysUser;
            T_SHDModels model = new T_SHDModels();

            //加载产品列表
            T_YLCPModels ylcpQymode = new T_YLCPModels();
            ylcpQymode.DataModel = ylcpQymode.DataModel ?? new T_YLCP();
            ylcpQymode.DataList = T_YLCPDomain.GetInstance().GetAllT_YLCP(ylcpQymode.DataModel).Where(p => p.CPStatus == Convert.ToInt32("1")).ToList();
            ViewData["YLCP"] = new SelectList(ylcpQymode.DataList, "CPID", "CPMC");

            //获取本企业下的人员列表
            T_Person person = new T_Person();
            person.PsQYID = (int)CurUser.UserCompanyID;
            ViewBag.Persons = new SelectList(T_PersonDomain.GetInstance().GetAllT_Person(person), "PsMZ", "PsMZ");

            model.DataModel = new T_SHD();
            if (id != 0)
            {
                model.DataModel = T_SHDDomain.GetInstance().GetModelById(id);
                model.SHMXList = T_SHDDomain.GetInstance().GetT_SHMXByshid(id);
            }
            else
            {
                model.DataModel.SHDH = T_SHDDomain.GetInstance().GetSHOrderNum(CurUser);
                model.DataModel.SHCJR = CurUser.UserAccount;
                model.DataModel.SHCJRQ = DateTime.Now;
            }
            model.Tag = tag;
            model.RoleCode = GetRoleCode();
            return View("~/Views/T_SHD/Save.cshtml", model);
        }
        [HttpPost]
        [CheckLogin()]
        public void Save(T_SHDModels model)
        {
            int result = 0;
            try
            {
                if (model.Tag == "Add")
                {
                    model.DataModel.ISSH = 0;
                    result = T_SHDDomain.GetInstance().AddModel(model.DataModel);
                }
                else if (model.Tag == "Edit")
                {
                    model.DataModel.ISSH = 0;
                    result = T_SHDDomain.GetInstance().UpdateModel(model.DataModel, model.DataModel.SHID);
                }

            }
            catch { }
            Response.ContentType = "text/json";
            if (result > 0)
                Response.Write("{\"statusCode\":\"200\", \"message\":\"操作成功\",\"callbackType\":\"closeCurrentReloadTab\",\"forwardUrl\":\"/T_SHD/Index\"}");
            else
                Response.Write("{\"statusCode\":\"300\", \"message\":\"操作失败\"}");
        }
        [HttpPost]
        [CheckLogin()]
        public void Delete(System.Int32 id)
        {
            //var rCode = GetRoleCode();
            var temp = T_SHDDomain.GetInstance().GetModelById(id);
            if (temp != null)
            {
                if (temp.ISSH == 1)
                {
                    Response.Write("{\"statusCode\":\"300\", \"message\":\"已审批通过的数据不能删除！\"}");
                    return;
                }
            }
            int result = T_SHDDomain.GetInstance().Delete(id);
            Response.ContentType = "text/json";
            if (result > 0)
                Response.Write("{\"statusCode\":\"200\", \"message\":\"操作成功\",\"callbackType\":\"forward\",\"forwardUrl\":\"/T_SHD/Index\"}");
            else
                Response.Write("{\"statusCode\":\"300\", \"message\":\"操作失败\"}");
        }
        private string GetRoleCode()
        {
            return Session["RoleCode"] == null ? "" : Session["RoleCode"].ToString();
        }
        [HttpPost]
        [CheckLogin()]
        public JsonResult GetYLCPDetailsID(int id)
        {
            T_YLCP cp = T_YLCPDomain.GetInstance().GetCpDetailsById(id);
            if (cp != null)
            {
                string resultStr = JsonConvert.SerializeObject(new
                {
                    CPID = cp.CPID,
                    CPBH = cp.CPBH,
                    SCQYMC = (cp.T_SupQY1 != null && !string.IsNullOrEmpty(cp.T_SupQY1.SupMC)) ? cp.T_SupQY1.SupMC : "",
                    CPGG = cp.CPGG,
                    CPXH = cp.CPXH,
                    CPDW = cp.CPDW,
                    XKZH = (cp.T_SupQY1 != null && !string.IsNullOrEmpty(cp.T_SupQY1.SupXKZBH)) ? cp.T_SupQY1.SupXKZBH : "",
                    ZCZH = cp.CPZCZ,
                    SCQYID = cp.CPSCQYID,
                    CPPrice = cp.CPPrice,
                    SUPQYID = cp.CPGYSID,
                    SUPQYMC = (cp.T_SupQY != null && !string.IsNullOrEmpty(cp.T_SupQY.SupMC)) ? cp.T_SupQY.SupMC : "",
                    XSJG = cp.XSJG,
                    CPMC = cp.CPMC
                });
                return Json(resultStr);
            }
            return Json("");
        }
        [HttpPost]
        [CheckLogin()]
        public void through(T_SHDModels model, int id)
        {
            int result = 0;
            try
            {
                Int32 xsdid = model.DataModel.SHID;
                result = T_SHDDomain.GetInstance().Sh(xsdid, id);
            }
            catch { }
            Response.ContentType = "text/json";
            if (result > 0)
                Response.Write("{\"statusCode\":\"200\", \"message\":\"操作成功\",\"callbackType\":\"closeCurrentReloadTab\",\"forwardUrl\":\"/T_SHD/Index\"}");
            else
                Response.Write("{\"statusCode\":\"300\", \"message\":\"操作失败\"}");
        }
        [CheckLogin()]
        public ActionResult Details(System.Int32 id)
        {
            T_SHDModels model = new T_SHDModels();
            model.DataModel = new T_SHD();
            model.DataModel.SHID = id;
            ViewData["ParaStr"] = ExportExcelPR(id);
            return View("~/Views/T_SHD/Details.cshtml", model);
        }
        #region 导出excel及打印功能

        [CheckLogin()]
        public void ExportExcel(System.Int32 id)
        {
            var str = ExportExcelPR(id);
            ExportToExcel("application/vnd.ms-excel", "损耗单.xls", str);
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
            var ckdinfo = T_SHDDomain.GetInstance().GetModelById(id);

            List<T_SHMX> ckmxList = T_SHMXDomain.GetInstance().GetListModelById(id);
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
            sHtml.Append("<tr height=\"40\"><td colspan=\"12\" align=\"center\" style='font-size:24px'><b>" + qy.WhsMC + "损耗单" + "</b></td></tr>");
            sHtml.Append("<tr height=\"40\"><td align=\"left\"  colspan=\"8\">日 期：" + DateTime.Now.ToString("yyyy-MM-dd") + "</td><td align=\"right\"  colspan=\"4\">单据编号：" + DateTime.Now.ToString("yyyyMMddHHmmssffff") + "</td></tr>");
            sHtml.Append("</table>");
            sHtml.Append("<table border=\"1\" width=\"100%\" style='border-collapse:collapse;border:1px solid black;'>");
            sHtml.Append("<tr height=\"30\" align=\"center\" ><td>产品名称</td><td>规格</td><td>型号</td><td>生产企业</td><td>生产日期</td><td>单位</td><td>数量</td><td>单价</td>"
                + "<td>金额</td><td>产品批号</td><td>产品有效期</td><td>经营许可证号</td><td>注册证号</td></tr>");

            //合计
            double total = 0.0;
            double NumCpNum = 0.0;
            for (int i = 0; i < ckmxList.Count; i++)
            {
                var ckmx = ckmxList[i];
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
                double cpPrice = ckmx.T_YLCP.CPPrice ?? 0.0;
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
                sHtml.Append("<tr height=\"30\" align=\"center\"><td>" + cpName
                            + "</td><td>" + cpGg + "</td><td>" + cpxh + "</td><td>" + cpScqy
                             + "</td><td>" + scrq
                            + "</td><td>" + cpDw + "</td><td>" + cpDj.ToString()
                            + "</td><td>" + cpPrice.ToString("0.000") + "</td><td>" + rowTotal.ToString("0.000") + "</td><td>" + scPh + "</td><td>" + scRq
                            + "</td><td>" + xkzbh
                            + "</td><td>" + cpzczh
                            + "</td></tr>");
            }
            //打印表尾
            sHtml.Append("<tr height=\"40\" align=\"center\"><td colspan=\"6\">合计金额：（大写）" + MoneySmallToBig(total.ToString()) + "</td><td colspan=\"7\">（小写）" + total.ToString("0.000") + "</td></tr>");
            sHtml.Append("</table>");
            sHtml.Append("<table  border=\"0\" width=\"100%\">");
            sHtml.Append("<tr height=\"40\" align=\"center\"><td colspan=\"7\" align=\"left\">申请人：&nbsp;" + ckdinfo.SQR + "</td><td align=\"left\" colspan='5'>复核员：&nbsp;" + ckdinfo.FHY + "</td></tr>");
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