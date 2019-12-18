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
    public class T_CusPersonDomain : BaseDomain<T_CusPerson>
    {
        static T_CusPersonDomain _instance;
        private T_CusPersonDomain()
        {
        }
        //单例模式
        static public T_CusPersonDomain GetInstance()
        {
            if (_instance == null)
            {
                _instance = new T_CusPersonDomain();
            }
            return _instance;
        }

        public List<T_CusPerson> PageT_CusPerson(T_CusPerson info, DateTime? startTime, DateTime? endTime, int pageIndex, int pageSize, out int pageCount, out int totalRecord)
        {
            Expression<Func<T_CusPerson, bool>> where = PredicateBuilder.True<T_CusPerson>();

            if (!String.IsNullOrEmpty(info.PsMZ))
            {
                where = where.And(p => p.PsMZ.Contains(info.PsMZ.Trim()));
            }

            if (!String.IsNullOrEmpty(info.PsSFZ))
            {
                where = where.And(p => p.PsSFZ.Contains(info.PsSFZ.Trim()));
            }

            Func<T_CusPerson, System.Int32> order = p => p.PsID;
            return GetPageInfo<System.Int32>(where, order, true, pageIndex, pageSize, out pageCount, out totalRecord);
        }

        public List<T_CusPerson> GetAllT_CusPerson(T_CusPerson info)
        {
            Expression<Func<T_CusPerson, bool>> where = PredicateBuilder.True<T_CusPerson>();
            return base.GetAllModels<System.Int32>(where);
        }

        public List<T_CusPerson> GetPageInfo<S>(Expression<Func<T_CusPerson, bool>> where, Func<T_CusPerson, S> order, bool desc, int pageIndex, int pageSize, out int pageCount, out int totalRecord)
        {
            totalRecord = 0;
            pageCount = 0;
            List<T_CusPerson> list = new List<T_CusPerson>();
            using (MedicalApparatusManageEntities hContext1 = new MedicalApparatusManageEntities())
            {
                try
                {
                    DbSet<T_CusPerson> db = hContext1.Set<T_CusPerson>();
                    DbQuery<T_CusPerson> dbq = db.Include("T_CusQY");
                    totalRecord = db.Where(where.Compile()).Count();
                    if (totalRecord > 0)
                    {
                        pageCount = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(totalRecord / pageSize))) + 1;
                        if (desc)
                        {
                            list = dbq.Where(where.Compile()).OrderByDescending<T_CusPerson, S>(order).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
                        }
                        else
                        {
                            list = dbq.Where(where.Compile()).OrderBy<T_CusPerson, S>(order).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
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
