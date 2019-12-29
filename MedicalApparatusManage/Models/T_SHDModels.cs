using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MedicalApparatusManage.Models
{
    public class T_SHDModels : BaseModels<T_SHD>
    {
        public T_SHMX T_SHMXModel { get; set; }
        public List<T_SHMX> SHMXList { get; set; }
        /// <summary>
        /// 损耗单号
        /// </summary>
        public string SHDH { get; set; }
        public T_SHDModels()
        {
            SHMXList = new List<T_SHMX>();
            T_SHMXModel = new T_SHMX();
            SHDH = "";
        }
    }
}