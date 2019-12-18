using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MedicalApparatusManage.Models
{
    public class T_RKDModels : BaseModels<T_RKD>
    {
        public T_RKMX T_RKMXModel { get; set; }
        public List<T_RKMX> RKMXList { get; set; }
        public string CGDMC { get; set; }
        public string SQRMC { get; set; }
        public string CKRMC { get; set; }
        public T_RKDModels()
        {
            RKMXList = new List<T_RKMX>();
            T_RKMXModel = new T_RKMX();
            CGDMC = "";
            SQRMC = "";
            CKRMC = "";
        }
    }
}