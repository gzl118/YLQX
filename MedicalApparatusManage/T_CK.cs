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
    
    public partial class T_CK
    {
        public T_CK()
        {
            this.T_CKMX = new HashSet<T_CKMX>();
            this.T_KC = new HashSet<T_KC>();
            this.T_RKMX = new HashSet<T_RKMX>();
            this.T_THMX = new HashSet<T_THMX>();
            this.T_SHMX = new HashSet<T_SHMX>();
        }
    
        public int CKID { get; set; }
        public string CKMC { get; set; }
        public string CKDZ { get; set; }
        public string CKSYWD { get; set; }
        public Nullable<double> CKSYSD { get; set; }
        public string CKBZ { get; set; }
        public string CKGLY { get; set; }
        public string FLAG { get; set; }
        public string CKLX { get; set; }
        public Nullable<System.DateTime> CJSJ { get; set; }
    
        public virtual ICollection<T_CKMX> T_CKMX { get; set; }
        public virtual ICollection<T_KC> T_KC { get; set; }
        public virtual ICollection<T_RKMX> T_RKMX { get; set; }
        public virtual ICollection<T_THMX> T_THMX { get; set; }
        public virtual ICollection<T_SHMX> T_SHMX { get; set; }
    }
}
