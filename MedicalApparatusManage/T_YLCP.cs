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
    
    public partial class T_YLCP
    {
        public T_YLCP()
        {
            this.T_CGMX = new HashSet<T_CGMX>();
            this.T_CKMX = new HashSet<T_CKMX>();
            this.T_KC = new HashSet<T_KC>();
            this.T_RKMX = new HashSet<T_RKMX>();
            this.T_THMX = new HashSet<T_THMX>();
            this.T_XSMX = new HashSet<T_XSMX>();
            this.T_YLCPZZ = new HashSet<T_YLCPZZ>();
            this.T_YSMX = new HashSet<T_YSMX>();
            this.T_SHMX = new HashSet<T_SHMX>();
        }
    
        public int CPID { get; set; }
        public string CPMC { get; set; }
        public string CPGG { get; set; }
        public string CPXH { get; set; }
        public string CPSCQY { get; set; }
        public string CPSCDZ { get; set; }
        public Nullable<int> CPJYDW { get; set; }
        public Nullable<int> CPGYSID { get; set; }
        public Nullable<int> CPLXID { get; set; }
        public string CPZCZ { get; set; }
        public string CPDW { get; set; }
        public Nullable<double> CPPrice { get; set; }
        public string CPPH { get; set; }
        public Nullable<System.DateTime> CPSCSJ { get; set; }
        public string CPLRR { get; set; }
        public Nullable<System.DateTime> CPLRRQ { get; set; }
        public string CPTP { get; set; }
        public string CPFJ { get; set; }
        public Nullable<int> CPStatus { get; set; }
        public string CPBH { get; set; }
        public Nullable<double> XSJG { get; set; }
        public Nullable<System.DateTime> YXQXKS { get; set; }
        public Nullable<System.DateTime> YXQXJS { get; set; }
        public Nullable<int> CPSCQYID { get; set; }
        public Nullable<int> BJSL { get; set; }
        public Nullable<int> BJDate { get; set; }
        public string ZCRName { get; set; }
        public string ZCRAddr { get; set; }
        public string Remark { get; set; }
        public string CCTJ { get; set; }
        public Nullable<int> CKID { get; set; }
        public string YXSC { get; set; }
    
        public virtual ICollection<T_CGMX> T_CGMX { get; set; }
        public virtual ICollection<T_CKMX> T_CKMX { get; set; }
        public virtual T_CPLX T_CPLX { get; set; }
        public virtual ICollection<T_KC> T_KC { get; set; }
        public virtual ICollection<T_RKMX> T_RKMX { get; set; }
        public virtual T_SupQY T_SupQY { get; set; }
        public virtual T_SupQY T_SupQY1 { get; set; }
        public virtual ICollection<T_THMX> T_THMX { get; set; }
        public virtual ICollection<T_XSMX> T_XSMX { get; set; }
        public virtual ICollection<T_YLCPZZ> T_YLCPZZ { get; set; }
        public virtual ICollection<T_YSMX> T_YSMX { get; set; }
        public virtual ICollection<T_SHMX> T_SHMX { get; set; }
    }
}
