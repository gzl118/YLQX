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

        public List<T_YSD> PageT_YSD(T_YSD info, DateTime? startTime, DateTime? endTime, int pageIndex, int pageSize, out int pageCount, out int totalRecord)
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
            Func<T_YSD, System.Int32> order = p => p.YSID;
            return GetPageInfo<System.Int32>(where, order, true, pageIndex, pageSize, out pageCount, out totalRecord);
        }

        public List<T_YSD> GetAllT_YSD(T_YSD info)
        {
            Expression<Func<T_YSD, bool>> where = PredicateBuilder.True<T_YSD>();
            return base.GetAllModels<System.Int32>(where);
        }

        public List<T_YSD> GetPageInfo<S>(Expression<Func<T_YSD, bool>> where, Func<T_YSD, S> order, bool desc, int pageIndex, int pageSize, out int pageCount, out int totalRecord)
        {
            totalRecord = 0;
            pageCount = 0;
            List<T_YSD> list = new List<T_YSD>();
            using (MedicalApparatusManageEntities hContext1 = new MedicalApparatusManageEntities())
            {
                try
                {
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
                    count = hContext1.Set<T_YSD>().Where(p => p.YSCJR == user.UserAccount).Count() + 1;
                }
                catch (Exception ex)
                {

                }
            }
            string result = prefix + DateTime.Now.ToString("yyMMdd") + user.UserId.ToString().PadLeft(3, '0') + (count + 1).ToString().PadLeft(5, '0');
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
