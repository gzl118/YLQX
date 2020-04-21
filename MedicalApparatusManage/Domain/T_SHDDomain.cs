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
    public class T_SHDDomain : BaseDomain<T_SHD>
    {
        static T_SHDDomain _instance;
        private T_SHDDomain()
        {
        }
        //单例模式
        static public T_SHDDomain GetInstance()
        {
            if (_instance == null)
            {
                _instance = new T_SHDDomain();
            }
            return _instance;
        }

        public List<T_SHD> PageT_SHD(T_SHD info, DateTime? startTime, DateTime? endTime, int pageIndex, int pageSize, out int pageCount, out int totalRecord)
        {
            Expression<Func<T_SHD, bool>> where = PredicateBuilder.True<T_SHD>();
            if (!String.IsNullOrEmpty(info.SHDH))
            {
                where = where.And(p => p.SHDH.Contains(info.SHDH));
            }
            if (!String.IsNullOrEmpty(info.SQR))
            {
                where = where.And(p => info.SQR.Contains(p.SQR));
            }
            if (startTime != null)
            {
                where = where.And(p => p.SQRQ >= startTime.Value);
            }
            if (endTime != null)
            {
                where = where.And(p => p.SQRQ <= endTime.Value);
            }
            Func<T_SHD, System.String> order = p => p.SHDH;
            return GetPageInfo<System.String>(where, order, true, pageIndex, pageSize, out pageCount, out totalRecord);
        }

        public List<T_SHD> GetAllT_SHD(T_SHD info)
        {
            Expression<Func<T_SHD, bool>> where = PredicateBuilder.True<T_SHD>();
            return base.GetAllModels<System.Int32>(where);
        }

        public List<T_SHD> GetPageInfo<S>(Expression<Func<T_SHD, bool>> where, Func<T_SHD, S> order, bool desc, int pageIndex, int pageSize, out int pageCount, out int totalRecord)
        {
            totalRecord = 0;
            pageCount = 0;
            List<T_SHD> list = new List<T_SHD>();
            using (MedicalApparatusManageEntities hContext1 = new MedicalApparatusManageEntities())
            {
                try
                {
                    DbSet<T_SHD> db = hContext1.Set<T_SHD>();
                    totalRecord = db.Where(where.Compile()).Count();
                    if (totalRecord > 0)
                    {
                        pageCount = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(totalRecord / pageSize))) + 1;
                        if (desc)
                        {
                            list = db.Where(where.Compile()).OrderByDescending<T_SHD, S>(order).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
                        }
                        else
                        {
                            list = db.Where(where.Compile()).OrderBy<T_SHD, S>(order).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
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

        public string GetSHOrderNum(SysUser user)
        {
            int count = 0;
            using (MedicalApparatusManageEntities hContext1 = new MedicalApparatusManageEntities())
            {
                try
                {
                    string sql = $@"SELECT ISNULL(MAX(SUBSTRING(SHDH, 12,8)),0) + 1 FROM T_SHD WHERE SHCJR='{user.UserAccount}' AND SUBSTRING(SHDH,3,6)=CONVERT(varchar(100), GETDATE(), 12)";
                    count = hContext1.Database.SqlQuery<int>(sql).FirstOrDefault();
                    //count = hContext1.Set<T_SHD>().Where(p => p.SHCJR == user.UserAccount).Count();
                }
                catch (Exception ex)
                {

                }
            }
            return "SH" + DateTime.Now.ToString("yyMMdd") + user.UserId.ToString().PadLeft(3, '0') + count.ToString().PadLeft(5, '0');
        }
        public List<T_SHMX> GetT_SHMXByshid(int shid)
        {
            List<T_SHMX> list = new List<T_SHMX>();
            using (MedicalApparatusManageEntities hContext1 = new MedicalApparatusManageEntities())
            {
                try
                {
                    DbSet<T_SHMX> db = hContext1.Set<T_SHMX>();
                    DbQuery<T_SHMX> dbq = db.Include("T_YLCP").Include("T_CK").Include("T_YLCP.T_SupQY1");
                    list = dbq.Where(p => p.SHID == shid).ToList(); ;
                }
                catch (Exception ex)
                {
                }
            }
            return list;
        }
        public List<T_SHMX> GetT_SHMXByCkdh(string shdh)
        {
            List<T_SHMX> list = new List<T_SHMX>();
            using (MedicalApparatusManageEntities hContext1 = new MedicalApparatusManageEntities())
            {
                try
                {
                    var ckd = hContext1.Set<T_SHD>().Where(p => p.SHDH == shdh).FirstOrDefault();
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
        public int Sh(int id, int status)
        {
            int result = 0;
            using (MedicalApparatusManageEntities hContext1 = new MedicalApparatusManageEntities())
            {
                try
                {
                    var model = hContext1.Set<T_SHD>().Find(id);
                    model.ISSH = status;
                    if (status == 1) //审核通过，修改库存
                    {
                        var lst = hContext1.Set<T_SHMX>().Where(p => p.SHID == id).ToList();
                        if (lst != null && lst.Count > 0)
                        {
                            lst.ForEach(item =>
                            {
                                //对新库存数据进行修改
                                DbSet<T_KC> kcdb = hContext1.Set<T_KC>();
                                var kc = kcdb.Where(p => p.CPID == item.CPID && p.CPPH == item.CPPH && p.CKID == item.CKID).FirstOrDefault();
                                if (kc != null)
                                {
                                    kc.CPNUM = kc.CPNUM - Convert.ToInt32(item.CPNUM);
                                }
                            });
                        }
                    }
                    hContext1.SaveChanges();
                    result = 1;
                }
                catch (Exception ex)
                {

                }
            }
            return result;
        }
        public int Delete(int id)
        {
            int result = 0;
            using (MedicalApparatusManageEntities hContext1 = new MedicalApparatusManageEntities())
            {
                try
                {
                    var dbchild = hContext1.Set<T_SHMX>();
                    var lst = dbchild.Where(p => p.SHID == id);
                    if (lst != null && lst.Count() > 0)
                        dbchild.RemoveRange(lst);
                    var db = hContext1.Set<T_SHD>();
                    var model = db.Find(id);
                    db.Remove(model);
                    hContext1.SaveChanges();
                    result = 1;
                }
                catch (Exception ex)
                {

                }
            }
            return result;
        }
    }
}