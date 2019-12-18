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
    public class T_SupQYDomain : BaseDomain<T_SupQY>
    {
        static T_SupQYDomain _instance;
        private T_SupQYDomain()
        {
        }
        //单例模式
        static public T_SupQYDomain GetInstance()
        {
            if (_instance == null)
            {
                _instance = new T_SupQYDomain();
            }
            return _instance;
        }

        public List<T_SupQY> PageT_SupQY(T_SupQY info, DateTime? startTime, DateTime? endTime, int pageIndex, int pageSize, out int pageCount, out int totalRecord)
        {
            Expression<Func<T_SupQY, bool>> where = PredicateBuilder.True<T_SupQY>();
            where = where.And(t => t.WhsID == info.WhsID);
            if (!String.IsNullOrEmpty(info.SupMC))
            {
                where = where.And(p => p.SupMC.Contains(info.SupMC));
            }
            if (!String.IsNullOrEmpty(info.SupDWXZ))
            {
                where = where.And(p => p.SupDWXZ == info.SupDWXZ);
            }
            if (info.SupStatus != null)
            {
                where = where.And(p => p.SupStatus == info.SupStatus);
            }
            Func<T_SupQY, System.Int32> order = p => p.SupID;
            return base.GetPageInfo<System.Int32>(where, order, true, pageIndex, pageSize, out pageCount, out totalRecord);
        }

        public List<T_SupQY> GetAllT_SupQY(T_SupQY info)
        {
            Expression<Func<T_SupQY, bool>> where = PredicateBuilder.True<T_SupQY>();
            if (info.SupStatus.HasValue)
            {
                where = where.And(p => p.SupStatus == info.SupStatus);
            }
            return base.GetAllModels<System.Int32>(where);
        }

        public List<T_SupQY> GetPageInfo<S>(Expression<Func<T_SupQY, bool>> where, Func<T_SupQY, S> order, bool desc, int pageIndex, int pageSize, out int pageCount, out int totalRecord)
        {
            totalRecord = 0;
            pageCount = 0;
            List<T_SupQY> list = new List<T_SupQY>();
            using (MedicalApparatusManageEntities hContext1 = new MedicalApparatusManageEntities())
            {
                try
                {
                    DbSet<T_SupQY> db = hContext1.Set<T_SupQY>();
                    DbQuery<T_SupQY> dbq = db.Include("T_WhsQY").Include("T_QYZZ");
                    totalRecord = db.Where(where.Compile()).Count();
                    if (totalRecord > 0)
                    {
                        pageCount = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(totalRecord / pageSize))) + 1;
                        if (desc)
                        {
                            list = dbq.Where(where.Compile()).OrderByDescending<T_SupQY, S>(order).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
                        }
                        else
                        {
                            list = dbq.Where(where.Compile()).OrderBy<T_SupQY, S>(order).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
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

        public int Sh(int id, int status)
        {
            int result = 0;
            using (MedicalApparatusManageEntities hContext1 = new MedicalApparatusManageEntities())
            {
                try
                {
                    var model = hContext1.Set<T_SupQY>().Find(id);
                    model.SupStatus = status;
                    hContext1.SaveChanges();
                    result = 1;
                }
                catch (Exception ex)
                {

                }
            }
            return result;
        }

        public void OperQyzz(int qyid, string qyzzFiles)
        {
            if (string.IsNullOrEmpty(qyzzFiles) || qyid == 0)
            {
                return;
            }
            string[] files = qyzzFiles.Split(';');
            List<T_QYZZ> addFiles = new List<T_QYZZ>();
            using (MedicalApparatusManageEntities hContext1 = new MedicalApparatusManageEntities())
            {
                try
                {
                    foreach (var item in files)
                    {
                        if (!string.IsNullOrEmpty(item))
                        {
                            T_QYZZ qyzz = new T_QYZZ() { ZZFJ = item, QYID = qyid };
                            addFiles.Add(qyzz);
                        }
                    }
                    //先删除后增加
                    DbSet<T_QYZZ> db = hContext1.Set<T_QYZZ>();
                    var deleteFiles = db.Where(p => p.QYID == qyid);
                    db.RemoveRange(deleteFiles);
                    db.AddRange(addFiles);
                    hContext1.SaveChanges();
                }
                catch (Exception ex)
                {

                }
            }
        }
    }
}
