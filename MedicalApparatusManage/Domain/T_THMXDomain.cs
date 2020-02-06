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
    public class T_THMXDomain : BaseDomain<T_THMX>
    {
        static T_THMXDomain _instance;
        private T_THMXDomain()
        {
        }
        //单例模式
        static public T_THMXDomain GetInstance()
        {
            if (_instance == null)
            {
                _instance = new T_THMXDomain();
            }
            return _instance;
        }

        public List<T_THMX> PageT_THMX(T_THMX info, DateTime? startTime, DateTime? endTime, int pageIndex, int pageSize, out int pageCount, out int totalRecord)
        {
            Expression<Func<T_THMX, bool>> where = PredicateBuilder.True<T_THMX>();
            Func<T_THMX, System.Int32> order = p => p.CPID;
            return GetPageInfo<System.Int32>(where, order, true, pageIndex, pageSize, out pageCount, out totalRecord);
        }

        public List<T_THMX> GetAllT_THMX(T_THMX info)
        {
            Expression<Func<T_THMX, bool>> where = PredicateBuilder.True<T_THMX>();
            return base.GetAllModels<System.Int32>(where);
        }

        public List<T_THMX> GetPageInfo<S>(Expression<Func<T_THMX, bool>> where, Func<T_THMX, S> order, bool desc, int pageIndex, int pageSize, out int pageCount, out int totalRecord)
        {
            totalRecord = 0;
            pageCount = 0;
            List<T_THMX> list = new List<T_THMX>();
            using (MedicalApparatusManageEntities hContext1 = new MedicalApparatusManageEntities())
            {
                try
                {
                    DbSet<T_THMX> db = hContext1.Set<T_THMX>();
                    DbQuery<T_THMX> dbq = db.Include("T_THD").Include("T_YLCP").Include("T_CK");
                    totalRecord = db.Where(where.Compile()).Count();
                    if (totalRecord > 0)
                    {
                        pageCount = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(totalRecord / pageSize))) + 1;
                        if (desc)
                        {
                            list = dbq.Where(where.Compile()).OrderByDescending<T_THMX, S>(order).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
                        }
                        else
                        {
                            list = dbq.Where(where.Compile()).OrderBy<T_THMX, S>(order).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
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
        public List<T_THMX> GetT_THMXByYsid(int thid)
        {
            List<T_THMX> list = new List<T_THMX>();
            using (MedicalApparatusManageEntities hContext1 = new MedicalApparatusManageEntities())
            {
                try
                {
                    var db = hContext1.Set<T_THMX>();
                    var dbq = db.Include("T_YLCP").Include("T_THD").Include("T_CK");
                    list = dbq.Where(p => p.THID == thid).ToList(); ;
                }
                catch (Exception ex)
                {
                }
            }
            return list;
        }
        public int AddModelByRkdh(T_THMX model, string THDH)
        {
            using (MedicalApparatusManageEntities hContext1 = new MedicalApparatusManageEntities())
            {
                try
                {
                    var parentModel = hContext1.Set<T_THD>().Where(p => p.THDH == THDH).FirstOrDefault();
                    parentModel.ISSH = 0;
                    var rkid = parentModel.THID;
                    model.THID = rkid;
                    hContext1.Set<T_THMX>().Add(model);
                    return hContext1.SaveChanges();
                }
                catch (Exception ex)
                {
                    return 0;
                }

            }
        }
        public List<T_THMX> GetT_THMXBythid(int thid)
        {
            List<T_THMX> list = new List<T_THMX>();
            using (MedicalApparatusManageEntities hContext1 = new MedicalApparatusManageEntities())
            {
                try
                {
                    DbSet<T_THMX> db = hContext1.Set<T_THMX>();
                    DbQuery<T_THMX> dbq = db.Include("T_YLCP").Include("T_THD").Include("T_YLCP.T_SupQY").Include("T_YLCP.T_SupQY1").Include("T_CK");
                    list = dbq.Where(p => p.THID == thid).ToList();
                }
                catch (Exception ex)
                {
                }
            }
            return list;
        }

        public List<T_THMX> GetT_THMXBythdh(string thdh)
        {
            List<T_THMX> list = new List<T_THMX>();
            using (MedicalApparatusManageEntities hContext1 = new MedicalApparatusManageEntities())
            {
                try
                {
                    var ysd = hContext1.Set<T_THD>().Where(p => p.THDH == thdh).FirstOrDefault();
                    if (ysd != null)
                    {
                        DbSet<T_THMX> db = hContext1.Set<T_THMX>();
                        DbQuery<T_THMX> dbq = db.Include("T_YLCP").Include("T_THD").Include("T_YLCP.T_SupQY").Include("T_YLCP.T_SupQY1").Include("T_CK");
                        list = dbq.Where(p => p.THID == ysd.THID).ToList();
                    }
                }
                catch (Exception ex)
                {
                }
            }
            return list;
        }
    }
}
