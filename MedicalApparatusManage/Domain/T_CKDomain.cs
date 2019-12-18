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
    public class T_CKDomain : BaseDomain<T_CK>
    {
        static T_CKDomain _instance;
        private T_CKDomain()
        {
        }
        //单例模式
        static public T_CKDomain GetInstance()
        {
            if (_instance == null)
            {
                _instance = new T_CKDomain();
            }
            return _instance;
        }

        public List<T_CK> PageT_CK(T_CK info, DateTime? startTime, DateTime? endTime, int pageIndex, int pageSize, out int pageCount, out int totalRecord)
        {
            Expression<Func<T_CK, bool>> where = PredicateBuilder.True<T_CK>();
            if(!String.IsNullOrEmpty(info.CKMC))
            {
                where = where.And(p => p.CKMC.Contains(info.CKMC));
            }
            if(!String.IsNullOrEmpty(info.CKGLY))
            {
                where = where.And(p => p.CKGLY.Contains(info.CKGLY));
            }
            Func<T_CK, System.Int32> order = p => p.CKID;
            return base.GetPageInfo<System.Int32>(where, order, true, pageIndex, pageSize, out pageCount, out totalRecord);
        }

        public List<T_CK> GetAllT_CK(T_CK info)
        {
            Expression<Func<T_CK, bool>> where = PredicateBuilder.True<T_CK>();
            return base.GetAllModels<System.Int32>(where);
        }

        public List<T_CK> GetPageInfo<S>(Expression<Func<T_CK, bool>> where, Func<T_CK, S> order, bool desc, int pageIndex, int pageSize, out int pageCount, out int totalRecord)
        {
            totalRecord = 0;
            pageCount = 0;
            List<T_CK> list = new List<T_CK>();
            using (MedicalApparatusManageEntities hContext1 = new MedicalApparatusManageEntities())
            {
                try
                {
                    DbSet<T_CK> db = hContext1.Set<T_CK>();
                    DbQuery<T_CK> dbq = db.Include("T_WhsQY");
                    totalRecord = db.Where(where.Compile()).Count();
                    if (totalRecord > 0)
                    {
                        pageCount = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(totalRecord / pageSize))) + 1;
                        if (desc)
                        {
                            list = dbq.Where(where.Compile()).OrderByDescending<T_CK, S>(order).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
                        }
                        else
                        {
                            list = dbq.Where(where.Compile()).OrderBy<T_CK, S>(order).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
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
