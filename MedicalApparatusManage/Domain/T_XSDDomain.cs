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
    public class T_XSDDomain : BaseDomain<T_XSD>
    {
        static T_XSDDomain _instance;
        private T_XSDDomain()
        {
        }
        //单例模式
        static public T_XSDDomain GetInstance()
        {
            if (_instance == null)
            {
                _instance = new T_XSDDomain();
            }
            return _instance;
        }

        public List<T_XSD> PageT_XSD(T_XSD info, DateTime? startTime, DateTime? endTime, int pageIndex, int pageSize, out int pageCount, out int totalRecord)
        {
            Expression<Func<T_XSD, bool>> where = PredicateBuilder.True<T_XSD>();
            if (!String.IsNullOrEmpty(info.XSDH))
            {
                where = where.And(p => p.XSDH.Contains(info.XSDH));
            }
            if (info.KHID != null && info.KHID > 0)
            {
                where = where.And(p => p.KHID == info.KHID);
            }
            if (!string.IsNullOrEmpty(info.XSRY))
            {
                where = where.And(p => info.XSRY.Equals(p.XSRY));
            }
            if (startTime != null)
            {
                where = where.And(p => p.XSRQ >= startTime.Value);
            }
            if (endTime != null)
            {
                where = where.And(p => p.XSRQ <= endTime.Value);
            }
            Func<T_XSD, System.String> order = p => p.XSDH;
            return GetPageInfo<System.String>(where, order, true, pageIndex, pageSize, out pageCount, out totalRecord);
        }

        public List<T_XSD> GetAllT_XSD(T_XSD info)
        {
            Expression<Func<T_XSD, bool>> where = PredicateBuilder.True<T_XSD>();
            return base.GetAllModels<System.Int32>(where);
        }

        public List<T_XSD> GetPageInfo<S>(Expression<Func<T_XSD, bool>> where, Func<T_XSD, S> order, bool desc, int pageIndex, int pageSize, out int pageCount, out int totalRecord)
        {
            totalRecord = 0;
            pageCount = 0;
            List<T_XSD> list = new List<T_XSD>();
            using (MedicalApparatusManageEntities hContext1 = new MedicalApparatusManageEntities())
            {
                try
                {
                    DbSet<T_XSD> db = hContext1.Set<T_XSD>();
                    DbQuery<T_XSD> dbq = db.Include("T_CusQY").Include("T_Person");
                    totalRecord = db.Where(where.Compile()).Count();
                    if (totalRecord > 0)
                    {
                        pageCount = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(totalRecord / pageSize))) + 1;
                        if (desc)
                        {
                            list = dbq.Where(where.Compile()).OrderByDescending<T_XSD, S>(order).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
                        }
                        else
                        {
                            list = dbq.Where(where.Compile()).OrderBy<T_XSD, S>(order).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
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

        public string GetXsOrderNum(SysUser user)
        {
            int count = 0;
            using (MedicalApparatusManageEntities hContext1 = new MedicalApparatusManageEntities())
            {
                try
                {
                    count = hContext1.Set<T_XSD>().Where(p => p.XSCJR == user.UserAccount).Count() + 1;
                }
                catch (Exception ex)
                {

                }
            }
            return "XS" + DateTime.Now.ToString("yyMMdd") + user.UserId.ToString().PadLeft(3, '0') + (count + 1).ToString().PadLeft(5, '0');
        }

        public int Sh(int id, int status)
        {
            int result = 0;
            using (MedicalApparatusManageEntities hContext1 = new MedicalApparatusManageEntities())
            {
                try
                {
                    var model = hContext1.Set<T_XSD>().Find(id);
                    model.XSFLAG = status;
                    hContext1.SaveChanges();
                    result = 1;
                }
                catch (Exception ex)
                {

                }
            }
            return result;
        }
        public int SaveTPrice(int id, double tPrice)
        {
            int result = 0;
            using (MedicalApparatusManageEntities hContext1 = new MedicalApparatusManageEntities())
            {
                try
                {
                    var model = hContext1.Set<T_XSD>().Find(id);
                    model.XSJE = tPrice;
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
