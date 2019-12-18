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
    public class T_SJZDLXDomain : BaseDomain<T_SJZDLX>
    {
        static T_SJZDLXDomain _instance;
        private T_SJZDLXDomain()
        {
        }
        //单例模式
        static public T_SJZDLXDomain GetInstance()
        {
            if (_instance == null)
            {
                _instance = new T_SJZDLXDomain();
            }
            return _instance;
        }

        public List<T_SJZDLX> PageT_SJZDLX(T_SJZDLX info, DateTime? startTime, DateTime? endTime, int pageIndex, int pageSize, out int pageCount, out int totalRecord)
        {
            Expression<Func<T_SJZDLX, bool>> where = PredicateBuilder.True<T_SJZDLX>();
            Func<T_SJZDLX, System.Int32> order = p => p.LXID;
            return base.GetPageInfo<System.Int32>(where, order, true, pageIndex, pageSize, out pageCount, out totalRecord);
        }

        public List<T_SJZDLX> GetAllT_SJZDLX(T_SJZDLX info)
        {
            Expression<Func<T_SJZDLX, bool>> where = PredicateBuilder.True<T_SJZDLX>();
            return base.GetAllModels<System.Int32>(where);
        }

        public List<T_SJZDLX> GetPageInfo<S>(Expression<Func<T_SJZDLX, bool>> where, Func<T_SJZDLX, S> order, bool desc, int pageIndex, int pageSize, out int pageCount, out int totalRecord)
        {
            totalRecord = 0;
            pageCount = 0;
            List<T_SJZDLX> list = new List<T_SJZDLX>();
            using (MedicalApparatusManageEntities hContext1 = new MedicalApparatusManageEntities())
            {
                try
                {
                    DbSet<T_SJZDLX> db = hContext1.Set<T_SJZDLX>();
                    DbQuery<T_SJZDLX> dbq = db.Include("T_WhsQY");
                    totalRecord = db.Where(where.Compile()).Count();
                    if (totalRecord > 0)
                    {
                        pageCount = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(totalRecord / pageSize))) + 1;
                        if (desc)
                        {
                            list = dbq.Where(where.Compile()).OrderByDescending<T_SJZDLX, S>(order).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
                        }
                        else
                        {
                            list = dbq.Where(where.Compile()).OrderBy<T_SJZDLX, S>(order).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
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
