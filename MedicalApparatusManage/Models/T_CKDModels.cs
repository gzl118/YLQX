using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MedicalApparatusManage.Models
{
    public class T_CKDModels : BaseModels<T_CKD>
    {
        public T_CKMX T_CKMXModel { get; set; }
        public List<T_CKMX> CKMXList { get; set; }
        public string XSDMC { get; set; }
        /// <summary>
        /// 销售单号
        /// </summary>
        public string XSDH { get; set; }
        public T_CKDModels()
        {
            CKMXList = new List<T_CKMX>();
            T_CKMXModel = new T_CKMX();
            XSDMC = "";
        }
    }
}