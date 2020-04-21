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

            if (!String.IsNullOrEmpty(info.THMC))
            {
                where = where.And(p => p.THMC.Contains(info.THMC));
            }
            if (!String.IsNullOrEmpty(info.SQR))
            {
                where = where.And(p => p.SQR == info.SQR);
            }
            if (startTime != null)
            {
                where = where.And(p => p.SQRQ >= startTime.Value);
            }
            if (endTime != null)
            {
                where = where.And(p => p.SQRQ <= endTime.Value);
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
                    DbQuery<T_THD> dbq = db.Include("T_YSD");
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
                catch (Exception ex)
                {
                    return null;
                }
                return list;
            }
        }

        public string GetTHOrderNum(string prefix, SysUser user)
        {
            int count = 0;
            using (MedicalApparatusManageEntities hContext1 = new MedicalApparatusManageEntities())
            {
                try
                {
                    string sql = $@"SELECT ISNULL(MAX(SUBSTRING(THDH, 12,8)),0) + 1 FROM T_THD WHERE THCJR='{user.UserAccount}' AND SUBSTRING(THDH,3,6)=CONVERT(varchar(100), GETDATE(), 12)";
                    count = hContext1.Database.SqlQuery<int>(sql).FirstOrDefault();
                    //count = hContext1.Set<T_THD>().Where(p => p.THCJR == user.UserAccount).Count();
                }
                catch (Exception ex)
                {

                }
            }
            string result = prefix + DateTime.Now.ToString("yyMMdd") + user.UserId.ToString().PadLeft(3, '0') + count.ToString().PadLeft(5, '0');
            return result;
        }
        public int Sh(int id, int status)
        {
            int result = 0;
            using (MedicalApparatusManageEntities hContext1 = new MedicalApparatusManageEntities())
            {
                try
                {
                    var model = hContext1.Set<T_THD>().Find(id);
                    model.ISSH = status;

                    #region 更新库存
                    if (model.RKFlag == 1)  //入库后退货
                    {
                        var ysdModel = hContext1.Set<T_YSD>().Find(model.YSID);
                        if (ysdModel != null && ysdModel.YSFLAG == "合格收货")  //只有验收单是“合格收货”的才会入库，才能引起库存变化
                        {
                            var lstMX = hContext1.Set<T_THMX>().Where(p => p.THID == id).ToList();
                            if (lstMX != null && lstMX.Count > 0)
                            {
                                foreach (var item in lstMX)
                                {
                                    DbSet<T_KC> kcdb = hContext1.Set<T_KC>();
                                    var kc = kcdb.Where(p => p.CPID == item.CPID && p.CPPH == item.CPPH && p.CKID == item.CKID).FirstOrDefault();
                                    if (kc == null)
                                    {
                                        return 0;
                                    }
                                    kc.CPNUM = kc.CPNUM - Convert.ToInt32(item.CPNUM);
                                }
                            }
                        }
                    }
                    #endregion

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
                    var dbchild = hContext1.Set<T_THMX>();
                    var lst = dbchild.Where(p => p.THID == id);
                    if (lst != null && lst.Count() > 0)
                        dbchild.RemoveRange(lst);
                    var db = hContext1.Set<T_THD>();
                    var model = db.Find(id);

                    #region 如果当前退货单使对应的验收单完结，则删除时更改验收单置为未完结状态
                    //if (model.IsFinish == 1)
                    //{
                    //    var dbYSD = hContext1.Set<T_YSD>();
                    //    var modelYSD = dbYSD.Find(model.YSID);
                    //    if (modelYSD != null && modelYSD.YSID != 0)
                    //    {
                    //        modelYSD.IsTHFinish = 0;
                    //    }
                    //}
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
