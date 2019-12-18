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
    public class T_SHFWDomain : BaseDomain<T_SHFW>
    {
        static T_SHFWDomain _instance;
        private T_SHFWDomain()
        {
        }
        //单例模式
        static public T_SHFWDomain GetInstance()
        {
            if (_instance == null)
            {
                _instance = new T_SHFWDomain();
            }
            return _instance;
        }

        public List<T_SHFW> PageT_SHFW(T_SHFW info, DateTime? startTime, DateTime? endTime, int pageIndex, int pageSize, out int pageCount, out int totalRecord)
        {
            Expression<Func<T_SHFW, bool>> where = PredicateBuilder.True<T_SHFW>();
            if(!String.IsNullOrEmpty(info.FWMC))
            {
                where = where.And(p => p.FWMC.Contains(info.FWMC));
            }

            if(startTime != null && endTime != null)
            {
                where = where.And(p => p.FWRQ >= startTime.Value);
                where = where.And(p => p.FWRQ <= endTime.Value);
            }
            Func<T_SHFW, System.Int32> order = p => p.SHID;
            return GetPageInfo<System.Int32>(where, order, true, pageIndex, pageSize, out pageCount, out totalRecord);
        }

        public List<T_SHFW> GetAllT_SHFW(T_SHFW info)
        {
            Expression<Func<T_SHFW, bool>> where = PredicateBuilder.True<T_SHFW>();
            return base.GetAllModels<System.Int32>(where);
        }

        public List<T_SHFW> GetPageInfo<S>(Expression<Func<T_SHFW, bool>> where, Func<T_SHFW, S> order, bool desc, int pageIndex, int pageSize, out int pageCount, out int totalRecord)
        {
            totalRecord = 0;
            pageCount = 0;
            List<T_SHFW> list = new List<T_SHFW>();
            using (MedicalApparatusManageEntities hContext1 = new MedicalApparatusManageEntities())
            {
                try
                {
                    DbSet<T_SHFW> db = hContext1.Set<T_SHFW>();
                    DbQuery<T_SHFW> dbq = db.Include("T_CusQY");
                    totalRecord = db.Where(where.Compile()).Count();
                    if (totalRecord > 0)
                    {
                        pageCount = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(totalRecord / pageSize))) + 1;
                        if (desc)
                        {
                            list = dbq.Where(where.Compile()).OrderByDescending<T_SHFW, S>(order).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
                        }
                        else
                        {
                            list = dbq.Where(where.Compile()).OrderBy<T_SHFW, S>(order).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
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
