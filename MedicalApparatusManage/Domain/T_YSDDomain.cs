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
    public class T_YSDDomain : BaseDomain<T_YSD>
    {
        static T_YSDDomain _instance;
        private T_YSDDomain()
        {
        }
        //单例模式
        static public T_YSDDomain GetInstance()
        {
            if (_instance == null)
            {
                _instance = new T_YSDDomain();
            }
            return _instance;
        }

        public List<T_YSD> PageT_YSD(T_YSD info, DateTime? startTime, DateTime? endTime, int pageIndex, int pageSize, int? cpId, int? cusId, out int pageCount, out int totalRecord)
        {
            Expression<Func<T_YSD, bool>> where = PredicateBuilder.True<T_YSD>();
            if (!String.IsNullOrEmpty(info.YSR))
            {
                where = where.And(p => info.YSR.Equals(p.YSR));
            }
            if (startTime != null)
            {
                where = where.And(p => p.YSRQ >= startTime.Value);
            }
            if (endTime != null)
            {
                where = where.And(p => p.YSRQ <= endTime.Value);
            }
            where = where.And(p => p.IsCGYS == info.IsCGYS);
            Func<T_YSD, System.Int32> order = p => p.YSID;
            return GetPageInfo<System.Int32>(where, order, true, pageIndex, pageSize, cpId, cusId, out pageCount, out totalRecord);
        }

        public List<T_YSD> GetAllT_YSD(T_YSD info)
        {
            Expression<Func<T_YSD, bool>> where = PredicateBuilder.True<T_YSD>();
            where = where.And(p => p.IsFinish == 0 && p.YSFLAG == "合格收货");
            return base.GetAllModels<System.Int32>(where);
        }

        public List<T_YSD> GetPageInfo<S>(Expression<Func<T_YSD, bool>> where, Func<T_YSD, S> order, bool desc, int pageIndex, int pageSize, int? cpId, int? cusId, out int pageCount, out int totalRecord)
        {
            totalRecord = 0;
            pageCount = 0;
            List<T_YSD> list = new List<T_YSD>();
            using (MedicalApparatusManageEntities hContext1 = new MedicalApparatusManageEntities())
            {
                try
                {
                    Expression<Func<T_YSMX, bool>> whereCGMX = PredicateBuilder.True<T_YSMX>();
                    var isContain = false;
                    if (cpId != null && cpId != 0)
                    {
                        whereCGMX = whereCGMX.And(p => p.CPID == cpId);
                        isContain = true;
                    }
                    if (cusId != null && cusId != 0)
                    {
                        whereCGMX = whereCGMX.And(p => p.ScqyID == cusId);
                        isContain = true;
                    }
                    if (isContain)
                    {
                        var dbchild = hContext1.Set<T_YSMX>().Where(whereCGMX.Compile());
                        var lst = dbchild.Select(p => p.YSID);
                        if (lst != null)
                            where = where.And(p => lst.Contains(p.YSID));
                    }

                    DbSet<T_YSD> db = hContext1.Set<T_YSD>();
                    totalRecord = db.Where(where.Compile()).Count();
                    if (totalRecord > 0)
                    {
                        pageCount = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(totalRecord / pageSize))) + 1;
                        if (desc)
                        {
                            list = db.Where(where.Compile()).OrderByDescending<T_YSD, S>(order).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
                        }
                        else
                        {
                            list = db.Where(where.Compile()).OrderBy<T_YSD, S>(order).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
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

        public string GetYsOrderNum(string prefix, SysUser user)
        {
            int count = 0;
            using (MedicalApparatusManageEntities hContext1 = new MedicalApparatusManageEntities())
            {
                try
                {
                    string sql = $@"SELECT ISNULL(MAX(SUBSTRING(YSDH, 12,8)),0) + 1 FROM T_YSD WHERE YSCJR='{user.UserAccount}' AND SUBSTRING(YSDH,3,6)=CONVERT(varchar(100), GETDATE(), 12)";
                    count = hContext1.Database.SqlQuery<int>(sql).FirstOrDefault();
                    //count = hContext1.Set<T_YSD>().Where(p => p.YSCJR == user.UserAccount).Count() + 1;
                }
                catch (Exception ex)
                {

                }
            }
            string result = prefix + DateTime.Now.ToString("yyMMdd") + user.UserId.ToString().PadLeft(3, '0') + count.ToString().PadLeft(5, '0');
            return result;
        }
        public int Delete(int id)
        {
            int result = 0;
            using (MedicalApparatusManageEntities hContext1 = new MedicalApparatusManageEntities())
            {
                try
                {
                    var dbchild = hContext1.Set<T_YSMX>();
                    var lst = dbchild.Where(p => p.YSID == id);
                    if (lst != null && lst.Count() > 0)
                        dbchild.RemoveRange(lst);
                    var db = hContext1.Set<T_YSD>();
                    var model = db.Find(id);

                    #region 如果当前验收单使对应的采购单完结，则删除时更改采购单置为未完结状态
                    if (model.IsCGFinish == 1)
                    {
                        var dbCGD = hContext1.Set<T_CGD>();
                        var modelCGD = dbCGD.Where(p => p.CGDH == model.CGDH).FirstOrDefault();
                        if (modelCGD != null && modelCGD.CGID != 0)
                        {
                            modelCGD.IsFinish = 0;
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
        public void UpdateFinish(string ysdh)
        {
            using (MedicalApparatusManageEntities hContext1 = new MedicalApparatusManageEntities())
            {
                try
                {
                    var db = hContext1.Set<T_YSD>();
                    var model = db.Where(p => p.YSDH == ysdh).First();
                    if (model != null)
                    {
                        model.IsFinish = 1;
                        hContext1.SaveChanges();
                    }
                }
                catch (Exception ex)
                {

                }
            }
        }
        public void UpdateFinish(int ysid)
        {
            using (MedicalApparatusManageEntities hContext1 = new MedicalApparatusManageEntities())
            {
                try
                {
                    var db = hContext1.Set<T_YSD>();
                    var model = db.Find(ysid);
                    if (model != null)
                    {
                        model.IsTHFinish = 1;
                        hContext1.SaveChanges();
                    }
                }
                catch (Exception ex)
                {

                }
            }
        }
        public List<T_YSD> GetYSDByCondition(string hflag, int? cpId, int? supId, string cpph)
        {
            Expression<Func<T_YSD, bool>> where = PredicateBuilder.True<T_YSD>();
            if (!String.IsNullOrEmpty(hflag))
            {
                where = where.And(p => p.YSFLAG == hflag);
            }
            List<T_YSD> list = new List<T_YSD>();
            using (MedicalApparatusManageEntities hContext1 = new MedicalApparatusManageEntities())
            {
                try
                {
                    Expression<Func<T_YSMX, bool>> whereMX = PredicateBuilder.True<T_YSMX>();
                    var isContain = false;
                    if (cpId != null && cpId != 0)
                    {
                        whereMX = whereMX.And(p => p.CPID == cpId);
                        isContain = true;
                    }
                    if (supId != null && supId != 0)
                    {
                        whereMX = whereMX.And(p => p.SupID == supId);
                        isContain = true;
                    }
                    if (!String.IsNullOrEmpty(cpph))
                    {
                        whereMX = whereMX.And(p => !string.IsNullOrEmpty(p.CPPH) && p.CPPH.Contains(cpph));
                        isContain = true;
                    }
                    if (isContain)
                    {
                        var dbchild = hContext1.Set<T_YSMX>().Where(whereMX.Compile());
                        var lst = dbchild.Select(p => p.YSID);
                        if (lst != null)
                            where = where.And(p => lst.Contains(p.YSID));
                    }
                    DbSet<T_YSD> db = hContext1.Set<T_YSD>();
                    list = db.Where(where.Compile()).OrderByDescending(p => p.YSDH).ToList();
                }
                catch (Exception ex)
                {
                    return null;
                }
                return list;
            }
        }
    }
}
