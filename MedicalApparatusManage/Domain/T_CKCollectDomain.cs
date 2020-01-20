using MedicalApparatusManage.Common;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;
using System.Web;

namespace MedicalApparatusManage.Domain
{
    public class T_CKCollectDomain : BaseDomain<T_CKCollect>
    {
        static T_CKCollectDomain _instance;
        private T_CKCollectDomain()
        {
        }
        //单例模式
        static public T_CKCollectDomain GetInstance()
        {
            if (_instance == null)
            {
                _instance = new T_CKCollectDomain();
            }
            return _instance;
        }
        public List<T_CKCollect> PageT_CKCollect(T_CKCollect info, DateTime? startTime, DateTime? endTime, int pageIndex, int pageSize, out int pageCount, out int totalRecord)
        {
            Expression<Func<T_CKCollect, bool>> where = PredicateBuilder.True<T_CKCollect>();
            if (info.CKID != 0)
            {
                where = where.And(p => p.CKID == info.CKID);
            }
            if (info.CollectPerson != null)
            {
                where = where.And(p => p.CollectPerson == info.CollectPerson);
            }
            if (startTime != null)
            {
                where = where.And(p => p.CollectDate >= startTime.Value);
            }
            if (endTime != null)
            {
                where = where.And(p => p.CollectDate < endTime.Value.AddDays(1));
            }
            Func<T_CKCollect, System.Int32> order = p => p.CKID;
            return GetPageInfo<System.Int32>(where, order, true, pageIndex, pageSize, out pageCount, out totalRecord);
        }
        public List<T_CKCollect> GetPageInfo<S>(Expression<Func<T_CKCollect, bool>> where, Func<T_CKCollect, S> order, bool desc, int pageIndex, int pageSize, out int pageCount, out int totalRecord)
        {
            totalRecord = 0;
            pageCount = 0;
            List<T_CKCollect> list = new List<T_CKCollect>();
            using (MedicalApparatusManageEntities hContext1 = new MedicalApparatusManageEntities())
            {
                try
                {
                    DbSet<T_CKCollect> db = hContext1.Set<T_CKCollect>();
                    DbQuery<T_CKCollect> dbq = db.Include("T_CK");
                    totalRecord = db.Where(where.Compile()).Count();
                    if (totalRecord > 0)
                    {
                        pageCount = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(totalRecord / pageSize))) + 1;
                        if (desc)
                        {
                            list = dbq.Where(where.Compile()).OrderByDescending<T_CKCollect, S>(order).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
                        }
                        else
                        {
                            list = dbq.Where(where.Compile()).OrderBy<T_CKCollect, S>(order).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
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