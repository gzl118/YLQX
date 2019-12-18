using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MedicalApparatusManage.Models
{
    public class T_XSDModels : BaseModels<T_XSD>
    {
        public T_XSMX T_XSMXModel { get; set; }
        public T_XSDModels()
        {
            base.XSMXList = base.XSMXList ?? new List<T_XSMX>();
            T_XSMXModel = new T_XSMX();
        }
    }
}