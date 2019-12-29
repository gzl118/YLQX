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
    public class T_SHMXDomain : BaseDomain<T_SHMX>
    {
        static T_SHMXDomain _instance;
        private T_SHMXDomain()
        {
        }
        //单例模式
        static public T_SHMXDomain GetInstance()
        {
            if (_instance == null)
            {
                _instance = new T_SHMXDomain();
            }
            return _instance;
        }

        public List<T_SHMX> PageT_CKMX(T_SHMX info, DateTime? startTime, DateTime? endTime, int pageIndex, int pageSize, out int pageCount, out int totalRecord)
        {
            Expression<Func<T_SHMX, bool>> where = PredicateBuilder.True<T_SHMX>();
            Func<T_SHMX, System.Int32> order = p => p.SHMXID;
            return GetPageInfo<System.Int32>(where, order, true, pageIndex, pageSize, out pageCount, out totalRecord);
        }

        public List<T_SHMX> GetAllT_CKMX(T_SHMX info)
        {
            Expression<Func<T_SHMX, bool>> where = PredicateBuilder.True<T_SHMX>();
            return base.GetAllModels<System.Int32>(where);
        }

        public List<T_SHMX> GetPageInfo<S>(Expression<Func<T_SHMX, bool>> where, Func<T_SHMX, S> order, bool desc, int pageIndex, int pageSize, out int pageCount, out int totalRecord)
        {
            totalRecord = 0;
            pageCount = 0;
            List<T_SHMX> list = new List<T_SHMX>();
            using (MedicalApparatusManageEntities hContext1 = new MedicalApparatusManageEntities())
            {
                try
                {
                    DbSet<T_SHMX> db = hContext1.Set<T_SHMX>();
                    DbQuery<T_SHMX> dbq = db.Include("T_YLCP").Include("T_CK").Include("T_SHD");
                    totalRecord = db.Where(where.Compile()).Count();
                    if (totalRecord > 0)
                    {
                        pageCount = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(totalRecord / pageSize))) + 1;
                        if (desc)
                        {
                            list = dbq.Where(where.Compile()).OrderByDescending<T_SHMX, S>(order).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
                        }
                        else
                        {
                            list = dbq.Where(where.Compile()).OrderBy<T_SHMX, S>(order).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
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

        public List<T_SHMX> GetListModelById(Int32 id)
        {
            using (MedicalApparatusManageEntities hContext1 = new MedicalApparatusManageEntities())
            {
                try
                {
                    DbSet<T_SHMX> db = hContext1.Set<T_SHMX>();
                    DbQuery<T_SHMX> dbq = db.Include("T_YLCP").Include("T_YLCP.T_SupQY1");
                    return dbq.Where(p => p.SHID == id).ToList();
                }
                catch (Exception)
                {
                    return new List<T_SHMX>();
                }
            }
        }

        public int AddModelByCkdh(T_SHMX model, string SHDH)
        {
            using (MedicalApparatusManageEntities hContext1 = new MedicalApparatusManageEntities())
            {
                try
                {
                    int ckid = hContext1.Set<T_SHD>().Where(p => p.SHDH == SHDH).FirstOrDefault().SHID;
                    model.SHID = ckid;

                    //对新库存数据进行修改
                    //DbSet<T_KC> kcdb = hContext1.Set<T_KC>();
                    //var kc = kcdb.Where(p => p.CPID == model.CPID && p.CPPH == model.CPPH && p.CKID == model.CKID).FirstOrDefault();
                    //if (kc == null)
                    //{
                    //    return 0;
                    //}
                    //kc.CPNUM = kc.CPNUM - Convert.ToInt32(model.CPNUM);
                    model.CPTPRICE = (model.CPPRICE ?? 0) * (model.CPNUM ?? 0);
                    hContext1.Set<T_SHMX>().Add(model);
                    return hContext1.SaveChanges();
                }
                catch (Exception ex)
                {
                    return 0;
                }

            }
        }

        public List<T_SHMX> GetT_SHKMXByCkid(int ckid)
        {
            List<T_SHMX> list = new List<T_SHMX>();
            using (MedicalApparatusManageEntities hContext1 = new MedicalApparatusManageEntities())
            {
                try
                {
                    DbSet<T_SHMX> db = hContext1.Set<T_SHMX>();
                    DbQuery<T_SHMX> dbq = db.Include("T_YLCP").Include("T_CK").Include("T_YLCP.T_SupQY1");
                    list = dbq.Where(p => p.SHID == ckid).ToList(); ;
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
                    DbSet<T_SHMX> db = hContext1.Set<T_SHMX>();
                    T_SHMX model = db.Where(p => p.GUID == guid).FirstOrDefault();
                    //对原库存数据进行修改
                    DbSet<T_KC> kcdb = hContext1.Set<T_KC>();
                    var oldkc = kcdb.Where(p => p.CPID == model.CPID && p.CPPH == model.CPPH && p.CKID == model.CKID).FirstOrDefault();
                    if (oldkc != null)
                    {
                        oldkc.CPNUM = oldkc.CPNUM + Convert.ToInt32(model.CPNUM);
                    }
                    db.Remove(model);
                    return hContext1.SaveChanges();
                }
                catch (Exception)
                {
                    return 0;
                }
            }
        }

        public List<T_SHMX> GetT_SHMXByCkdh(string ckdh)
        {
            List<T_SHMX> list = new List<T_SHMX>();
            using (MedicalApparatusManageEntities hContext1 = new MedicalApparatusManageEntities())
            {
                try
                {
                    var ckd = hContext1.Set<T_SHD>().Where(p => p.SHDH == ckdh).FirstOrDefault();
                    if (ckd != null)
                    {
                        DbSet<T_SHMX> db = hContext1.Set<T_SHMX>();
                        DbQuery<T_SHMX> dbq = db.Include("T_YLCP").Include("T_CK").Include("T_YLCP.T_SupQY1");
                        list = dbq.Where(p => p.SHID == ckd.SHID).ToList();
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