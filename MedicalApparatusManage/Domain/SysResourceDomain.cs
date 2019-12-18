using MedicalApparatusManage.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;

namespace MedicalApparatusManage.Domain
{
    public class SysResourceDomain : BaseDomain<SysResource>
    {
        static SysResourceDomain _instance;
        private SysResourceDomain()
        {
        }
        //单例模式
        static public SysResourceDomain GetInstance()
        {
            if (_instance == null)
            {
                _instance = new SysResourceDomain();
            }
            return _instance;
        }

        public List<SysResource> PageSysResource(SysResource info, DateTime? startTime, DateTime? endTime, int pageIndex, int pageSize, out int pageCount, out int totalRecord)
        {
            Expression<Func<SysResource, bool>> where = PredicateBuilder.True<SysResource>();
            Func<SysResource, System.Int32> order = p => p.ResourceId;
            return base.GetPageInfo<System.Int32>(where, order, true, pageIndex, pageSize, out pageCount, out totalRecord);
        }

        /// <summary>
        /// 根据登陆用户ID获取菜单
        /// </summary>
        /// <param name="用户ID"></param>
        /// <returns></returns>
        public List<SysResource> GetPagesByUserId(int userid)
        {
            List<SysResource> list = new List<SysResource>();
            string sql = string.Format(@"SELECT * FROM SysResource WHERE ResourceId IN 
                        (SELECT ResourceId FROM SysRoleResource WHERE RoleId IN 
                        (SELECT RoleId FROM SysUser WHERE UserId = {0}))", userid);
            using (MedicalApparatusManageEntities gContext = new MedicalApparatusManageEntities())
            {
                try
                {
                    list = gContext.Database.SqlQuery<SysResource>(sql).ToList();
                }
                catch (Exception)
                {

                }
            }
            return list;
        }
    }
}