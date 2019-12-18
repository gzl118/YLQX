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
    public class T_CKMXDomain : BaseDomain<T_CKMX>
    {
        static T_CKMXDomain _instance;
        private T_CKMXDomain()
        {
        }
        //单例模式
        static public T_CKMXDomain GetInstance()
        {
            if (_instance == null)
            {
                _instance = new T_CKMXDomain();
            }
            return _instance;
        }

        public List<T_CKMX> PageT_CKMX(T_CKMX info, DateTime? startTime, DateTime? endTime, int pageIndex, int pageSize, out int pageCount, out int totalRecord)
        {
            Expression<Func<T_CKMX, bool>> where = PredicateBuilder.True<T_CKMX>();
            Func<T_CKMX, System.Int32> order = p => p.CPID;
            return GetPageInfo<System.Int32>(where, order, true, pageIndex, pageSize, out pageCount, out totalRecord);
        }

        public List<T_CKMX> GetAllT_CKMX(T_CKMX info)
        {
            Expression<Func<T_CKMX, bool>> where = PredicateBuilder.True<T_CKMX>();
            return base.GetAllModels<System.Int32>(where);
        }

        public List<T_CKMX> GetPageInfo<S>(Expression<Func<T_CKMX, bool>> where, Func<T_CKMX, S> order, bool desc, int pageIndex, int pageSize, out int pageCount, out int totalRecord)
        {
            totalRecord = 0;
            pageCount = 0;
            List<T_CKMX> list = new List<T_CKMX>();
            using (MedicalApparatusManageEntities hContext1 = new MedicalApparatusManageEntities())
            {
                try
                {
                    DbSet<T_CKMX> db = hContext1.Set<T_CKMX>();
                    DbQuery<T_CKMX> dbq = db.Include("T_YLCP").Include("T_CK").Include("T_CKD");
                    totalRecord = db.Where(where.Compile()).Count();
                    if (totalRecord > 0)
                    {
                        pageCount = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(totalRecord / pageSize))) + 1;
                        if (desc)
                        {
                            list = dbq.Where(where.Compile()).OrderByDescending<T_CKMX, S>(order).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
                        }
                        else
                        {
                            list = dbq.Where(where.Compile()).OrderBy<T_CKMX, S>(order).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
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

        public List<T_CKMX> GetListModelById(Int32 id)
        {
            using (MedicalApparatusManageEntities hContext1 = new MedicalApparatusManageEntities())
            {
                try
                {
                    DbSet<T_CKMX> db = hContext1.Set<T_CKMX>();
                    DbQuery<T_CKMX> dbq = db.Include("T_YLCP").Include("T_YLCP.T_SupQY1");
                    return dbq.Where(p => p.CKDID == id).ToList();
                }
                catch (Exception)
                {
                    return new List<T_CKMX>();
                }
            }
        }

        public int AddModelByCkdh(T_CKMX model, string CKDH)
        {
            using (MedicalApparatusManageEntities hContext1 = new MedicalApparatusManageEntities())
            {
                try
                {
                    int ckid = hContext1.Set<T_CKD>().Where(p => p.CKDH == CKDH).FirstOrDefault().CKID;
                    model.CKDID = ckid;

                    //对新库存数据进行修改
                    DbSet<T_KC> kcdb = hContext1.Set<T_KC>();
                    var kc = kcdb.Where(p => p.CPID == model.CPID && p.CPPH == model.CPPH && p.CKID == model.CKID).FirstOrDefault();
                    if (kc == null)
                    {
                        return 0;
                    }
                    kc.CPNUM = kc.CPNUM - Convert.ToInt32(model.CPNUM);
                    hContext1.Set<T_CKMX>().Add(model);
                    return hContext1.SaveChanges();
                }
                catch (Exception ex)
                {
                    return 0;
                }

            }
        }

        public List<T_CKMX> GetT_CKMXByCkid(int ckid)
        {
            List<T_CKMX> list = new List<T_CKMX>();
            using (MedicalApparatusManageEntities hContext1 = new MedicalApparatusManageEntities())
            {
                try
                {
                    DbSet<T_CKMX> db = hContext1.Set<T_CKMX>();
                    DbQuery<T_CKMX> dbq = db.Include("T_YLCP").Include("T_CK").Include("T_YLCP.T_SupQY1");
                    list = dbq.Where(p => p.CKDID == ckid).ToList(); ;
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
                    DbSet<T_CKMX> db = hContext1.Set<T_CKMX>();
                    T_CKMX model = db.Where(p => p.GUID == guid).FirstOrDefault();
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

        public List<T_CKMX> GetT_CKMXByCkdh(string ckdh)
        {
            List<T_CKMX> list = new List<T_CKMX>();
            using (MedicalApparatusManageEntities hContext1 = new MedicalApparatusManageEntities())
            {
                try
                {
                    var ckd = hContext1.Set<T_CKD>().Where(p => p.CKDH == ckdh).FirstOrDefault();
                    if (ckd != null)
                    {
                        DbSet<T_CKMX> db = hContext1.Set<T_CKMX>();
                        DbQuery<T_CKMX> dbq = db.Include("T_YLCP").Include("T_CK").Include("T_YLCP.T_SupQY1");
                        list = dbq.Where(p => p.CKDID == ckd.CKID).ToList();
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
