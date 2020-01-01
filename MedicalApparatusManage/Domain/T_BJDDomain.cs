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
    public class T_BJDDomain : BaseDomain<T_BJD>
    {
        static T_BJDDomain _instance;
        private T_BJDDomain()
        {
        }
        //单例模式
        static public T_BJDDomain GetInstance()
        {
            if (_instance == null)
            {
                _instance = new T_BJDDomain();
            }
            return _instance;
        }

        public List<T_BJD> PageT_BJD(T_BJD info, DateTime? startTime, DateTime? endTime, int pageIndex, int pageSize, out int pageCount, out int totalRecord)
        {
            Expression<Func<T_BJD, bool>> where = PredicateBuilder.True<T_BJD>();
            if (startTime != null)
            {
                where = where.And(p => p.BJRQ >= startTime.Value);
            }
            if (endTime != null)
            {
                where = where.And(p => p.BJRQ <= endTime.Value);
            }
            Func<T_BJD, System.Int32> order = p => p.BJID;
            return GetEntityPageInfo<System.Int32>(where, order, true, pageIndex, pageSize, out pageCount, out totalRecord);
        }




        public List<T_BJD> GetAllT_BJD(T_BJD info)
        {
            Expression<Func<T_BJD, bool>> where = PredicateBuilder.True<T_BJD>();
            return base.GetAllModels<System.Int32>(where);
        }

        public List<T_BJD> GetEntityPageInfo<S>(Expression<Func<T_BJD, bool>> where, Func<T_BJD, S> order, bool desc, int pageIndex, int pageSize, out int pageCount, out int totalRecord)
        {
            totalRecord = 0;
            pageCount = 0;
            List<T_BJD> list = new List<T_BJD>();
            using (MedicalApparatusManageEntities hContext1 = new MedicalApparatusManageEntities())
            {
                try
                {
                    DbSet<T_BJD> db = hContext1.Set<T_BJD>();
                    DbQuery<T_BJD> dbq = db.Include("T_SJZDLX");
                    totalRecord = db.Where(where.Compile()).Count();
                    if (totalRecord > 0)
                    {
                        pageCount = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(totalRecord / pageSize))) + 1;
                        if (desc)
                        {
                            list = dbq.Where(where.Compile()).OrderByDescending<T_BJD, S>(order).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
                        }
                        else
                        {
                            list = dbq.Where(where.Compile()).OrderBy<T_BJD, S>(order).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
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

        public int GetCount()
        {
            int count = 0;
            using (MedicalApparatusManageEntities hContext1 = new MedicalApparatusManageEntities())
            {
                try
                {
                    DbSet<T_BJD> db = hContext1.Set<T_BJD>();
                    count = db.Count();
                }
                catch (Exception)
                {

                }
                return count;
            }
        }
    }
}
