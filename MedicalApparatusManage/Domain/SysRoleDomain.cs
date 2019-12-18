using MedicalApparatusManage.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;


namespace MedicalApparatusManage.Domain
{
    public class SysRoleDomain : BaseDomain<SysRole>
    {
        static SysRoleDomain _instance;
        private SysRoleDomain()
        {
        }
        //单例模式
        static public SysRoleDomain GetInstance()
        {
            if (_instance == null)
            {
                _instance = new SysRoleDomain();
            }
            return _instance;
        }

        public List<SysRole> GetAllT_SysUser(SysRole info)
        {
            Expression<Func<SysRole, bool>> where = PredicateBuilder.True<SysRole>();
            return base.GetAllModels<System.Int32>(where);
        }

        public List<SysRole> PageSysRole(SysRole info, DateTime? startTime, DateTime? endTime, int pageIndex, int pageSize, out int pageCount, out int totalRecord)
        {
            Expression<Func<SysRole, bool>> where = PredicateBuilder.True<SysRole>();
            Func<SysRole, System.Int32> order = p => p.RoleId;
            return base.GetPageInfo<System.Int32>(where, order, true, pageIndex, pageSize, out pageCount, out totalRecord);
        }
    }
}
