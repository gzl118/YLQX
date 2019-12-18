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
    public class T_FKDDomain : BaseDomain<T_FKD>
    {
        static T_FKDDomain _instance;
        private T_FKDDomain()
        {
        }
        //单例模式
        static public T_FKDDomain GetInstance()
        {
            if (_instance == null)
            {
                _instance = new T_FKDDomain();
            }
            return _instance;
        }

        public List<T_FKD> PageT_FKD(T_FKD info, DateTime? startTime, DateTime? endTime, int pageIndex, int pageSize, out int pageCount, out int totalRecord)
        {
            Expression<Func<T_FKD, bool>> where = PredicateBuilder.True<T_FKD>();
            if(startTime != null && endTime != null)
            {
                where = where.And(p => p.FKRQ >= startTime.Value);
                where = where.And(p => p.FKRQ <= endTime.Value);
            }
            Func<T_FKD, System.Int32> order = p => p.FKID;
            return GetPageInfo<System.Int32>(where, order, true, pageIndex, pageSize, out pageCount, out totalRecord);
        }

        public List<T_FKD> GetAllT_BJD(T_FKD info)
        {
            Expression<Func<T_FKD, bool>> where = PredicateBuilder.True<T_FKD>();
            return base.GetAllModels<System.Int32>(where);
        }

        public List<T_FKD> GetPageInfo<S>(Expression<Func<T_FKD, bool>> where, Func<T_FKD, S> order, bool desc, int pageIndex, int pageSize, out int pageCount, out int totalRecord)
        {
            totalRecord = 0;
            pageCount = 0;
            List<T_FKD> list = new List<T_FKD>();
            using (MedicalApparatusManageEntities hContext1 = new MedicalApparatusManageEntities())
            {
                try
                {
                    DbSet<T_FKD> db = hContext1.Set<T_FKD>();
                    DbQuery<T_FKD> dbq = db.Include("T_SHFW");
                    totalRecord = db.Where(where.Compile()).Count();
                    if (totalRecord > 0)
                    {
                        pageCount = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(totalRecord / pageSize))) + 1;
                        if (desc)
                        {
                            list = dbq.Where(where.Compile()).OrderByDescending<T_FKD, S>(order).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
                        }
                        else
                        {
                            list = dbq.Where(where.Compile()).OrderBy<T_FKD, S>(order).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
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
