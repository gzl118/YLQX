using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MedicalApparatusManage.Models
{
    public class T_YSDModels : BaseModels<T_YSD>
    {
        public string YSBGPath { get { return "/UploadFiles/验收报告/" + base.DataModel.YSBG; } }
        public T_YSMX YSMXModel { get; set; }
        public List<T_YSMX> YSMXList { get; set; }
        public T_YSDModels()
        {
            YSMXModel = new T_YSMX();
            YSMXList = new List<T_YSMX>();
        }
    }
}