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
    public class T_SJZDFLDomain : BaseDomain<T_SJZDFL>
    {
        static T_SJZDFLDomain _instance;
        private T_SJZDFLDomain()
        {
        }
        //单例模式
        static public T_SJZDFLDomain GetInstance()
        {
            if (_instance == null)
            {
                _instance = new T_SJZDFLDomain();
            }
            return _instance;
        }

        public List<T_SJZDFL> PageT_SJZDFL(T_SJZDFL info, DateTime? startTime, DateTime? endTime, int pageIndex, int pageSize, out int pageCount, out int totalRecord)
        {
            Expression<Func<T_SJZDFL, bool>> where = PredicateBuilder.True<T_SJZDFL>();
            Func<T_SJZDFL, System.Int32> order = p => p.FLID;
            return base.GetPageInfo<System.Int32>(where, order, true, pageIndex, pageSize, out pageCount, out totalRecord);
        }

        public List<T_SJZDFL> GetAllT_SJZDFL(T_SJZDFL info)
        {
            Expression<Func<T_SJZDFL, bool>> where = PredicateBuilder.True<T_SJZDFL>();
            return base.GetAllModels<System.Int32>(where);
        }

        public T_SJZDFL GetModeByName(string name)
        {
            using (MedicalApparatusManageEntities hContext1 = new MedicalApparatusManageEntities())
            {
                try
                {
                    List<T_SJZDFL> list = null;
                    Expression<Func<T_SJZDFL, bool>> where = PredicateBuilder.True<T_SJZDFL>();
                    if (!string.IsNullOrEmpty(name))
                    {
                        where.And(p =>p.FLMC == name);
                    }
                    DbSet<T_SJZDFL> db = hContext1.Set<T_SJZDFL>();

                    list = db.Where(where.Compile()).ToList();
                    if (list.Count > 0)
                    {
                        return list[0];
                    }
                }
                catch (Exception ex)
                { 
                }
            }
            return null;
        }

        public List<T_SJZDFL> GetPageInfo<S>(Expression<Func<T_SJZDFL, bool>> where, Func<T_SJZDFL, S> order, bool desc, int pageIndex, int pageSize, out int pageCount, out int totalRecord)
        {
            totalRecord = 0;
            pageCount = 0;
            List<T_SJZDFL> list = new List<T_SJZDFL>();
            using (MedicalApparatusManageEntities hContext1 = new MedicalApparatusManageEntities())
            {
                try
                {
                    DbSet<T_SJZDFL> db = hContext1.Set<T_SJZDFL>();
                    DbQuery<T_SJZDFL> dbq = db.Include("");
                    totalRecord = db.Where(where.Compile()).Count();
                    if (totalRecord > 0)
                    {
                        pageCount = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(totalRecord / pageSize))) + 1;
                        if (desc)
                        {
                            list = dbq.Where(where.Compile()).OrderByDescending<T_SJZDFL, S>(order).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
                        }
                        else
                        {
                            list = dbq.Where(where.Compile()).OrderBy<T_SJZDFL, S>(order).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
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
