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
    public class T_RKMXDomain : BaseDomain<T_RKMX>
    {
        static T_RKMXDomain _instance;
        private T_RKMXDomain()
        {
        }
        //单例模式
        static public T_RKMXDomain GetInstance()
        {
            if (_instance == null)
            {
                _instance = new T_RKMXDomain();
            }
            return _instance;
        }

        public List<T_RKMX> PageT_RKMX(T_RKMX info, DateTime? startTime, DateTime? endTime, int pageIndex, int pageSize, out int pageCount, out int totalRecord)
        {
            Expression<Func<T_RKMX, bool>> where = PredicateBuilder.True<T_RKMX>();
            Func<T_RKMX, System.Int32> order = p => p.CPID;
            return GetPageInfo<System.Int32>(where, order, true, pageIndex, pageSize, out pageCount, out totalRecord);
        }

        public List<T_RKMX> GetAllT_RKMX(T_RKMX info)
        {
            Expression<Func<T_RKMX, bool>> where = PredicateBuilder.True<T_RKMX>();
            return base.GetAllModels<System.Int32>(where);
        }

        public List<T_RKMX> GetPageInfo<S>(Expression<Func<T_RKMX, bool>> where, Func<T_RKMX, S> order, bool desc, int pageIndex, int pageSize, out int pageCount, out int totalRecord)
        {
            totalRecord = 0;
            pageCount = 0;
            List<T_RKMX> list = new List<T_RKMX>();
            using (MedicalApparatusManageEntities hContext1 = new MedicalApparatusManageEntities())
            {
                try
                {
                    DbSet<T_RKMX> db = hContext1.Set<T_RKMX>();
                    DbQuery<T_RKMX> dbq = db.Include("T_YLCP").Include("T_RKD").Include("T_SupQY").Include("T_SupQY1").Include("T_CK");
                    totalRecord = db.Where(where.Compile()).Count();
                    if (totalRecord > 0)
                    {
                        pageCount = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(totalRecord / pageSize))) + 1;
                        if (desc)
                        {
                            list = dbq.Where(where.Compile()).OrderByDescending<T_RKMX, S>(order).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
                        }
                        else
                        {
                            list = dbq.Where(where.Compile()).OrderBy<T_RKMX, S>(order).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
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

        public List<T_RKMX> GetListModelById(Int32 id)
        {
            using (MedicalApparatusManageEntities hContext1 = new MedicalApparatusManageEntities())
            {
                try
                {
                    DbSet<T_RKMX> db = hContext1.Set<T_RKMX>();
                    DbQuery<T_RKMX> dbq = db.Include("T_YLCP").Include("T_RKD").Include("T_SupQY").Include("T_SupQY1").Include("T_CK");
                    return dbq.Where(p => p.RKID == id).ToList();
                }
                catch (Exception)
                {
                    return new List<T_RKMX>();
                }
            }
        }

        public int AddModelByRkdh(T_RKMX model, string RKDH)
        {
            using (MedicalApparatusManageEntities hContext1 = new MedicalApparatusManageEntities())
            {
                try
                {
                    var parentModel = hContext1.Set<T_RKD>().Where(p => p.RKDH == RKDH).FirstOrDefault();
                    parentModel.ISSH = 0;
                    var rkid = parentModel.RKID;
                    model.RKID = rkid;
                    hContext1.Set<T_RKMX>().Add(model);
                    #region 
                    //对新库存数据进行修改
                    //DbSet<T_KC> kcdb = hContext1.Set<T_KC>();
                    //var newkc = kcdb.Where(p => p.CPID == model.CPID && p.CPPH == model.CPPH && p.CKID == model.CKID).FirstOrDefault();
                    //if (newkc == null)
                    //{
                    //    T_KC newKc = new T_KC()
                    //    {
                    //        CPID = model.CPID,
                    //        CKID = model.CKID,
                    //        CPNUM = Convert.ToInt32(model.CPNUM),
                    //        FlAG = 1,
                    //        CPPH = model.CPPH,
                    //        CPSCRQ = model.CPSCRQ,
                    //        CPYXQ = model.CPYXQ,
                    //        SupID = model.SupID,
                    //        ScqyID = model.ScqyID
                    //    };
                    //    kcdb.Add(newKc);
                    //}
                    //else
                    //{
                    //    newkc.CPNUM = newkc.CPNUM + Convert.ToInt32(model.CPNUM);
                    //    newkc.CPYXQ = model.CPYXQ;
                    //    newkc.CPSCRQ = model.CPSCRQ;
                    //}

                    #endregion

                    return hContext1.SaveChanges();
                }
                catch (Exception ex)
                {
                    return 0;
                }

            }
        }

        public List<T_RKMX> GetT_RKMXByRkid(int rkid)
        {
            List<T_RKMX> list = new List<T_RKMX>();
            using (MedicalApparatusManageEntities hContext1 = new MedicalApparatusManageEntities())
            {
                try
                {
                    DbSet<T_RKMX> db = hContext1.Set<T_RKMX>();
                    DbQuery<T_RKMX> dbq = db.Include("T_YLCP").Include("T_RKD").Include("T_SupQY").Include("T_SupQY1").Include("T_CK");
                    list = dbq.Where(p => p.RKID == rkid).ToList(); ;
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
                    DbSet<T_RKMX> db = hContext1.Set<T_RKMX>();
                    T_RKMX model = db.Where(p => p.GUID == guid).FirstOrDefault();
                    #region 
                    //对原库存数据进行修改
                    //DbSet<T_KC> kcdb = hContext1.Set<T_KC>();
                    //var oldkc = kcdb.Where(p => p.CPID == model.CPID && p.CPPH == model.CPPH && p.CKID == model.CKID).FirstOrDefault();
                    //if (oldkc != null)
                    //{
                    //    oldkc.CPNUM = oldkc.CPNUM - Convert.ToInt32(model.CPNUM);
                    //}

                    #endregion

                    var dbparent = hContext1.Set<T_RKD>().Find(model.RKID);
                    dbparent.ISSH = 0;
                    db.Remove(model);
                    return hContext1.SaveChanges();
                }
                catch (Exception)
                {
                    return 0;
                }
            }
        }

        public List<T_RKMX> GetT_RKMXByRkdh(string rkdh)
        {
            List<T_RKMX> list = new List<T_RKMX>();
            using (MedicalApparatusManageEntities hContext1 = new MedicalApparatusManageEntities())
            {
                try
                {
                    var rkd = hContext1.Set<T_RKD>().Where(p => p.RKDH == rkdh).FirstOrDefault();
                    if (rkd != null)
                    {
                        DbSet<T_RKMX> db = hContext1.Set<T_RKMX>();
                        DbQuery<T_RKMX> dbq = db.Include("T_YLCP").Include("T_RKD").Include("T_SupQY").Include("T_SupQY1").Include("T_CK");
                        list = dbq.Where(p => p.RKID == rkd.RKID).ToList();
                    }
                }
                catch (Exception ex)
                {
                }
            }
            return list;
        }

        //public override int UpdateModel(T_RKMX model, params object[] keyValues)
        //{
        //    int result = 0;
        //    using (MedicalApparatusManageEntities hContext1 = new MedicalApparatusManageEntities())
        //    {
        //        try
        //        {
        //            DbSet<T_RKMX> db = hContext1.Set<T_RKMX>();
        //            DbSet<T_KC> kcdb = hContext1.Set<T_KC>();

        //            T_RKMX oldModel = db.Find(keyValues);

        //            //对原库存数据进行修改
        //            var oldkc = kcdb.Where(p => p.CPID == oldModel.CPID && p.CPPH == oldModel.CPPH && p.CKID == oldModel.CKID).FirstOrDefault();
        //            if (oldkc != null)
        //            {
        //                oldkc.CPNUM = oldkc.CPNUM - Convert.ToInt32(oldModel.CPNUM);
        //            }

        //            //对新库存数据进行修改
        //            var newkc = kcdb.Where(p => p.CPID == model.CPID && p.CPPH == model.CPPH && p.CKID == model.CKID).FirstOrDefault();
        //            if (newkc == null)
        //            {
        //                T_KC newKc = new T_KC()
        //                {
        //                    CPID = model.CPID,
        //                    CKID = model.CKID,
        //                    CPNUM = Convert.ToInt32(model.CPNUM),
        //                    FlAG = 1,
        //                    CPPH = model.CPPH,
        //                    CPSCRQ = model.CPSCRQ,
        //                    CPYXQ = model.CPYXQ,
        //                    SupID = model.SupID,
        //                    ScqyID = model.ScqyID
        //                };
        //                kcdb.Add(newKc);
        //            }
        //            else
        //            {
        //                newkc.CPNUM = newkc.CPNUM + Convert.ToInt32(model.CPNUM);
        //            }

        //            foreach (var item in model.GetType().GetProperties())
        //            {
        //                item.SetValue(oldModel, item.GetValue(model, null), null);
        //            }
        //            hContext1.SaveChanges();
        //        }
        //        catch (Exception ex)
        //        {
        //            return 0;
        //        }
        //    }
        //    return result;
        //}
    }
}
