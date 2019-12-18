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
    public class T_CPLXDomain : BaseDomain<T_CPLX>
    {
        static T_CPLXDomain _instance;
        private T_CPLXDomain()
        {
        }
        //单例模式
        static public T_CPLXDomain GetInstance()
        {
            if (_instance == null)
            {
                _instance = new T_CPLXDomain();
            }
            return _instance;
        }

        public List<T_CPLX> PageT_CPLX(T_CPLX info, DateTime? startTime, DateTime? endTime, int pageIndex, int pageSize, out int pageCount, out int totalRecord)
        {
            Expression<Func<T_CPLX, bool>> where = PredicateBuilder.True<T_CPLX>();
            Func<T_CPLX, System.Int32> order = p => p.LXID;
            return base.GetPageInfo<System.Int32>(where, order, true, pageIndex, pageSize, out pageCount, out totalRecord);
        }

        public List<T_CPLX> GetAllT_CPLX(T_CPLX info)
        {
            Expression<Func<T_CPLX, bool>> where = PredicateBuilder.True<T_CPLX>();
            return base.GetAllModels<System.Int32>(where);
        }

        public List<T_CPLX> GetPageInfo<S>(Expression<Func<T_CPLX, bool>> where, Func<T_CPLX, S> order, bool desc, int pageIndex, int pageSize, out int pageCount, out int totalRecord)
        {
            totalRecord = 0;
            pageCount = 0;
            List<T_CPLX> list = new List<T_CPLX>();
            using (MedicalApparatusManageEntities hContext1 = new MedicalApparatusManageEntities())
            {
                try
                {
                    DbSet<T_CPLX> db = hContext1.Set<T_CPLX>();
                    DbQuery<T_CPLX> dbq = db.Include("T_WhsQY");
                    totalRecord = db.Where(where.Compile()).Count();
                    if (totalRecord > 0)
                    {
                        pageCount = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(totalRecord / pageSize))) + 1;
                        if (desc)
                        {
                            list = dbq.Where(where.Compile()).OrderByDescending<T_CPLX, S>(order).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
                        }
                        else
                        {
                            list = dbq.Where(where.Compile()).OrderBy<T_CPLX, S>(order).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
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
