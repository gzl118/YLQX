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
    
    public partial class T_FP
    {
        public int FPID { get; set; }
        public string FPCODE { get; set; }
        public string KPQY { get; set; }
        public Nullable<System.DateTime> KPRQ { get; set; }
        public string SCRY { get; set; }
        public string SCSJ { get; set; }
        public Nullable<int> CGID { get; set; }
        public string BZ { get; set; }
        public string FPFJ { get; set; }
    
        public virtual T_CGD T_CGD { get; set; }
    }
}
