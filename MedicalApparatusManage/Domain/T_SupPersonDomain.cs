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
    public class T_SupPersonDomain : BaseDomain<T_SupPerson>
    {
        static T_SupPersonDomain _instance;
        private T_SupPersonDomain()
        {
        }
        //单例模式
        static public T_SupPersonDomain GetInstance()
        {
            if (_instance == null)
            {
                _instance = new T_SupPersonDomain();
            }
            return _instance;
        }

        public List<T_SupPerson> PageT_SupPerson(T_SupPerson info, DateTime? startTime, DateTime? endTime, int pageIndex, int pageSize, out int pageCount, out int totalRecord)
        {
            Expression<Func<T_SupPerson, bool>> where = PredicateBuilder.True<T_SupPerson>();
            if (!String.IsNullOrEmpty(info.PsMZ))
            {
                where = where.And(p => p.PsMZ.Contains(info.PsMZ.Trim()));
            }

            if (!String.IsNullOrEmpty(info.PsSFZ))
            {
                where = where.And(p => p.PsSFZ.Contains(info.PsSFZ.Trim()));
            }
            Func<T_SupPerson, System.Int32> order = p => p.PsID;
            return GetPageInfo<System.Int32>(where, order, true, pageIndex, pageSize, out pageCount, out totalRecord);
        }
        public List<T_SupPerson> GetAllT_SupPerson(T_SupPerson info)
        {
            Expression<Func<T_SupPerson, bool>> where = PredicateBuilder.True<T_SupPerson>();
            return base.GetAllModels<System.Int32>(where);
        }

        public List<T_SupPerson> GetPageInfo<S>(Expression<Func<T_SupPerson, bool>> where, Func<T_SupPerson, S> order, bool desc, int pageIndex, int pageSize, out int pageCount, out int totalRecord)
        {
            totalRecord = 0;
            pageCount = 0;
            List<T_SupPerson> list = new List<T_SupPerson>();
            using (MedicalApparatusManageEntities hContext1 = new MedicalApparatusManageEntities())
            {
                try
                {
                    DbSet<T_SupPerson> db = hContext1.Set<T_SupPerson>();
                    DbQuery<T_SupPerson> dbq = db.Include("T_SupQY");
                    totalRecord = db.Where(where.Compile()).Count();
                    if (totalRecord > 0)
                    {
                        pageCount = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(totalRecord / pageSize))) + 1;
                        if (desc)
                        {
                            list = dbq.Where(where.Compile()).OrderByDescending<T_SupPerson, S>(order).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
                        }
                        else
                        {
                            list = dbq.Where(where.Compile()).OrderBy<T_SupPerson, S>(order).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
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
