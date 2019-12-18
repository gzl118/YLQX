using MedicalApparatusManage.Common;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;
using System.Web;


namespace MedicalApparatusManage.Domain
{
    public class SysUserRoleDomain : BaseDomain<SysUserRole>
    {
        static SysUserRoleDomain _instance;
        private SysUserRoleDomain()
        {
        }
        //单例模式
        static public SysUserRoleDomain GetInstance()
        {
            if (_instance == null)
            {
                _instance = new SysUserRoleDomain();
            }
            return _instance;
        }

        public List<SysUserRole> PageSysUserRole(SysUserRole info, DateTime? startTime, DateTime? endTime, int pageIndex, int pageSize, out int pageCount, out int totalRecord)
        {
            Expression<Func<SysUserRole, bool>> where = PredicateBuilder.True<SysUserRole>();
            Func<SysUserRole, System.Int32> order = p => p.ActionId;
            return GetPageInfo<System.Int32>(where, order, true, pageIndex, pageSize, out pageCount, out totalRecord);
        }

        public List<SysUserRole> GetPageInfo<S>(Expression<Func<SysUserRole, bool>> where, Func<SysUserRole, S> order, bool desc, int pageIndex, int pageSize, out int pageCount, out int totalRecord)
        {
            totalRecord = 0;
            pageCount = 0;
            List<SysUserRole> list = new List<SysUserRole>();
            using (MedicalApparatusManageEntities hContext1 = new MedicalApparatusManageEntities())
            {
                try
                {
                    DbSet<SysUserRole> db = hContext1.Set<SysUserRole>();
                    DbQuery<SysUserRole> dbq = db.Include("SysUser").Include("SysRole");
                    totalRecord = db.Where(where.Compile()).Count();
                    if (totalRecord > 0)
                    {
                        pageCount = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(totalRecord / pageSize))) + 1;
                        if (desc)
                        {
                            list = dbq.Where(where.Compile()).OrderByDescending<SysUserRole, S>(order).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
                        }
                        else
                        {
                            list = dbq.Where(where.Compile()).OrderBy<SysUserRole, S>(order).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
                        }
                    }
                }
                catch (Exception)
                {
                    return null;
                }
                return list;
            }
        }
    }
}
