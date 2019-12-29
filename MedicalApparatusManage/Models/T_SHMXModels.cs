using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MedicalApparatusManage.Models
{
    public class T_SHMXModels : BaseModels<T_SHMX>
    {
        /// <summary>
        /// 损耗单号
        /// </summary>
        public string SHDH { get; set; }
    }
}