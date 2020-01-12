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
    public class T_CGDDomain : BaseDomain<T_CGD>
    {
        static T_CGDDomain _instance;
        private T_CGDDomain()
        {
        }
        //单例模式
        static public T_CGDDomain GetInstance()
        {
            if (_instance == null)
            {
                _instance = new T_CGDDomain();
            }
            return _instance;
        }

        public List<T_CGD> PageT_CGD(T_CGD info, DateTime? startTime, DateTime? endTime, int pageIndex, int pageSize, out int pageCount, out int totalRecord)
        {
            Expression<Func<T_CGD, bool>> where = PredicateBuilder.True<T_CGD>();
            if (!String.IsNullOrEmpty(info.CGPERSON))
            {
                where = where.And(p => info.CGPERSON.Equals(p.CGPERSON));
            }
            if (!String.IsNullOrEmpty(info.CGDMC))
            {
                where = where.And(p => p.CGDMC != null && p.CGDMC.Contains(info.CGDMC));
            }
            if (startTime != null)
            {
                where = where.And(p => p.CGRQ >= startTime.Value);
            }
            if (endTime != null)
            {
                where = where.And(p => p.CGRQ <= endTime.Value);
            }

            Func<T_CGD, System.Int32> order = p => p.CGID;
            return base.GetPageInfo<System.Int32>(where, order, true, pageIndex, pageSize, out pageCount, out totalRecord);
        }

        public List<T_CGD> GetAllT_CGD(T_CGD info)
        {
            Expression<Func<T_CGD, bool>> where = PredicateBuilder.True<T_CGD>();
            return base.GetAllModels<System.Int32>(where);
        }

        public List<T_CGD> GetPageInfo<S>(Expression<Func<T_CGD, bool>> where, Func<T_CGD, S> order, bool desc, int pageIndex, int pageSize, out int pageCount, out int totalRecord)
        {
            totalRecord = 0;
            pageCount = 0;
            List<T_CGD> list = new List<T_CGD>();
            using (MedicalApparatusManageEntities hContext1 = new MedicalApparatusManageEntities())
            {
                try
                {
                    DbSet<T_CGD> db = hContext1.Set<T_CGD>();
                    DbQuery<T_CGD> dbq = db.Include("T_WhsQY");
                    totalRecord = db.Where(where.Compile()).Count();
                    if (totalRecord > 0)
                    {
                        pageCount = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(totalRecord / pageSize))) + 1;
                        if (desc)
                        {
                            list = dbq.Where(where.Compile()).OrderByDescending<T_CGD, S>(order).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
                        }
                        else
                        {
                            list = dbq.Where(where.Compile()).OrderBy<T_CGD, S>(order).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
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

        public string GetCgOrderNum(string prefix, SysUser user)
        {
            int count = 0;
            using (MedicalApparatusManageEntities hContext1 = new MedicalApparatusManageEntities())
            {
                try
                {
                    count = hContext1.Set<T_CGD>().Where(p => p.CGCJR == user.UserAccount).Count() + 1;
                }
                catch (Exception ex)
                {

                }
            }
            string result = prefix + DateTime.Now.ToString("yyMMdd") + user.UserId.ToString().PadLeft(3, '0') + (count + 1).ToString().PadLeft(5, '0');
            return result;
        }

        public int Sh(int id, int status)
        {
            int result = 0;
            using (MedicalApparatusManageEntities hContext1 = new MedicalApparatusManageEntities())
            {
                try
                {
                    var model = hContext1.Set<T_CGD>().Find(id);
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
        public int SaveTPrice(int id, double tPrice, string dhid)
        {
            int result = 0;
            using (MedicalApparatusManageEntities hContext1 = new MedicalApparatusManageEntities())
            {
                try
                {
                    if (id == 0)
                    {
                        var model = hContext1.Set<T_CGD>().Where(p => p.CGDH == dhid).FirstOrDefault();
                        if (model.CGID != 0)
                        {
                            model.CGTotalPrice = tPrice;
                        }
                    }
                    else
                    {
                        var model = hContext1.Set<T_CGD>().Find(id);
                        model.CGTotalPrice = tPrice;
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
                    var dbchild = hContext1.Set<T_CGMX>();
                    var lst = dbchild.Where(p => p.CGID == id);
                    if (lst != null && lst.Count() > 0)
                        dbchild.RemoveRange(lst);
                    var db = hContext1.Set<T_CGD>();
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
