using MedicalApparatusManage.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;

namespace MedicalApparatusManage.Domain
{
    public class SysUserDomain : BaseDomain<SysUser>
    {
        static SysUserDomain _instance;
        private SysUserDomain()
        {
        }
        //单例模式
        static public SysUserDomain GetInstance()
        {
            if (_instance == null)
            {
                _instance = new SysUserDomain();
            }
            return _instance;
        }

        public List<SysUser> PageSysUser(SysUser info, DateTime? startTime, DateTime? endTime, int pageIndex, int pageSize, out int pageCount, out int totalRecord)
        {
            Expression<Func<SysUser, bool>> where = PredicateBuilder.True<SysUser>();
            Func<SysUser, System.Int32> order = p => p.UserId;
            return base.GetPageInfo<System.Int32>(where, order, true, pageIndex, pageSize, out pageCount, out totalRecord);
        }

        public List<SysUser> GetAllT_SysUser(SysUser info)
        {
            Expression<Func<SysUser, bool>> where = PredicateBuilder.True<SysUser>();
            return base.GetAllModels<System.Int32>(where);
        }

        /// <summary>
        /// 登陆
        /// </summary>
        /// <param name="用户ID"></param>
        /// <param name="密码"></param>
        /// <returns></returns>
        public string Login(string userAccount, string password, out SysUser user)
        {
            string result = "";
            user = null;
            using (MedicalApparatusManageEntities gContext = new MedicalApparatusManageEntities())
            {
                try
                {
                    var list = gContext.SysUser.Where(p => p.UserAccount == userAccount && p.UserPassword == password);
                    if (list.Count() > 0)
                    {
                        var liststate = gContext.SysUser.Where(p => p.UserAccount == userAccount && p.UserPassword == password && p.UserState == 1);
                        if (liststate.Count() > 0)
                        {
                            result = "success";
                            user = list.FirstOrDefault();
                        }
                        else
                        {
                            result = "该用户已禁用！";
                        }

                    }
                    else
                    {
                        result = "请检查用户名和密码是否正确！";
                    }
                }
                catch (Exception ex)
                {
                    result = ex.Message;
                }
            }
            return result;
        }

        public SysUser UpdatePwd(SysUser user, string password)
        {
            SysUser result = null;
            using (MedicalApparatusManageEntities gContext = new MedicalApparatusManageEntities())
            {
                try
                {
                    var list = gContext.SysUser.Where(p => p.UserAccount == user.UserAccount && p.UserPassword == user.UserPassword);
                    if (list.Count() > 0)
                    {
                        SysUser userNew = list.FirstOrDefault();
                        userNew.UserPassword = password;
                        gContext.SaveChanges();
                        result = userNew;
                    }
                }
                catch (Exception)
                {

                }
            }
            return result;
        }

        public int GetRoleByUserId(int userId)
        {
            int roleId = 0;
            using (MedicalApparatusManageEntities gContext = new MedicalApparatusManageEntities())
            {
                var role = gContext.Set<SysUserRole>().Where(p => p.UserId == userId).FirstOrDefault();
                if (role != null)
                {
                    roleId = role.RoleId.Value;
                }
            }
            return roleId;
        }
    }
}