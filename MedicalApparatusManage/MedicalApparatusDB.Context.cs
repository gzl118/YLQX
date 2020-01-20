﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Data.Entity.Core.Objects;
    using System.Linq;
    
    public partial class MedicalApparatusManageEntities : DbContext
    {
        public MedicalApparatusManageEntities()
            : base("name=MedicalApparatusManageEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<ActivityInfo> ActivityInfo { get; set; }
        public virtual DbSet<sysdiagrams> sysdiagrams { get; set; }
        public virtual DbSet<SysFavorite> SysFavorite { get; set; }
        public virtual DbSet<SysResource> SysResource { get; set; }
        public virtual DbSet<SysRole> SysRole { get; set; }
        public virtual DbSet<SysRoleResource> SysRoleResource { get; set; }
        public virtual DbSet<SysUser> SysUser { get; set; }
        public virtual DbSet<SysUserRole> SysUserRole { get; set; }
        public virtual DbSet<T_BJD> T_BJD { get; set; }
        public virtual DbSet<T_BJDate> T_BJDate { get; set; }
        public virtual DbSet<T_CGD> T_CGD { get; set; }
        public virtual DbSet<T_CGMX> T_CGMX { get; set; }
        public virtual DbSet<T_CK> T_CK { get; set; }
        public virtual DbSet<T_CKD> T_CKD { get; set; }
        public virtual DbSet<T_CKMX> T_CKMX { get; set; }
        public virtual DbSet<T_CPLX> T_CPLX { get; set; }
        public virtual DbSet<T_CusPerson> T_CusPerson { get; set; }
        public virtual DbSet<T_CusQY> T_CusQY { get; set; }
        public virtual DbSet<T_CusQYZZ> T_CusQYZZ { get; set; }
        public virtual DbSet<T_FKD> T_FKD { get; set; }
        public virtual DbSet<T_FP> T_FP { get; set; }
        public virtual DbSet<T_HT> T_HT { get; set; }
        public virtual DbSet<T_JYFW> T_JYFW { get; set; }
        public virtual DbSet<T_KC> T_KC { get; set; }
        public virtual DbSet<T_Person> T_Person { get; set; }
        public virtual DbSet<T_QYSH> T_QYSH { get; set; }
        public virtual DbSet<T_QYZZ> T_QYZZ { get; set; }
        public virtual DbSet<T_RKD> T_RKD { get; set; }
        public virtual DbSet<T_RKMX> T_RKMX { get; set; }
        public virtual DbSet<T_SHFW> T_SHFW { get; set; }
        public virtual DbSet<T_SJZDFL> T_SJZDFL { get; set; }
        public virtual DbSet<T_SJZDLX> T_SJZDLX { get; set; }
        public virtual DbSet<T_SupPerson> T_SupPerson { get; set; }
        public virtual DbSet<T_SupQY> T_SupQY { get; set; }
        public virtual DbSet<T_THD> T_THD { get; set; }
        public virtual DbSet<T_THMX> T_THMX { get; set; }
        public virtual DbSet<T_WhsQY> T_WhsQY { get; set; }
        public virtual DbSet<T_WhsQYZZ> T_WhsQYZZ { get; set; }
        public virtual DbSet<T_XSD> T_XSD { get; set; }
        public virtual DbSet<T_XSHT> T_XSHT { get; set; }
        public virtual DbSet<T_XSMX> T_XSMX { get; set; }
        public virtual DbSet<T_YLCP> T_YLCP { get; set; }
        public virtual DbSet<T_YLCPZZ> T_YLCPZZ { get; set; }
        public virtual DbSet<T_YSD> T_YSD { get; set; }
        public virtual DbSet<T_YSMX> T_YSMX { get; set; }
        public virtual DbSet<T_PackingUnit> T_PackingUnit { get; set; }
        public virtual DbSet<T_SHD> T_SHD { get; set; }
        public virtual DbSet<T_SHMX> T_SHMX { get; set; }
        public virtual DbSet<T_CKCollect> T_CKCollect { get; set; }
    
        public virtual int proc_get_bjd()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("proc_get_bjd");
        }
    
        public virtual int sp_alterdiagram(string diagramname, Nullable<int> owner_id, Nullable<int> version, byte[] definition)
        {
            var diagramnameParameter = diagramname != null ?
                new ObjectParameter("diagramname", diagramname) :
                new ObjectParameter("diagramname", typeof(string));
    
            var owner_idParameter = owner_id.HasValue ?
                new ObjectParameter("owner_id", owner_id) :
                new ObjectParameter("owner_id", typeof(int));
    
            var versionParameter = version.HasValue ?
                new ObjectParameter("version", version) :
                new ObjectParameter("version", typeof(int));
    
            var definitionParameter = definition != null ?
                new ObjectParameter("definition", definition) :
                new ObjectParameter("definition", typeof(byte[]));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("sp_alterdiagram", diagramnameParameter, owner_idParameter, versionParameter, definitionParameter);
        }
    
        public virtual int sp_creatediagram(string diagramname, Nullable<int> owner_id, Nullable<int> version, byte[] definition)
        {
            var diagramnameParameter = diagramname != null ?
                new ObjectParameter("diagramname", diagramname) :
                new ObjectParameter("diagramname", typeof(string));
    
            var owner_idParameter = owner_id.HasValue ?
                new ObjectParameter("owner_id", owner_id) :
                new ObjectParameter("owner_id", typeof(int));
    
            var versionParameter = version.HasValue ?
                new ObjectParameter("version", version) :
                new ObjectParameter("version", typeof(int));
    
            var definitionParameter = definition != null ?
                new ObjectParameter("definition", definition) :
                new ObjectParameter("definition", typeof(byte[]));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("sp_creatediagram", diagramnameParameter, owner_idParameter, versionParameter, definitionParameter);
        }
    
        public virtual int sp_dropdiagram(string diagramname, Nullable<int> owner_id)
        {
            var diagramnameParameter = diagramname != null ?
                new ObjectParameter("diagramname", diagramname) :
                new ObjectParameter("diagramname", typeof(string));
    
            var owner_idParameter = owner_id.HasValue ?
                new ObjectParameter("owner_id", owner_id) :
                new ObjectParameter("owner_id", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("sp_dropdiagram", diagramnameParameter, owner_idParameter);
        }
    
        public virtual ObjectResult<sp_helpdiagramdefinition_Result> sp_helpdiagramdefinition(string diagramname, Nullable<int> owner_id)
        {
            var diagramnameParameter = diagramname != null ?
                new ObjectParameter("diagramname", diagramname) :
                new ObjectParameter("diagramname", typeof(string));
    
            var owner_idParameter = owner_id.HasValue ?
                new ObjectParameter("owner_id", owner_id) :
                new ObjectParameter("owner_id", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<sp_helpdiagramdefinition_Result>("sp_helpdiagramdefinition", diagramnameParameter, owner_idParameter);
        }
    
        public virtual ObjectResult<sp_helpdiagrams_Result> sp_helpdiagrams(string diagramname, Nullable<int> owner_id)
        {
            var diagramnameParameter = diagramname != null ?
                new ObjectParameter("diagramname", diagramname) :
                new ObjectParameter("diagramname", typeof(string));
    
            var owner_idParameter = owner_id.HasValue ?
                new ObjectParameter("owner_id", owner_id) :
                new ObjectParameter("owner_id", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<sp_helpdiagrams_Result>("sp_helpdiagrams", diagramnameParameter, owner_idParameter);
        }
    
        public virtual int sp_renamediagram(string diagramname, Nullable<int> owner_id, string new_diagramname)
        {
            var diagramnameParameter = diagramname != null ?
                new ObjectParameter("diagramname", diagramname) :
                new ObjectParameter("diagramname", typeof(string));
    
            var owner_idParameter = owner_id.HasValue ?
                new ObjectParameter("owner_id", owner_id) :
                new ObjectParameter("owner_id", typeof(int));
    
            var new_diagramnameParameter = new_diagramname != null ?
                new ObjectParameter("new_diagramname", new_diagramname) :
                new ObjectParameter("new_diagramname", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("sp_renamediagram", diagramnameParameter, owner_idParameter, new_diagramnameParameter);
        }
    
        public virtual int sp_upgraddiagrams()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("sp_upgraddiagrams");
        }
    }
}
