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
    public class T_XSHTDomain : BaseDomain<T_XSHT>
    {
        static T_XSHTDomain _instance;
        private T_XSHTDomain()
        {
        }
        //单例模式
        static public T_XSHTDomain GetInstance()
        {
            if (_instance == null)
            {
                _instance = new T_XSHTDomain();
            }
            return _instance;
        }

        public List<T_XSHT> PageT_XSHT(T_XSHT info, DateTime? startTime, DateTime? endTime, int pageIndex, int pageSize, out int pageCount, out int totalRecord)
        {
            Expression<Func<T_XSHT, bool>> where = PredicateBuilder.True<T_XSHT>();
            if (!String.IsNullOrEmpty(info.HTBH))
            {
                where = where.And(p => p.HTBH.Contains(info.HTBH));
            }

            if (!String.IsNullOrEmpty(info.HTMC))
            {
                where = where.And(p => p.HTMC.Contains(info.HTMC));
            }
            Func<T_XSHT, System.Int32> order = p => p.HTID;
            return GetPageInfo<System.Int32>(where, order, true, pageIndex, pageSize, out pageCount, out totalRecord);
        }


        public List<T_XSHT> GetAllT_XSHT(T_XSHT info)
        {
            Expression<Func<T_XSHT, bool>> where = PredicateBuilder.True<T_XSHT>();
            return base.GetAllModels<System.Int32>(where);
        }

        public List<T_XSHT> GetPageInfo<S>(Expression<Func<T_XSHT, bool>> where, Func<T_XSHT, S> order, bool desc, int pageIndex, int pageSize, out int pageCount, out int totalRecord)
        {
            totalRecord = 0;
            pageCount = 0;
            List<T_XSHT> list = new List<T_XSHT>();
            using (MedicalApparatusManageEntities hContext1 = new MedicalApparatusManageEntities())
            {
                try
                {
                    DbSet<T_XSHT> db = hContext1.Set<T_XSHT>();
                    DbQuery<T_XSHT> dbq = db.Include("T_CusQY");
                    totalRecord = db.Where(where.Compile()).Count();
                    if (totalRecord > 0)
                    {
                        pageCount = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(totalRecord / pageSize))) + 1;
                        if (desc)
                        {
                            list = dbq.Where(where.Compile()).OrderByDescending<T_XSHT, S>(order).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
                        }
                        else
                        {
                            list = dbq.Where(where.Compile()).OrderBy<T_XSHT, S>(order).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
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
