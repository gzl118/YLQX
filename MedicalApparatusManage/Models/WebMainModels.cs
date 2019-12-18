using System.Collections.Generic;

namespace MedicalApparatusManage.Models
{
    public class WebMainModels
    {
        public List<SysResource> ListResource { get; set; }

        public List<SysFavorite> ListSysFavorite { get; set; }

        public SysUser ModelUser { get; set; }

        public string UserName { get; set; }

        public string passWord { get; set; }

    }

}