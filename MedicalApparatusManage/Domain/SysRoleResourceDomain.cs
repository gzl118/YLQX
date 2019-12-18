using MedicalApparatusManage.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;


namespace MedicalApparatusManage.Domain
{
    public class SysRoleResourceDomain : BaseDomain<SysRoleResource>
    {
        static SysRoleResourceDomain _instance;
        private SysRoleResourceDomain()
        {
        }
        //单例模式
        static public SysRoleResourceDomain GetInstance()
        {
            if (_instance == null)
            {
                _instance = new SysRoleResourceDomain();
            }
            return _instance;
        }

        public List<SysRoleResource> PageSysRoleResource(SysRoleResource info, DateTime? startTime, DateTime? endTime, int pageIndex, int pageSize, out int pageCount, out int totalRecord)
        {
            Expression<Func<SysRoleResource, bool>> where = PredicateBuilder.True<SysRoleResource>();
            Func<SysRoleResource, System.Int32> order = p => p.AuthorId;
            return base.GetPageInfo<System.Int32>(where, order, true, pageIndex, pageSize, out pageCount, out totalRecord);
        }
    }
}
