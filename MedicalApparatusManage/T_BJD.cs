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
    
    public partial class T_BJD
    {
        public int BJID { get; set; }
        public Nullable<System.DateTime> BJRQ { get; set; }
        public Nullable<int> BJLXID { get; set; }
        public string BJNR { get; set; }
        public string BZ { get; set; }
    
        public virtual T_SJZDLX T_SJZDLX { get; set; }
    }
}