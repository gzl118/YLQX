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
    public class T_CGMXDomain : BaseDomain<T_CGMX>
    {
        static T_CGMXDomain _instance;
        private T_CGMXDomain()
        {
        }
        //单例模式
        static public T_CGMXDomain GetInstance()
        {
            if (_instance == null)
            {
                _instance = new T_CGMXDomain();
            }
            return _instance;
        }

        public List<T_CGMX> PageT_CGMX(T_CGMX info, DateTime? startTime, DateTime? endTime, int pageIndex, int pageSize, out int pageCount, out int totalRecord)
        {
            Expression<Func<T_CGMX, bool>> where = PredicateBuilder.True<T_CGMX>();
            if (info.T_SupQY1 != null && info.T_SupQY1.SupID != 0)
            {
                where = where.And(p => p.T_SupQY1 != null && p.T_SupQY1.SupID == info.T_SupQY1.SupID);
            }

            if (startTime != null)
            {
                where = where.And(p => p.T_CGD.CGRQ >= startTime.Value);
            }
            if (endTime != null)
            {
                where = where.And(p => p.T_CGD.CGRQ <= endTime.Value);
            }
            if (info.CPID != 0)
            {
                where = where.And(p => p.CPID == info.CPID);
            }
            Func<T_CGMX, System.Int32> order = p => p.CPID;
            return GetPageInfo<System.Int32>(where, order, true, pageIndex, pageSize, out pageCount, out totalRecord);
        }

        public List<T_CGMX> GetAllT_CGMX(T_CGMX info)
        {
            Expression<Func<T_CGMX, bool>> where = PredicateBuilder.True<T_CGMX>();
            return GetAllModels<System.Int32>(where);
        }

        public override List<T_CGMX> GetPageInfo<S>(Expression<Func<T_CGMX, bool>> where, Func<T_CGMX, S> order, bool desc, int pageIndex, int pageSize, out int pageCount, out int totalRecord)
        {
            totalRecord = 0;
            pageCount = 0;
            List<T_CGMX> list = new List<T_CGMX>();
            using (MedicalApparatusManageEntities hContext1 = new MedicalApparatusManageEntities())
            {
                try
                {
                    DbSet<T_CGMX> db = hContext1.Set<T_CGMX>();
                    DbQuery<T_CGMX> dbq = db.Include("T_YLCP").Include("T_CGD").Include("T_SupQY").Include("T_SupQY1");
                    totalRecord = db.Where(where.Compile()).Count();
                    if (totalRecord > 0)
                    {
                        pageCount = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(totalRecord / pageSize))) + 1;
                        if (desc)
                        {
                            list = dbq.Where(where.Compile()).OrderByDescending<T_CGMX, string>(p => p.T_CGD.CGDH).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
                        }
                        else
                        {
                            list = dbq.Where(where.Compile()).OrderBy<T_CGMX, string>(p => p.T_CGD.CGDH).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
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

        public List<T_CGMX> GetAllModels<S>(Expression<Func<T_CGMX, bool>> where)
        {
            List<T_CGMX> list = new List<T_CGMX>();
            using (MedicalApparatusManageEntities hContext1 = new MedicalApparatusManageEntities())
            {
                try
                {
                    DbSet<T_CGMX> db = hContext1.Set<T_CGMX>();
                    DbQuery<T_CGMX> dbq = db.Include("T_YLCP").Include("T_CGD").Include("T_SupQY").Include("T_SupQY1");
                    list = dbq.Where(where.Compile()).ToList();
                }
                catch (Exception)
                {

                }
                return list;
            }
        }

        public int AddModelByCgdh(T_CGMX model, string CGDH)
        {
            using (MedicalApparatusManageEntities hContext1 = new MedicalApparatusManageEntities())
            {
                try
                {
                    int cgid = hContext1.Set<T_CGD>().Where(p => p.CGDH == CGDH).FirstOrDefault().CGID;
                    model.CGID = cgid;
                    hContext1.Set<T_CGMX>().Add(model);
                    return hContext1.SaveChanges();
                }
                catch (Exception ex)
                {
                    return 0;
                }

            }
        }

        public List<T_CGMX> GetT_CGMXByCgid(int cgid)
        {
            List<T_CGMX> list = new List<T_CGMX>();
            using (MedicalApparatusManageEntities hContext1 = new MedicalApparatusManageEntities())
            {
                try
                {
                    DbSet<T_CGMX> db = hContext1.Set<T_CGMX>();
                    DbQuery<T_CGMX> dbq = db.Include("T_YLCP").Include("T_CGD").Include("T_SupQY").Include("T_SupQY1");
                    list = dbq.Where(p => p.CGID == cgid).ToList(); ;
                }
                catch (Exception ex)
                {
                }
            }
            return list;
        }

        public List<T_CGMX> GetT_YSMXByCgdh(string cgdh)
        {
            List<T_CGMX> list = new List<T_CGMX>();
            using (MedicalApparatusManageEntities hContext1 = new MedicalApparatusManageEntities())
            {
                try
                {
                    var ysd = hContext1.Set<T_CGD>().Where(p => p.CGDH == cgdh).FirstOrDefault();
                    if (ysd != null)
                    {
                        DbSet<T_CGMX> db = hContext1.Set<T_CGMX>();
                        DbQuery<T_CGMX> dbq = db.Include("T_YLCP").Include("T_CGD").Include("T_SupQY").Include("T_SupQY1");
                        list = dbq.Where(p => p.CGID == ysd.CGID).ToList();
                    }
                }
                catch (Exception ex)
                {
                }
            }
            return list;
        }

        public int DeleteModelByGuid(string guid)
        {
            using (MedicalApparatusManageEntities hContext1 = new MedicalApparatusManageEntities())
            {
                try
                {
                    DbSet<T_CGMX> db = hContext1.Set<T_CGMX>();
                    T_CGMX model = db.Where(p => p.GUID == guid).FirstOrDefault();
                    db.Remove(model);
                    return hContext1.SaveChanges();
                }
                catch (Exception)
                {
                    return 0;
                }
            }
        }
    }
}
