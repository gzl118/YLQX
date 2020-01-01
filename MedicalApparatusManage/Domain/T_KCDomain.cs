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
    public class T_KCDomain : BaseDomain<T_KC>
    {
        static T_KCDomain _instance;
        private T_KCDomain()
        {
        }
        //单例模式
        static public T_KCDomain GetInstance()
        {
            if (_instance == null)
            {
                _instance = new T_KCDomain();
            }
            return _instance;
        }

        public List<T_KC> PageT_KC(T_KC info, DateTime? startTime, DateTime? endTime, int pageIndex, int pageSize, out int pageCount, out int totalRecord)
        {
            Expression<Func<T_KC, bool>> where = PredicateBuilder.True<T_KC>();
            if (info.CKID != 0)
            {
                where = where.And(p => p.CKID == info.CKID);
            }
            if (info.CPID != 0)
            {
                where = where.And(p => p.CPID == info.CPID);
            }
            //if(info.T_YLCP != null && info.T_YLCP.CPSCQY != "")
            //{
            //    where = where.And(p => p.T_YLCP.CPSCQY == info.T_YLCP.CPSCQY);
            //}
            if (info.ScqyID != null && info.ScqyID != 0)
            {
                where = where.And(p => p.ScqyID == info.ScqyID);
            }
            Func<T_KC, System.Int32> order = p => p.CPID;
            return GetPageInfo<System.Int32>(where, order, true, pageIndex, pageSize, out pageCount, out totalRecord);
        }

        public List<T_KC> GetAllT_KC(T_KC info)
        {
            Expression<Func<T_KC, bool>> where = PredicateBuilder.True<T_KC>();
            if (info.CPID > 0)
            {
                where = where.And(p => p.CPID == info.CPID);
            }
            if (info.CKID > 0)
            {
                where = where.And(p => p.CKID == info.CKID);
            }
            if (info.CPPH != null && info.CPPH != "")
            {
                where = where.And(p => p.CPPH == info.CPPH);
            }
            return GetAllModels<System.Int32>(where);
        }

        public List<T_KC> GetAllModels<S>(Expression<Func<T_KC, bool>> where)
        {
            List<T_KC> list = new List<T_KC>();
            using (MedicalApparatusManageEntities hContext1 = new MedicalApparatusManageEntities())
            {
                try
                {
                    DbSet<T_KC> db = hContext1.Set<T_KC>();
                    DbQuery<T_KC> dbq = db.Include("T_CK").Include("T_YLCP").Include("T_SupQY1").Include("T_SupQY");
                    list = dbq.Where(where.Compile()).ToList();
                }
                catch (Exception)
                {

                }
                return list;
            }
        }

        public List<T_KC> GetPageInfo<S>(Expression<Func<T_KC, bool>> where, Func<T_KC, S> order, bool desc, int pageIndex, int pageSize, out int pageCount, out int totalRecord)
        {
            totalRecord = 0;
            pageCount = 0;
            List<T_KC> list = new List<T_KC>();
            using (MedicalApparatusManageEntities hContext1 = new MedicalApparatusManageEntities())
            {
                try
                {
                    DbSet<T_KC> db = hContext1.Set<T_KC>();
                    DbQuery<T_KC> dbq = db.Include("T_CK").Include("T_YLCP").Include("T_SupQY").Include("T_SupQY1");
                    totalRecord = db.Where(where.Compile()).Count();
                    if (totalRecord > 0)
                    {
                        pageCount = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(totalRecord / pageSize))) + 1;
                        if (desc)
                        {
                            list = dbq.Where(where.Compile()).OrderByDescending<T_KC, S>(order).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
                        }
                        else
                        {
                            list = dbq.Where(where.Compile()).OrderBy<T_KC, S>(order).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
                        }
                    }
                }
                catch (Exception ex)
                {
                    return null;
                }
                return list;
            }
        }

        public T_KC GetKCid(int CKID)
        {
            T_KC result = null;
            using (MedicalApparatusManageEntities gContext = new MedicalApparatusManageEntities())
            {
                try
                {
                    var list = gContext.T_KC.Where(p => p.CKID == CKID);
                    if (list.Count() > 0)
                    {
                        T_KC T_KCNew = list.FirstOrDefault();
                        T_KCNew.CKID = CKID;
                        gContext.SaveChanges();
                        result = T_KCNew;
                    }
                }
                catch (Exception)
                {

                }
            }
            return result;
        }

        public T_KC GetKCByPara(int CKID, int CPID, string CPPH)
        {
            T_KC result = null;


            using (MedicalApparatusManageEntities gContext = new MedicalApparatusManageEntities())
            {
                try
                {
                    Expression<Func<T_KC, bool>> where = PredicateBuilder.True<T_KC>();
                    if (CPID > 0)
                    {
                        where = where.And(p => p.CPID == CPID);
                    }
                    if (CKID > 0)
                    {
                        where = where.And(p => p.CKID == CKID);
                    }
                    if (CPPH != null && CPPH != "")
                    {
                        where = where.And(p => p.CPPH == CPPH);
                    }
                    var list = gContext.T_KC.Where(where.Compile());
                    if (list.Count() > 0)
                    {
                        T_KC T_KCNew = list.FirstOrDefault();
                        T_KCNew.CKID = CKID;
                        gContext.SaveChanges();
                        result = T_KCNew;
                    }
                }
                catch (Exception)
                {

                }
            }
            return result;
        }
    }
}
