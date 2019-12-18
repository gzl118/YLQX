using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web;

namespace MedicalApparatusManage.Domain
{
    public class T_YSMXDomain : BaseDomain<T_YSMX>
    {
        static T_YSMXDomain _instance;
        private T_YSMXDomain()
        {
        }
        //单例模式
        static public T_YSMXDomain GetInstance()
        {
            if (_instance == null)
            {
                _instance = new T_YSMXDomain();
            }
            return _instance;
        }

        public int AddModelByYsdh(T_YSMX model, string YSDH)
        {
            using (MedicalApparatusManageEntities hContext1 = new MedicalApparatusManageEntities())
            {
                try
                {
                    int ysid = hContext1.Set<T_YSD>().Where(p => p.YSDH == YSDH).FirstOrDefault().YSID;
                    model.YSID = ysid;
                    hContext1.Set<T_YSMX>().Add(model);
                    return hContext1.SaveChanges();
                }
                catch (Exception ex)
                {
                    return 0;
                }

            }
        }

        public List<T_YSMX> GetT_YSMXByYsid(int ysid)
        {
            List<T_YSMX> list = new List<T_YSMX>();
            using (MedicalApparatusManageEntities hContext1 = new MedicalApparatusManageEntities())
            {
                try
                {
                    DbSet<T_YSMX> db = hContext1.Set<T_YSMX>();
                    DbQuery<T_YSMX> dbq = db.Include("T_YLCP").Include("T_YSD").Include("T_SupQY").Include("T_SupQY1");
                    list = dbq.Where(p => p.YSID == ysid).ToList(); ;
                }
                catch (Exception ex)
                {
                }
            }
            return list;
        }

        public List<T_YSMX> GetT_YSMXByYsdh(string ysdh)
        {
            List<T_YSMX> list = new List<T_YSMX>();
            using (MedicalApparatusManageEntities hContext1 = new MedicalApparatusManageEntities())
            {
                try
                {
                    var ysd = hContext1.Set<T_YSD>().Where(p => p.YSDH == ysdh).FirstOrDefault();
                    if (ysd != null)
                    {
                        DbSet<T_YSMX> db = hContext1.Set<T_YSMX>();
                        DbQuery<T_YSMX> dbq = db.Include("T_YLCP").Include("T_YSD").Include("T_SupQY").Include("T_SupQY1");
                        list = dbq.Where(p => p.YSID == ysd.YSID).ToList();
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
                    DbSet<T_YSMX> db = hContext1.Set<T_YSMX>();
                    T_YSMX model = db.Where(p => p.GUID == guid).FirstOrDefault();
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