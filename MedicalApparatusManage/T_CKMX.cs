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
    
    public partial class T_CKMX
    {
        public int CKMID { get; set; }
        public int CPID { get; set; }
        public int CKID { get; set; }
        public Nullable<int> CKDID { get; set; }
        public Nullable<double> CPNUM { get; set; }
        public string BZ { get; set; }
        public string GUID { get; set; }
        public Nullable<double> CPPRICE { get; set; }
        public string CPPH { get; set; }
        public Nullable<System.DateTime> CPSCRQ { get; set; }
        public Nullable<System.DateTime> CPYXQ { get; set; }
    
        public virtual T_CK T_CK { get; set; }
        public virtual T_CKD T_CKD { get; set; }
        public virtual T_YLCP T_YLCP { get; set; }
    }
}
