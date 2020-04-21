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
    public class T_CKDDomain : BaseDomain<T_CKD>
    {
        static T_CKDDomain _instance;
        private T_CKDDomain()
        {
        }
        //单例模式
        static public T_CKDDomain GetInstance()
        {
            if (_instance == null)
            {
                _instance = new T_CKDDomain();
            }
            return _instance;
        }

        public List<T_CKD> PageT_CKD(T_CKD info, DateTime? startTime, DateTime? endTime, int pageIndex, int pageSize, int cpID, int scqyId, int ghId, out int pageCount, out int totalRecord)
        {
            Expression<Func<T_CKD, bool>> where = PredicateBuilder.True<T_CKD>();
            if (!String.IsNullOrEmpty(info.CKDH))
            {
                where = where.And(p => p.CKDH.Contains(info.CKDH));
            }
            if (!string.IsNullOrEmpty(info.CKMC))
            {
                where = where.And(p => !string.IsNullOrEmpty(p.CKMC) && p.CKMC.Contains(info.CKMC));
            }
            if (startTime != null)
            {
                where = where.And(p => p.CHRQ >= startTime.Value);
            }
            if (endTime != null)
            {
                where = where.And(p => p.CHRQ <= endTime.Value);
            }
            Func<T_CKD, System.String> order = p => p.CKDH;
            return GetPageInfo<System.String>(where, order, true, pageIndex, pageSize, cpID, scqyId, ghId, out pageCount, out totalRecord);
        }

        public List<T_CKD> GetAllT_CKD(T_CKD info)
        {
            Expression<Func<T_CKD, bool>> where = PredicateBuilder.True<T_CKD>();
            return base.GetAllModels<System.Int32>(where);
        }

        public List<T_CKD> GetPageInfo<S>(Expression<Func<T_CKD, bool>> where, Func<T_CKD, S> order, bool desc, int pageIndex, int pageSize, int cpID, int scqyId, int ghId, out int pageCount, out int totalRecord)
        {
            totalRecord = 0;
            pageCount = 0;
            List<T_CKD> list = new List<T_CKD>();
            using (MedicalApparatusManageEntities hContext1 = new MedicalApparatusManageEntities())
            {
                try
                {
                    Expression<Func<T_CKMX, bool>> whereCKMX = PredicateBuilder.True<T_CKMX>();
                    var isContain = false;
                    if (cpID != 0)
                    {
                        whereCKMX = whereCKMX.And(p => p.CPID == cpID);
                        isContain = true;
                    }
                    if (scqyId != 0)
                    {
                        var dbcp = hContext1.Set<T_YLCP>().Where(p => p.CPSCQYID == scqyId);
                        var lstCPID = dbcp.Select(p => p.CPID);
                        whereCKMX = whereCKMX.And(p => lstCPID.Contains(p.CPID));
                        isContain = true;
                    }
                    if (isContain)
                    {
                        var dbchild = hContext1.Set<T_CKMX>().Where(whereCKMX.Compile());
                        var lst = dbchild.Select(p => p.CKDID);
                        if (lst != null)
                            where = where.And(p => lst.Contains(p.CKID));
                    }
                    if (ghId != 0)
                    {
                        where = where.And(p => p.T_XSD != null && p.T_XSD.KHID == ghId);
                    }


                    DbSet<T_CKD> db = hContext1.Set<T_CKD>();
                    DbQuery<T_CKD> dbq = db.Include("T_XSD");
                    totalRecord = db.Where(where.Compile()).Count();
                    if (totalRecord > 0)
                    {
                        pageCount = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(totalRecord / pageSize))) + 1;
                        if (desc)
                        {
                            list = dbq.Where(where.Compile()).OrderByDescending<T_CKD, S>(order).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
                        }
                        else
                        {
                            list = dbq.Where(where.Compile()).OrderBy<T_CKD, S>(order).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
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

        public List<T_CKD> GetListModelById(Int32 id)
        {
            using (MedicalApparatusManageEntities hContext1 = new MedicalApparatusManageEntities())
            {
                try
                {
                    DbSet<T_CKD> db = hContext1.Set<T_CKD>();
                    DbQuery<T_CKD> dbq = db.Include("T_XSD");
                    return dbq.Where(p => p.CKID == id).ToList();
                }
                catch (Exception)
                {
                    return new List<T_CKD>();
                }
            }
        }

        public string GetCkOrderNum(SysUser user)
        {
            int count = 0;
            using (MedicalApparatusManageEntities hContext1 = new MedicalApparatusManageEntities())
            {
                try
                {
                    string sql = $@"SELECT ISNULL(MAX(SUBSTRING(CKDH, 12,8)),0) + 1 FROM T_CKD WHERE CKCJR='{user.UserAccount}' AND SUBSTRING(CKDH,3,6)=CONVERT(varchar(100), GETDATE(), 12)";
                    count = hContext1.Database.SqlQuery<int>(sql).FirstOrDefault();
                    //count = hContext1.Set<T_CKD>().Where(p => p.CKCJR == user.UserAccount).Count() + 1;
                }
                catch (Exception ex)
                {

                }
            }
            return "CK" + DateTime.Now.ToString("yyMMdd") + user.UserId.ToString().PadLeft(3, '0') + count.ToString().PadLeft(5, '0');
        }

        public int GetCkidByCkdh(string ckdh)
        {
            int result = 0;
            using (MedicalApparatusManageEntities hContext1 = new MedicalApparatusManageEntities())
            {
                try
                {
                    var data = hContext1.Set<T_CKD>().Where(p => p.CKDH == ckdh).FirstOrDefault();
                    if (data != null)
                    {
                        result = data.CKID;
                    }
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
                    var dbchild = hContext1.Set<T_CKMX>();
                    var lst = dbchild.Where(p => p.CKID == id);
                    if (lst != null && lst.Count() > 0)
                        dbchild.RemoveRange(lst);
                    var db = hContext1.Set<T_CKD>();
                    var model = db.Find(id);

                    #region 如果当前出库单使对应的销售单完结，则删除时更改销售单置为未完结状态
                    if (model.IsFinish == 1)
                    {
                        var dbXSD = hContext1.Set<T_XSD>();
                        var modelXSD = dbXSD.Find(model.XSID);
                        if (modelXSD != null)
                        {
                            modelXSD.IsFinish = 0;
                        }
                    }
                    #endregion

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
