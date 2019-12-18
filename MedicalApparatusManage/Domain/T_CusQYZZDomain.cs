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
    public class T_CusQYZZDomain : BaseDomain<T_CusQYZZ>
    {
        static T_CusQYZZDomain _instance;
        private T_CusQYZZDomain()
        {
        }
        //单例模式
        static public T_CusQYZZDomain GetInstance()
        {
            if (_instance == null)
            {
                _instance = new T_CusQYZZDomain();
            }
            return _instance;
        }

        public List<T_CusQYZZ> PageT_CusQYZZ(T_CusQYZZ info, DateTime? startTime, DateTime? endTime, int pageIndex, int pageSize, out int pageCount, out int totalRecord)
        {
            Expression<Func<T_CusQYZZ, bool>> where = PredicateBuilder.True<T_CusQYZZ>();
            if(!String.IsNullOrEmpty(info.ZZMC))
            {
                where = where.And(p => p.ZZMC.Contains(info.ZZMC));
            }
            if(!String.IsNullOrEmpty(info.ZZLX))
            {
                where = where.And(p => p.ZZLX.Contains(info.ZZLX));
            }
            Func<T_CusQYZZ, System.Int32> order = p => p.ZZID;
            return GetPageInfo<System.Int32>(where, order, true, pageIndex, pageSize, out pageCount, out totalRecord);
        }

        public List<T_CusQYZZ> GetAllT_BJD(T_CusQYZZ info)
        {
            Expression<Func<T_CusQYZZ, bool>> where = PredicateBuilder.True<T_CusQYZZ>();
            return base.GetAllModels<System.Int32>(where);
        }

        public List<T_CusQYZZ> GetPageInfo<S>(Expression<Func<T_CusQYZZ, bool>> where, Func<T_CusQYZZ, S> order, bool desc, int pageIndex, int pageSize, out int pageCount, out int totalRecord)
        {
            totalRecord = 0;
            pageCount = 0;
            List<T_CusQYZZ> list = new List<T_CusQYZZ>();
            using (MedicalApparatusManageEntities hContext1 = new MedicalApparatusManageEntities())
            {
                try
                {
                    DbSet<T_CusQYZZ> db = hContext1.Set<T_CusQYZZ>();
                    DbQuery<T_CusQYZZ> dbq = db.Include("T_CusQY");
                    totalRecord = db.Where(where.Compile()).Count();
                    if (totalRecord > 0)
                    {
                        pageCount = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(totalRecord / pageSize))) + 1;
                        if (desc)
                        {
                            list = dbq.Where(where.Compile()).OrderByDescending<T_CusQYZZ, S>(order).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
                        }
                        else
                        {
                            list = dbq.Where(where.Compile()).OrderBy<T_CusQYZZ, S>(order).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
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

        public List<T_CusQYZZ> GetQYZZByQyid(int qyid)
        {
            List<T_CusQYZZ> list = new List<T_CusQYZZ>();
            using (MedicalApparatusManageEntities hContext1 = new MedicalApparatusManageEntities())
            {
                try
                {
                    list = hContext1.Set<T_CusQYZZ>().Where(p => p.QYID == qyid).ToList();
                }
                catch (Exception)
                {

                }
            }
            return list;
        }
    }
}
