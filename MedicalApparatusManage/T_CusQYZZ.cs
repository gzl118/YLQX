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
    
    public partial class T_CusQYZZ
    {
        public int ZZID { get; set; }
        public string ZZMC { get; set; }
        public string ZZLX { get; set; }
        public string ZZBH { get; set; }
        public string ZZFJ { get; set; }
        public string ZZYX { get; set; }
        public string ZZBZ { get; set; }
        public Nullable<int> QYID { get; set; }
    
        public virtual T_CusQY T_CusQY { get; set; }
    }
}
