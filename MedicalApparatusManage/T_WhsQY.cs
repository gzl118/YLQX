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
    
    public partial class T_WhsQY
    {
        public T_WhsQY()
        {
            this.T_CusQY = new HashSet<T_CusQY>();
            this.T_Person = new HashSet<T_Person>();
            this.T_SupQY = new HashSet<T_SupQY>();
            this.T_WhsQYZZ = new HashSet<T_WhsQYZZ>();
        }
    
        public int WhsID { get; set; }
        public string WhsMC { get; set; }
        public string WhsFSDB { get; set; }
        public Nullable<System.DateTime> WhsZCSJ { get; set; }
        public string WhsZCDZ { get; set; }
        public string WhsZZJGDM { get; set; }
        public string WhsDWXZ { get; set; }
        public string WhsJXFW { get; set; }
        public string WhsBZ { get; set; }
        public Nullable<System.DateTime> WhsJYKSSJ { get; set; }
        public Nullable<System.DateTime> WhsJXJSSJ { get; set; }
        public string WhsKHH { get; set; }
        public string WhsZH { get; set; }
        public string WhsJYXKZ { get; set; }
        public Nullable<double> WhsZCZB { get; set; }
        public string WhsXKZBH { get; set; }
        public string WhsJYFS { get; set; }
        public string WhsJYCS { get; set; }
        public string WhsKFDZ { get; set; }
        public string WhsJYFW { get; set; }
        public Nullable<System.DateTime> WhsFZRQ { get; set; }
        public Nullable<System.DateTime> WhsYXQKS { get; set; }
        public Nullable<System.DateTime> WhsYXQJS { get; set; }
        public string WhsZZSC { get; set; }
        public Nullable<System.DateTime> JYXKZFZRQ { get; set; }
        public Nullable<System.DateTime> JYXKZYXQKS { get; set; }
        public Nullable<System.DateTime> JYXKZYXQJS { get; set; }
    
        public virtual ICollection<T_CusQY> T_CusQY { get; set; }
        public virtual ICollection<T_Person> T_Person { get; set; }
        public virtual ICollection<T_SupQY> T_SupQY { get; set; }
        public virtual ICollection<T_WhsQYZZ> T_WhsQYZZ { get; set; }
    }
}
