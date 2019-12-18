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
    public class T_JYFWDomain : BaseDomain<T_JYFW>
    {
        static T_JYFWDomain _instance;
        private T_JYFWDomain()
        {
        }
        //单例模式
        static public T_JYFWDomain GetInstance()
        {
            if (_instance == null)
            {
                _instance = new T_JYFWDomain();
            }
            return _instance;
        }

        public List<T_JYFW> PageT_JYFW(T_JYFW info, DateTime? startTime, DateTime? endTime, int pageIndex, int pageSize, out int pageCount, out int totalRecord)
        {
            Expression<Func<T_JYFW, bool>> where = PredicateBuilder.True<T_JYFW>();
            Func<T_JYFW, System.Int32> order = p => p.FWID;
            return base.GetPageInfo<System.Int32>(where, order, true, pageIndex, pageSize, out pageCount, out totalRecord);
        }

        public List<T_JYFW> GetAllT_JYFW(T_JYFW info)
        {
            Expression<Func<T_JYFW, bool>> where = PredicateBuilder.True<T_JYFW>();
            return base.GetAllModels<System.Int32>(where);
        }

        public List<T_JYFW> GetPageInfo<S>(Expression<Func<T_JYFW, bool>> where, Func<T_JYFW, S> order, bool desc, int pageIndex, int pageSize, out int pageCount, out int totalRecord)
        {
            totalRecord = 0;
            pageCount = 0;
            List<T_JYFW> list = new List<T_JYFW>();
            using (MedicalApparatusManageEntities hContext1 = new MedicalApparatusManageEntities())
            {
                try
                {
                    DbSet<T_JYFW> db = hContext1.Set<T_JYFW>();
                    DbQuery<T_JYFW> dbq = db.Include("T_WhsQY");
                    totalRecord = db.Where(where.Compile()).Count();
                    if (totalRecord > 0)
                    {
                        pageCount = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(totalRecord / pageSize))) + 1;
                        if (desc)
                        {
                            list = dbq.Where(where.Compile()).OrderByDescending<T_JYFW, S>(order).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
                        }
                        else
                        {
                            list = dbq.Where(where.Compile()).OrderBy<T_JYFW, S>(order).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
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
