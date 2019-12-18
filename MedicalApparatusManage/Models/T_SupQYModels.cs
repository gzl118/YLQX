using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MedicalApparatusManage.Models
{
    public class T_SupQYModels : BaseModels<T_SupQY>
    {
        public string ZZSCPath { get { return "/UploadFiles/供货企业资质/" + base.DataModel.SupZZSC; } }
        public string QYZZFiles { get; set; }
        public List<T_JYFW> JYFWList { get; set; }
        public T_SupQYModels()
        {
            QYZZFiles = string.Empty;
            base.QYZZList = new List<T_QYZZ>();
        }
    }
}