//------------------------------------------------------------------------------
// <auto-generated>
//     此代码已从模板生成。
//
//     手动更改此文件可能导致应用程序出现意外的行为。
//     如果重新生成代码，将覆盖对此文件的手动更改。
// </auto-generated>
//------------------------------------------------------------------------------

namespace MedicalApparatusManage
{
    using System;
    using System.Collections.Generic;
    
    public partial class SysResource
    {
        public SysResource()
        {
            this.SysRoleResource = new HashSet<SysRoleResource>();
        }
    
        public int ResourceId { get; set; }
        public string ResourceName { get; set; }
        public string ResourceCode { get; set; }
        public string ParentCode { get; set; }
        public string ResourceRemark { get; set; }
        public string ResourceOrder { get; set; }
        public Nullable<int> ResourceNodeTage { get; set; }
        public Nullable<int> ResourceIsHavChild { get; set; }
    
        public virtual ICollection<SysRoleResource> SysRoleResource { get; set; }
    }
}