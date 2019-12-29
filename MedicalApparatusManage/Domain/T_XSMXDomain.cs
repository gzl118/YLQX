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
    public class T_XSMXDomain : BaseDomain<T_XSMX>
    {
        static T_XSMXDomain _instance;
        private T_XSMXDomain()
        {
        }
        //单例模式
        static public T_XSMXDomain GetInstance()
        {
            if (_instance == null)
            {
                _instance = new T_XSMXDomain();
            }
            return _instance;
        }

        public List<T_XSMX> PageT_XSMX(T_XSMX info, DateTime? startTime, DateTime? endTime, int pageIndex, int pageSize, out int pageCount, out int totalRecord)
        {
            Expression<Func<T_XSMX, bool>> where = PredicateBuilder.True<T_XSMX>();
            if (info.T_YLCP != null && info.T_YLCP.T_SupQY1 != null && info.T_YLCP.T_SupQY1.SupID != 0)
            {
                where = where.And(p => p.T_YLCP != null && p.T_YLCP.T_SupQY1 != null && p.T_YLCP.T_SupQY1.SupID == info.T_YLCP.T_SupQY1.SupID);
            }

            if (startTime != null)
            {
                where = where.And(p => p.T_XSD.XSRQ >= startTime.Value);
            }
            if (endTime != null)
            {
                where = where.And(p => p.T_XSD.XSRQ <= endTime.Value);
            }
            if (info.CPID != 0)
            {
                where = where.And(p => p.CPID == info.CPID);
            }
            Func<T_XSMX, System.Int32> order = p => p.CPID;
            return GetPageInfo<System.Int32>(where, order, true, pageIndex, pageSize, out pageCount, out totalRecord);
        }

        public List<T_XSMX> GetAllT_XSMX(T_XSMX info)
        {
            Expression<Func<T_XSMX, bool>> where = PredicateBuilder.True<T_XSMX>();
            if (info.XSID != null && info.XSID > 0)
            {
                where = where.And(p => p.XSID == info.XSID);
            }
            return GetAllModels<System.Int32>(where);
        }

        public override List<T_XSMX> GetPageInfo<S>(Expression<Func<T_XSMX, bool>> where, Func<T_XSMX, S> order, bool desc, int pageIndex, int pageSize, out int pageCount, out int totalRecord)
        {
            totalRecord = 0;
            pageCount = 0;
            List<T_XSMX> list = new List<T_XSMX>();
            using (MedicalApparatusManageEntities hContext1 = new MedicalApparatusManageEntities())
            {
                try
                {
                    DbSet<T_XSMX> db = hContext1.Set<T_XSMX>();
                    DbQuery<T_XSMX> dbq = db.Include("T_XSD").Include("T_YLCP").Include("T_YLCP.T_SupQY").Include("T_YLCP.T_SupQY1").Include("T_XSD.T_CusQY");
                    totalRecord = db.Where(where.Compile()).Count();
                    if (totalRecord > 0)
                    {
                        pageCount = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(totalRecord / pageSize))) + 1;
                        if (desc)
                        {
                            list = dbq.Where(where.Compile()).OrderByDescending<T_XSMX, string>(p => p.T_XSD.XSDH).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
                        }
                        else
                        {
                            list = dbq.Where(where.Compile()).OrderBy<T_XSMX, string>(p => p.T_XSD.XSDH).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
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

        public List<T_XSMX> GetAllModels<S>(Expression<Func<T_XSMX, bool>> where)
        {
            List<T_XSMX> list = new List<T_XSMX>();
            using (MedicalApparatusManageEntities hContext1 = new MedicalApparatusManageEntities())
            {
                try
                {
                    DbSet<T_XSMX> db = hContext1.Set<T_XSMX>();
                    DbQuery<T_XSMX> dbq = db.Include("T_YLCP");
                    list = dbq.Where(where.Compile()).ToList();
                }
                catch (Exception)
                {

                }
                return list;
            }
        }

        public int AddModelByXsdh(T_XSMX model, string XSDH)
        {
            using (MedicalApparatusManageEntities hContext1 = new MedicalApparatusManageEntities())
            {
                try
                {
                    int xsid = hContext1.Set<T_XSD>().Where(p => p.XSDH == XSDH).FirstOrDefault().XSID;
                    model.XSID = xsid;
                    hContext1.Set<T_XSMX>().Add(model);
                    return hContext1.SaveChanges();
                }
                catch (Exception ex)
                {
                    return 0;
                }

            }
        }

        public List<T_XSMX> GetT_XSMXByXsid(int xsid)
        {
            List<T_XSMX> list = new List<T_XSMX>();
            using (MedicalApparatusManageEntities hContext1 = new MedicalApparatusManageEntities())
            {
                try
                {
                    DbSet<T_XSMX> db = hContext1.Set<T_XSMX>();
                    DbQuery<T_XSMX> dbq = db.Include("T_XSD").Include("T_YLCP").Include("T_YLCP.T_SupQY").Include("T_YLCP.T_SupQY1").Include("T_XSD.T_CusQY");
                    list = dbq.Where(p => p.XSID == xsid).ToList(); ;
                }
                catch (Exception ex)
                {
                }
            }
            return list;
        }

        public List<T_XSMX> GetT_XSMXByXsdh(string xsdh)
        {
            List<T_XSMX> list = new List<T_XSMX>();
            using (MedicalApparatusManageEntities hContext1 = new MedicalApparatusManageEntities())
            {
                try
                {
                    var xsd = hContext1.Set<T_XSD>().Where(p => p.XSDH == xsdh).FirstOrDefault();
                    if (xsd != null)
                    {
                        DbSet<T_XSMX> db = hContext1.Set<T_XSMX>();
                        DbQuery<T_XSMX> dbq = db.Include("T_YLCP").Include("T_XSD").Include("T_YLCP.T_SupQY").Include("T_YLCP.T_SupQY1").Include("T_XSD.T_CusQY");
                        list = dbq.Where(p => p.XSID == xsd.XSID).ToList();
                    }
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
                    DbSet<T_XSMX> db = hContext1.Set<T_XSMX>();
                    T_XSMX model = db.Where(p => p.GUID == guid).FirstOrDefault();
                    db.Remove(model);
                    return hContext1.SaveChanges();
                }
                catch (Exception)
                {
                    return 0;
                }
            }
        }
    }
}
