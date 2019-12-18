﻿using MedicalApparatusManage.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;


namespace MedicalApparatusManage.Domain
{
    public class T_THDDomain : BaseDomain<T_THD>
    {
        static T_THDDomain _instance;
        private T_THDDomain()
        {
        }
        //单例模式
        static public T_THDDomain GetInstance()
        {
            if (_instance == null)
            {
                _instance = new T_THDDomain();
            }
            return _instance;
        }

        public List<T_THD> PageT_THD(T_THD info, DateTime? startTime, DateTime? endTime, int pageIndex, int pageSize, out int pageCount, out int totalRecord)
        {
            Expression<Func<T_THD, bool>> where = PredicateBuilder.True<T_THD>();

            if(!String.IsNullOrEmpty(info.THMC))
            {
                where = where.And(p => p.THMC.Contains(info.THMC));
            }
            if(!String.IsNullOrEmpty(info.THKHMC))
            {
                where = where.And(p => p.THKHMC.Contains(info.THKHMC));
            }
            Func<T_THD, System.Int32> order = p => p.THID;
            return GetPageInfo<System.Int32>(where, order, true, pageIndex, pageSize, out pageCount, out totalRecord);
        }

        public List<T_THD> GetAllT_THD(T_THD info)
        {
            Expression<Func<T_THD, bool>> where = PredicateBuilder.True<T_THD>();
            return base.GetAllModels<System.Int32>(where);
        }

        public List<T_THD> GetPageInfo<S>(Expression<Func<T_THD, bool>> where, Func<T_THD, S> order, bool desc, int pageIndex, int pageSize, out int pageCount, out int totalRecord)
        {
            totalRecord = 0;
            pageCount = 0;
            List<T_THD> list = new List<T_THD>();
            using (MedicalApparatusManageEntities hContext1 = new MedicalApparatusManageEntities())
            {
                try
                {
                    DbSet<T_THD> db = hContext1.Set<T_THD>();
                    DbQuery<T_THD> dbq = db.Include("T_XSD").Include("T_CusQY");
                    totalRecord = db.Where(where.Compile()).Count();
                    if (totalRecord > 0)
                    {
                        pageCount = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(totalRecord / pageSize))) + 1;
                        if (desc)
                        {
                            list = dbq.Where(where.Compile()).OrderByDescending<T_THD, S>(order).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
                        }
                        else
                        {
                            list = dbq.Where(where.Compile()).OrderBy<T_THD, S>(order).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
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
