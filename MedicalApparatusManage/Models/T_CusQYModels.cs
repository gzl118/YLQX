using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MedicalApparatusManage.Models
{
    public class T_CusQYModels : BaseModels<T_CusQY>
    {
        public string ZZSCPath { get { return "/UploadFiles/购货企业资质/" + base.DataModel.CusZZSC; } }
        public string QYZZFiles { get; set; }
        public List<T_JYFW> JYFWList { get; set; }
        public T_CusQYModels()
        {
            QYZZFiles = string.Empty;
            base.CusQYZZList = new List<T_CusQYZZ>();
        }
    }
}