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
    public class T_FPDomain : BaseDomain<T_FP>
    {
        static T_FPDomain _instance;
        private T_FPDomain()
        {
        }
        //单例模式
        static public T_FPDomain GetInstance()
        {
            if (_instance == null)
            {
                _instance = new T_FPDomain();
            }
            return _instance;
        }

        public List<T_FP> PageT_FP(T_FP info, DateTime? startTime, DateTime? endTime, int pageIndex, int pageSize, out int pageCount, out int totalRecord)
        {
            Expression<Func<T_FP, bool>> where = PredicateBuilder.True<T_FP>();
            if(!String.IsNullOrEmpty(info.KPQY))
            {
                where = where.And(p => p.KPQY.Contains(info.KPQY));
            }
            if(startTime!= null && endTime != null)
            {
                where = where.And(p => p.KPRQ >= startTime.Value);
                where = where.And(p => p.KPRQ <= endTime.Value);
            }
            Func<T_FP, System.Int32> order = p => p.FPID;
            return base.GetPageInfo<System.Int32>(where, order, true, pageIndex, pageSize, out pageCount, out totalRecord);
        }

        public List<T_FP> GetAllT_FP(T_FP info)
        {
            Expression<Func<T_FP, bool>> where = PredicateBuilder.True<T_FP>();
            return base.GetAllModels<System.Int32>(where);
        }

        public List<T_FP> GetPageInfo<S>(Expression<Func<T_FP, bool>> where, Func<T_FP, S> order, bool desc, int pageIndex, int pageSize, out int pageCount, out int totalRecord)
        {
            totalRecord = 0;
            pageCount = 0;
            List<T_FP> list = new List<T_FP>();
            using (MedicalApparatusManageEntities hContext1 = new MedicalApparatusManageEntities())
            {
                try
                {
                    DbSet<T_FP> db = hContext1.Set<T_FP>();
                    DbQuery<T_FP> dbq = db.Include("T_WhsQY");
                    totalRecord = db.Where(where.Compile()).Count();
                    if (totalRecord > 0)
                    {
                        pageCount = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(totalRecord / pageSize))) + 1;
                        if (desc)
                        {
                            list = dbq.Where(where.Compile()).OrderByDescending<T_FP, S>(order).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
                        }
                        else
                        {
                            list = dbq.Where(where.Compile()).OrderBy<T_FP, S>(order).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
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
