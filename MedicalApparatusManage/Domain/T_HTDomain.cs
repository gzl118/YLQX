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
    public class T_HTDomain : BaseDomain<T_HT>
    {
        static T_HTDomain _instance;
        private T_HTDomain()
        {
        }
        //单例模式
        static public T_HTDomain GetInstance()
        {
            if (_instance == null)
            {
                _instance = new T_HTDomain();
            }
            return _instance;
        }

        public List<T_HT> PageT_HT(T_HT info, DateTime? startTime, DateTime? endTime, int pageIndex, int pageSize, out int pageCount, out int totalRecord)
        {
            Expression<Func<T_HT, bool>> where = PredicateBuilder.True<T_HT>();
            if(!String.IsNullOrEmpty(info.HTBH))
            {
                where = where.And(p => p.HTBH.Contains(info.HTBH));
            }

            if (!String.IsNullOrEmpty(info.HTMC))
            {
                where = where.And(p => p.HTMC.Contains(info.HTMC));
            }
            Func<T_HT, System.Int32> order = p => p.HTID;
            return GetPageInfo<System.Int32>(where, order, true, pageIndex, pageSize, out pageCount, out totalRecord);
        }

        public List<T_HT> GetAllT_HT(T_HT info)
        {
            Expression<Func<T_HT, bool>> where = PredicateBuilder.True<T_HT>();
            return base.GetAllModels<System.Int32>(where);
        }

        public List<T_HT> GetPageInfo<S>(Expression<Func<T_HT, bool>> where, Func<T_HT, S> order, bool desc, int pageIndex, int pageSize, out int pageCount, out int totalRecord)
        {
            totalRecord = 0;
            pageCount = 0;
            List<T_HT> list = new List<T_HT>();
            using (MedicalApparatusManageEntities hContext1 = new MedicalApparatusManageEntities())
            {
                try
                {
                    DbSet<T_HT> db = hContext1.Set<T_HT>();
                    DbQuery<T_HT> dbq = db.Include("T_SupQY");
                    totalRecord = db.Where(where.Compile()).Count();
                    if (totalRecord > 0)
                    {
                        pageCount = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(totalRecord / pageSize))) + 1;
                        if (desc)
                        {
                            list = dbq.Where(where.Compile()).OrderByDescending<T_HT, S>(order).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
                        }
                        else
                        {
                            list = dbq.Where(where.Compile()).OrderBy<T_HT, S>(order).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
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
