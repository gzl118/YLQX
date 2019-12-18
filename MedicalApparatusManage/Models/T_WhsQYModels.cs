using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MedicalApparatusManage.Models
{
    public class T_WhsQYModels : BaseModels<T_WhsQY>
    {
        public string ZZSCPath { get { return "/UploadFiles/本企业资质/" + base.DataModel.WhsZZSC; } }
        public string QYZZFiles { get; set; }
        public string[] Applications;
        public List<T_WhsQYZZ> WhsQYZZList { get; set; }
        public List<T_JYFW> JYFWList { get; set; }
        public T_WhsQYModels()
        {
            QYZZFiles = string.Empty;
            WhsQYZZList = new List<T_WhsQYZZ>();
        }
    }
}