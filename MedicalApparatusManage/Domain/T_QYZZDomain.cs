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
    public class T_QYZZDomain : BaseDomain<T_QYZZ>
    {
        static T_QYZZDomain _instance;
        private T_QYZZDomain()
        {
        }
        //单例模式
        static public T_QYZZDomain GetInstance()
        {
            if (_instance == null)
            {
                _instance = new T_QYZZDomain();
            }
            return _instance;
        }

        public List<T_QYZZ> PageT_QYZZ(T_QYZZ info, DateTime? startTime, DateTime? endTime, int pageIndex, int pageSize, out int pageCount, out int totalRecord)
        {
            Expression<Func<T_QYZZ, bool>> where = PredicateBuilder.True<T_QYZZ>();
            if (!String.IsNullOrEmpty(info.ZZMC))
            {
                where = where.And(p => p.ZZMC.Contains(info.ZZMC));
            }
            if (!String.IsNullOrEmpty(info.ZZLX))
            {
                where = where.And(p => p.ZZLX.Contains(info.ZZLX));
            }
            Func<T_QYZZ, System.Int32> order = p => p.ZZID;
            return GetPageInfo<System.Int32>(where, order, true, pageIndex, pageSize, out pageCount, out totalRecord);
        }

        public List<T_QYZZ> GetAllT_QYZZ(T_QYZZ info)
        {
            Expression<Func<T_QYZZ, bool>> where = PredicateBuilder.True<T_QYZZ>();
            return base.GetAllModels<System.Int32>(where);
        }

        public List<T_QYZZ> GetPageInfo<S>(Expression<Func<T_QYZZ, bool>> where, Func<T_QYZZ, S> order, bool desc, int pageIndex, int pageSize, out int pageCount, out int totalRecord)
        {
            totalRecord = 0;
            pageCount = 0;
            List<T_QYZZ> list = new List<T_QYZZ>();
            using (MedicalApparatusManageEntities hContext1 = new MedicalApparatusManageEntities())
            {
                try
                {
                    DbSet<T_QYZZ> db = hContext1.Set<T_QYZZ>();
                    DbQuery<T_QYZZ> dbq = db.Include("T_SupQY");
                    totalRecord = db.Where(where.Compile()).Count();
                    if (totalRecord > 0)
                    {
                        pageCount = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(totalRecord / pageSize))) + 1;
                        if (desc)
                        {
                            list = dbq.Where(where.Compile()).OrderByDescending<T_QYZZ, S>(order).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
                        }
                        else
                        {
                            list = dbq.Where(where.Compile()).OrderBy<T_QYZZ, S>(order).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
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

        public List<T_QYZZ> GetQYZZByQyid(int qyid)
        {
            List<T_QYZZ> list = new List<T_QYZZ>();
            using (MedicalApparatusManageEntities hContext1 = new MedicalApparatusManageEntities())
            {
                try
                {
                    list = hContext1.Set<T_QYZZ>().Where(p => p.QYID == qyid).ToList();
                }
                catch (Exception)
                {

                }
            }
            return list;
        }
    }
}
