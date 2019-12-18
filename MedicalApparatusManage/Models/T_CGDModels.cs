using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MedicalApparatusManage.Models
{
    public class T_CGDModels : BaseModels<T_CGD>
    {
        public T_CGMX CGMXModel { get; set; }
        public T_CGDModels()
        {
            base.CGMXList = base.CGMXList ?? new List<T_CGMX>();
            CGMXModel = new T_CGMX();
        }
    }
}