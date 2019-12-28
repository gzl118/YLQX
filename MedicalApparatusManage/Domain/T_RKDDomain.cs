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
    public class T_RKDDomain : BaseDomain<T_RKD>
    {
        static T_RKDDomain _instance;
        private T_RKDDomain()
        {
        }
        //单例模式
        static public T_RKDDomain GetInstance()
        {
            if (_instance == null)
            {
                _instance = new T_RKDDomain();
            }
            return _instance;
        }
        public int Sh(int id, int status)
        {
            int result = 0;
            using (MedicalApparatusManageEntities hContext1 = new MedicalApparatusManageEntities())
            {
                try
                {
                    var model = hContext1.Set<T_RKD>().Find(id);
                    model.ISSH = status;
                    hContext1.SaveChanges();
                    result = 1;
                }
                catch (Exception ex)
                {

                }
            }
            return result;
        }
        public List<T_RKD> PageT_RKD(T_RKD info, DateTime? startTime, DateTime? endTime, int pageIndex, int pageSize, out int pageCount, out int totalRecord)
        {
            Expression<Func<T_RKD, bool>> where = PredicateBuilder.True<T_RKD>();
            if (!String.IsNullOrEmpty(info.CKGLRY))
            {
                where = where.And(p => info.CKGLRY.Equals(p.CKGLRY));
            }
            if (startTime != null)
            {
                where = where.And(p => p.JSRQ >= startTime.Value);
            }
            if (endTime != null)
            {
                where = where.And(p => p.JSRQ <= endTime.Value);
            }
            Func<T_RKD, System.Int32> order = p => p.RKID;
            return GetPageInfo<System.Int32>(where, order, true, pageIndex, pageSize, out pageCount, out totalRecord);
        }

        public List<T_RKD> GetAllT_RKD(T_RKD info)
        {
            Expression<Func<T_RKD, bool>> where = PredicateBuilder.True<T_RKD>();
            return base.GetAllModels<System.Int32>(where);
        }

        public List<T_RKD> GetPageInfo<S>(Expression<Func<T_RKD, bool>> where, Func<T_RKD, S> order, bool desc, int pageIndex, int pageSize, out int pageCount, out int totalRecord)
        {
            totalRecord = 0;
            pageCount = 0;
            List<T_RKD> list = new List<T_RKD>();
            using (MedicalApparatusManageEntities hContext1 = new MedicalApparatusManageEntities())
            {
                try
                {
                    DbSet<T_RKD> db = hContext1.Set<T_RKD>();
                    totalRecord = db.Where(where.Compile()).Count();
                    if (totalRecord > 0)
                    {
                        pageCount = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(totalRecord / pageSize))) + 1;
                        if (desc)
                        {
                            list = db.Where(where.Compile()).OrderByDescending<T_RKD, S>(order).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
                        }
                        else
                        {
                            list = db.Where(where.Compile()).OrderBy<T_RKD, S>(order).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
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

        public List<T_RKD> GetListModelById(Int32 id)
        {
            using (MedicalApparatusManageEntities hContext1 = new MedicalApparatusManageEntities())
            {
                try
                {
                    DbSet<T_RKD> db = hContext1.Set<T_RKD>();
                    return db.Where(p => p.RKID == id).ToList();
                }
                catch (Exception)
                {
                    return new List<T_RKD>();
                }
            }
        }

        public string GetRkOrderNum(SysUser user)
        {
            int count = 0;
            using (MedicalApparatusManageEntities hContext1 = new MedicalApparatusManageEntities())
            {
                try
                {
                    count = hContext1.Set<T_RKD>().Where(p => p.RKCJR == user.UserAccount).Count() + 1;
                }
                catch (Exception ex)
                {

                }
            }
            return "RK" + DateTime.Now.ToString("yyMMdd") + user.UserId.ToString().PadLeft(3, '0') + (count + 1).ToString().PadLeft(5, '0');
        }

        public int GetRkidByRkdh(string rkdh)
        {
            int result = 0;
            using (MedicalApparatusManageEntities hContext1 = new MedicalApparatusManageEntities())
            {
                try
                {
                    var data = hContext1.Set<T_RKD>().Where(p => p.RKDH == rkdh).FirstOrDefault();
                    if (data != null)
                    {
                        result = data.RKID;
                    }
                }
                catch (Exception ex)
                {

                }
            }
            return result;
        }
    }
}
