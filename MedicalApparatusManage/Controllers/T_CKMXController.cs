using MedicalApparatusManage.Domain;
using MedicalApparatusManage.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace MedicalApparatusManage.Controllers
{
    public class T_CKMXController : Controller
    {
        public int resultCount = 0; // 总条数 

        [CheckLogin()]
        public ActionResult Index(T_CKMXModels evalModel, string id)
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
            ViewBag.CKDID = id;
            int pagesize = Convert.ToInt32(evalModel.pageSize);
            int pagecount = Convert.ToInt32(evalModel.pagecount);
            int currentPage = Convert.ToInt32(evalModel.currentPage);
            evalModel.DataModel = evalModel.DataModel ?? new T_CKMX();
            evalModel.DataList = T_CKMXDomain.GetInstance().PageT_CKMX(evalModel.DataModel, evalModel.StartTime, evalModel.EndTime, currentPage, pagesize, out pagecount, out resultCount).Where(p => p.CKDID == int.Parse(id)).ToList();
            evalModel.resultCount = resultCount;
            ViewData["ParaStr"] = ExportExcelPR(int.Parse(id));
            return View("~/Views/T_CKMX/Index.cshtml", evalModel);
        }

        [CheckLogin()]
        public ActionResult Save(System.Int32 id, string tag)
        {
            T_CKMXModels model = new T_CKMXModels();
            model.DataModel = new T_CKMX();
            Int32 did = id;
            if (tag != "Add")
            {
                model.DataModel = T_CKMXDomain.GetInstance().GetModelById(id);
                did = model.DataModel.CKDID ?? 0;
            }
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

            //加载出库单列表
            T_CKDModels ckdQymode = new T_CKDModels();

            ckdQymode.DataModel = ckdQymode.DataModel ?? new T_CKD();

            T_CKD ckd = T_CKDDomain.GetInstance().GetModelById(did);
            ckdQymode.DataList = new List<T_CKD>();
            ckdQymode.DataList.Add(ckd);
            ViewData["CKD"] = new SelectList(ckdQymode.DataList, "CKID", "CKMC");



            //if (id != 0)
            //{
            //    model.DataModel = T_CKMXDomain.GetInstance().GetModelById(id);
            //}
            model.Tag = tag;
            return View("~/Views/T_CKMX/Save.cshtml", model);
        }

        [CheckLogin()]
        public void ExportExcel(System.Int32 id)
        {
            //接收需要导出的数据
            T_CKD ckdinfo = new T_CKD();

            List<T_CKD> list = T_CKDDomain.GetInstance().GetListModelById(id);
            if (list.Count > 0)
            {
                ckdinfo = list[0];
            }

            int xsqyid = ckdinfo.T_XSD.KHID ?? 0;

            T_CusQY cusqy = T_CusQYDomain.GetInstance().GetModelById(xsqyid);
            //客户企业名称
            string xsqyName = cusqy.CusMC;

            List<T_CKMX> ckmxList = T_CKMXDomain.GetInstance().GetListModelById(id);

            //命名导出表格的StringBuilder变量
            StringBuilder sHtml = new StringBuilder(string.Empty);
            //打印表头
            sHtml.Append("<table border=\"0\" width=\"100%\">");
            sHtml.Append("<tr height=\"40\"><td colspan=\"10\" align=\"center\" style='font-size:24px'><b>出库单" + "</b></td></tr>");
            sHtml.Append("<tr height=\"40\"><td colspan=\"8\" align=\"left\">购买单位：" + xsqyName + "</td><td align=\"right\">日期：" + DateTime.Now.ToString("yyyy-MM-dd") + "</td><td align=\"right\">单据编号：" + DateTime.Now.ToString("yyyyMMddHHmmssffff") + "</td></tr>");
            sHtml.Append("</table>");
            sHtml.Append("<table border=\"1\" width=\"100%\">");
            //sHtml.Append("<tr height=\"40\"><td colspan=\"10\" align=\"center\" style='font-size:24px'><b>出库单" + "</b></td></tr>");
            //sHtml.Append("<tr height=\"40\"><td colspan=\"8\" align=\"left\">&nbsp;购买单位：" + xsqyName + "</td><td align=\"right\">日期：" + DateTime.Now.ToString("yyyy-MM-dd") + "</td><td align=\"right\">单据编号：" + DateTime.Now.ToString("yyyyMMddHHmmssffff") + "</td></tr>");
            //打印列名
            sHtml.Append("<tr height=\"30\" align=\"center\" ><td>商品名称</td><td>规格</td><td>生产厂家</td><td>单位</td><td>数量</td><td>单价</td>"
                + "<td>金额</td><td>批号</td><td>生产日期</td><td>注册证号</td></tr>");

            //合计
            double total = 0.0;
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
                //生成批号
                string scPh = ckmx.T_YLCP.CPPH ?? "";
                //生产日期
                string scRq = "";
                if (ckmx.T_YLCP.CPSCSJ != null)
                {
                    scRq = ckmx.T_YLCP.CPSCSJ.Value.ToLongDateString();
                }

                //生产企业
                string cpScqy = ckmx.T_YLCP.CPSCQY ?? "";
                //单价
                double cpPrice = ckmx.T_YLCP.CPPrice ?? 0.0;
                //产品总价
                double rowTotal = cpDj * cpPrice;

                total = total + rowTotal;
                //注册证号
                string cpzczh = ckmx.T_YLCP.CPZCZ;

                sHtml.Append("<tr height=\"30\" align=\"center\"><td>" + cpName
    + "</td><td>" + cpGg + "</td><td>" + cpScqy
    + "</td><td>" + cpDw + "</td><td>" + cpDj.ToString()
    + "</td><td>" + cpPrice.ToString() + "</td><td>" + rowTotal.ToString() + "</td><td>" + scPh + "</td><td>" + scRq
    + "</td><td>" + cpzczh
    + "</td></tr>");
            }
            //循环读取List集合 
            for (int i = 0; i < list.Count; i++)
            {

            }
            //打印表尾
            sHtml.Append("<tr height=\"40\" align=\"center\"><td>合计：</td><td colspan=\"9\">" + total + "</td></tr>");
            sHtml.Append("</table>");
            sHtml.Append("<table  border=\"0\" width=\"100%\">");
            sHtml.Append("<tr height=\"40\" align=\"center\"><td colspan=\"7\" align=\"left\">制单人：&nbsp;&nbsp</td><td align=\"right\">审核人：&nbsp;&nbsp</td><td align=\"right\">发货人：&nbsp;&nbsp</td><td align=\"center\">销售员：&nbsp;&nbsp</td></tr>");
            sHtml.Append("</table>");
            //调用输出Excel表的方法
            ExportToExcel("application/vnd.ms-excel", "出库单.xls", sHtml.ToString());
        }

        [CheckLogin()]
        public void ExportToExcel(string FileType, string FileName, string ExcelContent)
        {
            System.Web.HttpContext.Current.Response.Charset = "GB2312";
            System.Web.HttpContext.Current.Response.ContentEncoding = System.Text.Encoding.GetEncoding("GB2312");
            System.Web.HttpContext.Current.Response.AppendHeader("Content-Disposition", "attachment;filename=" + HttpUtility.UrlEncode(FileName, System.Text.Encoding.GetEncoding("GB2312")).ToString());
            System.Web.HttpContext.Current.Response.ContentType = FileType;
            System.IO.StringWriter tw = new System.IO.StringWriter();
            System.Web.HttpContext.Current.Response.Output.Write(ExcelContent.ToString());
            System.Web.HttpContext.Current.Response.Flush();
            System.Web.HttpContext.Current.Response.End();
        }

        [CheckLogin()]
        public string ExportExcelPR(System.Int32 id)
        {
            //接收需要导出的数据
            T_CKD ckdinfo = new T_CKD();

            List<T_CKD> list = T_CKDDomain.GetInstance().GetListModelById(id);
            if (list.Count > 0)
            {
                ckdinfo = list[0];
            }

            int xsqyid = ckdinfo.T_XSD.KHID ?? 0;

            T_CusQY cusqy = T_CusQYDomain.GetInstance().GetModelById(xsqyid);
            //客户企业名称
            string xsqyName = cusqy.CusMC;

            List<T_CKMX> ckmxList = T_CKMXDomain.GetInstance().GetListModelById(id);

            //命名导出表格的StringBuilder变量
            StringBuilder sHtml = new StringBuilder(string.Empty);
            //打印表头
            sHtml.Append("<table border=\"0\" width=\"100%\">");
            sHtml.Append("<tr height=\"40\"><td colspan=\"10\" align=\"center\" style='font-size:24px'><b>出库单" + "</b></td></tr>");
            sHtml.Append("<tr height=\"40\"><td colspan=\"8\" align=\"left\">购买单位：" + xsqyName + "</td><td align=\"right\">日期：" + DateTime.Now.ToString("yyyy-MM-dd") + "</td><td align=\"right\">单据编号：" + DateTime.Now.ToString("yyyyMMddHHmmssffff") + "</td></tr>");
            sHtml.Append("</table>");
            sHtml.Append("<table border=\"1\" width=\"100%\">");
            //sHtml.Append("<tr height=\"40\"><td colspan=\"10\" align=\"center\" style='font-size:24px'><b>出库单" + "</b></td></tr>");
            //sHtml.Append("<tr height=\"40\"><td colspan=\"8\" align=\"left\">&nbsp;购买单位：" + xsqyName + "</td><td align=\"right\">日期：" + DateTime.Now.ToString("yyyy-MM-dd") + "</td><td align=\"right\">单据编号：" + DateTime.Now.ToString("yyyyMMddHHmmssffff") + "</td></tr>");
            //打印列名
            sHtml.Append("<tr height=\"30\" align=\"center\" ><td>商品名称</td><td>规格</td><td>生产厂家</td><td>单位</td><td>数量</td><td>单价</td>"
                + "<td>金额</td><td>批号</td><td>生产日期</td><td>注册证号</td></tr>");

            //合计
            double total = 0.0;
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
                //生成批号
                string scPh = ckmx.T_YLCP.CPPH ?? "";
                //生产日期
                string scRq = "";
                if (ckmx.T_YLCP.CPSCSJ != null)
                {
                    scRq = ckmx.T_YLCP.CPSCSJ.Value.ToLongDateString();
                }

                //生产企业
                string cpScqy = ckmx.T_YLCP.CPSCQY ?? "";
                //单价
                double cpPrice = ckmx.T_YLCP.CPPrice ?? 0.0;
                //产品总价
                double rowTotal = cpDj * cpPrice;

                total = total + rowTotal;
                //注册证号
                string cpzczh = ckmx.T_YLCP.CPZCZ;

                sHtml.Append("<tr height=\"30\" align=\"center\"><td>" + cpName
    + "</td><td>" + cpGg + "</td><td>" + cpScqy
    + "</td><td>" + cpDw + "</td><td>" + cpDj.ToString()
    + "</td><td>" + cpPrice.ToString() + "</td><td>" + rowTotal.ToString() + "</td><td>" + scPh + "</td><td>" + scRq
    + "</td><td>" + cpzczh
    + "</td></tr>");
            }
            //循环读取List集合 
            for (int i = 0; i < list.Count; i++)
            {

            }
            //打印表尾
            sHtml.Append("<tr height=\"40\" align=\"center\"><td>合计：</td><td colspan=\"9\">" + total + "</td></tr>");
            sHtml.Append("</table>");
            sHtml.Append("<table  border=\"0\" width=\"100%\">");
            sHtml.Append("<tr height=\"40\" align=\"center\"><td colspan=\"7\" align=\"left\">制单人：&nbsp;&nbsp</td><td align=\"right\">审核人：&nbsp;&nbsp</td><td align=\"right\">发货人：&nbsp;&nbsp</td><td align=\"center\">销售员：&nbsp;&nbsp</td></tr>");
            sHtml.Append("</table>");
            //调用输出Excel表的方法
            return sHtml.ToString();
            //ExportToExcel("application/vnd.ms-excel", "出库单.xls", sHtml.ToString());
        }

        [HttpPost]
        [CheckLogin()]
        public void Save(T_CKMXModels model)
        {
            int result = 0;
            string guid = string.Empty;
            try
            {
                if (model.Tag == "Add")
                {
                    model.DataModel.GUID = Guid.NewGuid().ToString("N");
                    guid = model.DataModel.GUID;
                    result = T_CKMXDomain.GetInstance().AddModelByCkdh(model.DataModel, model.CKDH);
                }
                else if (model.Tag == "Edit")
                {
                    result = T_CKMXDomain.GetInstance().UpdateModel(model.DataModel, model.DataModel.CKMID);
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
        public void Delete(string guid)
        {
            int result = T_CKMXDomain.GetInstance().DeleteModelByGuid(guid);
            Response.ContentType = "text/json";
            if (result > 0)
            {
                Response.Write("{\"statusCode\":\"200\", \"message\":\"操作成功\",\"callbackType\":\"forward\",\"forwardUrl\":\"/T_CKD/Index\"}");
            }
            else
                Response.Write("{\"statusCode\":\"300\", \"message\":\"操作失败\"}");
        }

        [CheckLogin()]
        public ActionResult CKMXTable(System.Int32 id, string ckdh, int canEdit)
        {
            T_CKDModels model = new T_CKDModels();
            if (id != 0)
            {
                model.CKMXList = T_CKMXDomain.GetInstance().GetT_CKMXByCkid(id);
            }
            else
            {
                model.CKMXList = T_CKMXDomain.GetInstance().GetT_CKMXByCkdh(ckdh);
            }
            return View("~/Views/T_CKMX/CKMXTable.cshtml", model);
        }
    }
}
