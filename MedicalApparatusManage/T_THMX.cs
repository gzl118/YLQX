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
    
    public partial class T_THMX
    {
        public int MXID { get; set; }
        public int CPID { get; set; }
        public int THID { get; set; }
        public Nullable<double> CPNUM { get; set; }
        public string BZ { get; set; }
        public int CKID { get; set; }
        public Nullable<int> FLAG { get; set; }
    
        public virtual T_CK T_CK { get; set; }
        public virtual T_THD T_THD { get; set; }
        public virtual T_YLCP T_YLCP { get; set; }
    }
}