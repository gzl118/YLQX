using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MedicalApparatusManage.Models
{
    public class T_THDModels : BaseModels<T_THD>
    {
        public T_THMX T_THMXModel { get; set; }
        public List<T_THMX> THMXList { get; set; }

        public string YSDH { get; set; }
        public T_THDModels()
        {
            THMXList = new List<T_THMX>();
        }
    }
}