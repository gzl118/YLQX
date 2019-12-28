using MedicalApparatusManage.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;


namespace MedicalApparatusManage.Domain
{
    public class T_YLCPDomain : BaseDomain<T_YLCP>
    {
        static T_YLCPDomain _instance;
        private T_YLCPDomain()
        {
        }
        //单例模式
        static public T_YLCPDomain GetInstance()
        {
            if (_instance == null)
            {
                _instance = new T_YLCPDomain();
            }
            return _instance;
        }

        public List<T_YLCP> PageT_YLCP(T_YLCP info, DateTime? startTime, DateTime? endTime, int pageIndex, int pageSize, out int pageCount, out int totalRecord)
        {
            Expression<Func<T_YLCP, bool>> where = PredicateBuilder.True<T_YLCP>();
            if (!String.IsNullOrEmpty(info.CPMC))
            {
                where = where.And(p => p.CPMC.Contains(info.CPMC));
            }
            if (info.CPSCQYID.HasValue)
            {
                where = where.And(p => p.CPSCQYID == info.CPSCQYID);
            }
            Func<T_YLCP, System.Int32> order = p => p.CPID;
            return GetPageInfo<System.Int32>(where, order, true, pageIndex, pageSize, out pageCount, out totalRecord);
        }

        public List<T_YLCP> GetAllT_YLCP(T_YLCP info)
        {
            Expression<Func<T_YLCP, bool>> where = PredicateBuilder.True<T_YLCP>();
            if (info != null && info.CPGYSID != null)
            {
                where = where.And(p => p.CPGYSID == info.CPGYSID.Value);
            }
            if (info != null && info.CPStatus != null)
            {
                where = where.And(p => p.CPStatus == info.CPStatus.Value);
            }
            return base.GetAllModels<System.Int32>(where);
        }

        public List<T_YLCP> GetPageInfo<S>(Expression<Func<T_YLCP, bool>> where, Func<T_YLCP, S> order, bool desc, int pageIndex, int pageSize, out int pageCount, out int totalRecord)
        {
            totalRecord = 0;
            pageCount = 0;
            List<T_YLCP> list = new List<T_YLCP>();
            using (MedicalApparatusManageEntities hContext1 = new MedicalApparatusManageEntities())
            {
                try
                {
                    DbSet<T_YLCP> db = hContext1.Set<T_YLCP>();
                    DbQuery<T_YLCP> dbq = db.Include("T_SupQY").Include("T_CPLX").Include("T_SupQY1");
                    totalRecord = db.Where(where.Compile()).Count();
                    if (totalRecord > 0)
                    {
                        pageCount = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(totalRecord / pageSize))) + 1;
                        if (desc)
                        {
                            list = dbq.Where(where.Compile()).OrderByDescending<T_YLCP, S>(order).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
                        }
                        else
                        {
                            list = dbq.Where(where.Compile()).OrderBy<T_YLCP, S>(order).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
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

        public string GetCpOrderNum(string prefix, SysUser user)
        {
            int count = 0;
            using (MedicalApparatusManageEntities hContext1 = new MedicalApparatusManageEntities())
            {
                try
                {
                    count = hContext1.Set<T_YLCP>().Where(p => p.CPLRR == user.UserAccount).Count() + 1;
                }
                catch (Exception ex)
                {

                }
            }
            string result = prefix + DateTime.Now.ToString("yyMMdd") + user.UserId.ToString().PadLeft(3, '0') + count.ToString().PadLeft(5, '0');
            return result;
        }

        public int CpSh(int cpid, int cpstatus)
        {
            int result = 0;
            using (MedicalApparatusManageEntities hContext1 = new MedicalApparatusManageEntities())
            {
                try
                {
                    var model = hContext1.Set<T_YLCP>().Find(cpid);
                    model.CPStatus = cpstatus;
                    hContext1.SaveChanges();
                    result = 1;
                }
                catch (Exception ex)
                {

                }
            }
            return result;
        }

        public void OperCpzz(int cpid, string cpzzFiles)
        {
            if (string.IsNullOrEmpty(cpzzFiles) || cpid == 0)
            {
                return;
            }
            string[] files = cpzzFiles.Split(';');
            List<T_YLCPZZ> addFiles = new List<T_YLCPZZ>();
            using (MedicalApparatusManageEntities hContext1 = new MedicalApparatusManageEntities())
            {
                try
                {
                    foreach (var item in files)
                    {
                        if (!string.IsNullOrEmpty(item))
                        {
                            T_YLCPZZ qyzz = new T_YLCPZZ() { ZZFJ = item, CPID = cpid };
                            addFiles.Add(qyzz);
                        }
                    }
                    //先删除后增加
                    DbSet<T_YLCPZZ> db = hContext1.Set<T_YLCPZZ>();
                    var deleteFiles = db.Where(p => p.CPID == cpid);
                    db.RemoveRange(deleteFiles);
                    db.AddRange(addFiles);
                    hContext1.SaveChanges();
                }
                catch (Exception ex)
                {

                }
            }
        }

        public T_YLCP GetCpDetailsById(int cpid)
        {
            T_YLCP cp = new T_YLCP();
            using (MedicalApparatusManageEntities hContext1 = new MedicalApparatusManageEntities())
            {
                try
                {
                    cp = base.GetModelById(cpid);
                    if (cp != null)
                    {
                        if (cp.CPSCQYID.HasValue)
                        {
                            cp.T_SupQY1 = hContext1.Set<T_SupQY>().Find(cp.CPSCQYID);
                        }
                        if (cp.CPGYSID.HasValue)
                        {
                            cp.T_SupQY = hContext1.Set<T_SupQY>().Find(cp.CPGYSID);
                        }
                    }
                }
                catch (Exception ex)
                {

                }
            }
            return cp;
        }
    }
}
