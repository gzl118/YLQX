using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MedicalApparatusManage.Models
{
    public class T_YLCPModels : BaseModels<T_YLCP>
    {
        public string CPZZFiles { get; set; }
        public List<T_YLCPZZ> YLCPZZList { get; set; }
        public T_YLCPModels()
        {
            CPZZFiles = string.Empty;
            YLCPZZList = new List<T_YLCPZZ>();
        }
    }
}