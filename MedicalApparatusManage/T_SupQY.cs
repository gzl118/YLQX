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
    
    public partial class T_SupQY
    {
        public T_SupQY()
        {
            this.T_CGMX = new HashSet<T_CGMX>();
            this.T_CGMX1 = new HashSet<T_CGMX>();
            this.T_HT = new HashSet<T_HT>();
            this.T_KC = new HashSet<T_KC>();
            this.T_KC1 = new HashSet<T_KC>();
            this.T_QYZZ = new HashSet<T_QYZZ>();
            this.T_RKMX = new HashSet<T_RKMX>();
            this.T_RKMX1 = new HashSet<T_RKMX>();
            this.T_SupPerson = new HashSet<T_SupPerson>();
            this.T_YLCP = new HashSet<T_YLCP>();
            this.T_YLCP1 = new HashSet<T_YLCP>();
            this.T_YSMX = new HashSet<T_YSMX>();
            this.T_YSMX1 = new HashSet<T_YSMX>();
        }
    
        public int SupID { get; set; }
        public string SupMC { get; set; }
        public string SupFSDB { get; set; }
        public Nullable<System.DateTime> SupZCSJ { get; set; }
        public string SupZCDZ { get; set; }
        public string SupZZJGDM { get; set; }
        public string SupDWXZ { get; set; }
        public string SupJXFW { get; set; }
        public string SupBZ { get; set; }
        public Nullable<System.DateTime> SupJYKSSJ { get; set; }
        public Nullable<System.DateTime> SupJXJSSJ { get; set; }
        public string SupKHH { get; set; }
        public string SupZH { get; set; }
        public string SupJYXKZ { get; set; }
        public Nullable<double> SupZCZB { get; set; }
        public string SupType { get; set; }
        public int WhsID { get; set; }
        public Nullable<int> SupStatus { get; set; }
        public string SupXKZBH { get; set; }
        public string SupJYFS { get; set; }
        public string SupJYCS { get; set; }
        public string SupKFDZ { get; set; }
        public string SupJYFW { get; set; }
        public Nullable<System.DateTime> SupFZRQ { get; set; }
        public Nullable<System.DateTime> SupYXQKS { get; set; }
        public Nullable<System.DateTime> SupYXQJS { get; set; }
        public string SupZZSC { get; set; }
        public string SupLXR { get; set; }
        public string SupSFZH { get; set; }
        public string SupLXDH { get; set; }
        public Nullable<System.DateTime> JYXKZFZRQ { get; set; }
        public Nullable<System.DateTime> JYXKZYXQKS { get; set; }
        public Nullable<System.DateTime> JYXKZYXQJS { get; set; }
        public Nullable<System.DateTime> SQSKS { get; set; }
        public Nullable<System.DateTime> SQSJS { get; set; }
        public string SCAddr { get; set; }
        public string SupSCFW { get; set; }
        public string SupJYXKZBH { get; set; }
        public string SupBAPZBH { get; set; }
        public string SupBAJYCS { get; set; }
        public string SupBAJYFS { get; set; }
        public string SupBAKFDZ { get; set; }
        public string SupBAJYFW { get; set; }
        public Nullable<System.DateTime> SupBARQ { get; set; }
    
        public virtual ICollection<T_CGMX> T_CGMX { get; set; }
        public virtual ICollection<T_CGMX> T_CGMX1 { get; set; }
        public virtual ICollection<T_HT> T_HT { get; set; }
        public virtual ICollection<T_KC> T_KC { get; set; }
        public virtual ICollection<T_KC> T_KC1 { get; set; }
        public virtual ICollection<T_QYZZ> T_QYZZ { get; set; }
        public virtual ICollection<T_RKMX> T_RKMX { get; set; }
        public virtual ICollection<T_RKMX> T_RKMX1 { get; set; }
        public virtual ICollection<T_SupPerson> T_SupPerson { get; set; }
        public virtual T_WhsQY T_WhsQY { get; set; }
        public virtual ICollection<T_YLCP> T_YLCP { get; set; }
        public virtual ICollection<T_YLCP> T_YLCP1 { get; set; }
        public virtual ICollection<T_YSMX> T_YSMX { get; set; }
        public virtual ICollection<T_YSMX> T_YSMX1 { get; set; }
    }
}
