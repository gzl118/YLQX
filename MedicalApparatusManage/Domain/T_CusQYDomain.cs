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
    public class T_CusQYDomain : BaseDomain<T_CusQY>
    {
        static T_CusQYDomain _instance;
        private T_CusQYDomain()
        {
        }
        //单例模式
        static public T_CusQYDomain GetInstance()
        {
            if (_instance == null)
            {
                _instance = new T_CusQYDomain();
            }
            return _instance;
        }

        public List<T_CusQY> PageT_CusQY(T_CusQY info, DateTime? startTime, DateTime? endTime, int pageIndex, int pageSize, out int pageCount, out int totalRecord)
        {
            Expression<Func<T_CusQY, bool>> where = PredicateBuilder.True<T_CusQY>();
            if (!String.IsNullOrEmpty(info.CusMC))
            {
                where = where.And(p => p.CusMC.Contains(info.CusMC));
            }
            if (!String.IsNullOrEmpty(info.CusDWXZ))
            {
                where = where.And(p => p.CusDWXZ == info.CusDWXZ);
            }
            if (info.CusStatus != null)
            {
                where = where.And(p => p.CusStatus == info.CusStatus);
            }
            Func<T_CusQY, System.Int32> order = p => p.CusID;
            return base.GetPageInfo<System.Int32>(where, order, true, pageIndex, pageSize, out pageCount, out totalRecord);
        }

        public List<T_CusQY> GetAllT_CusQY(T_CusQY info)
        {
            Expression<Func<T_CusQY, bool>> where = PredicateBuilder.True<T_CusQY>();
            return base.GetAllModels<System.Int32>(where);
        }

        public List<T_CusQY> GetPageInfo<S>(Expression<Func<T_CusQY, bool>> where, Func<T_CusQY, S> order, bool desc, int pageIndex, int pageSize, out int pageCount, out int totalRecord)
        {
            totalRecord = 0;
            pageCount = 0;
            List<T_CusQY> list = new List<T_CusQY>();
            using (MedicalApparatusManageEntities hContext1 = new MedicalApparatusManageEntities())
            {
                try
                {
                    DbSet<T_CusQY> db = hContext1.Set<T_CusQY>();
                    DbQuery<T_CusQY> dbq = db.Include("T_WhsQY");
                    totalRecord = db.Where(where.Compile()).Count();
                    if (totalRecord > 0)
                    {
                        pageCount = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(totalRecord / pageSize))) + 1;
                        if (desc)
                        {
                            list = dbq.Where(where.Compile()).OrderByDescending<T_CusQY, S>(order).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
                        }
                        else
                        {
                            list = dbq.Where(where.Compile()).OrderBy<T_CusQY, S>(order).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
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
                    var model = hContext1.Set<T_CusQY>().Find(id);
                    model.CusStatus = status;
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
            List<T_CusQYZZ> addFiles = new List<T_CusQYZZ>();
            using (MedicalApparatusManageEntities hContext1 = new MedicalApparatusManageEntities())
            {
                try
                {
                    foreach (var item in files)
                    {
                        if (!string.IsNullOrEmpty(item))
                        {
                            T_CusQYZZ qyzz = new T_CusQYZZ() { ZZFJ = item, QYID = qyid };
                            addFiles.Add(qyzz);
                        }
                    }
                    //先删除后增加
                    DbSet<T_CusQYZZ> db = hContext1.Set<T_CusQYZZ>();
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
        public int Delete(int id)
        {
            var result = 0;
            try
            {
                using (MedicalApparatusManageEntities hContext1 = new MedicalApparatusManageEntities())
                {
                    var db = hContext1.Set<T_CusQYZZ>();
                    var deleteFiles = db.Where(p => p.QYID == id);
                    db.RemoveRange(deleteFiles);
                    var temp = hContext1.Set<T_CusQY>().Find(id);
                    hContext1.Set<T_CusQY>().Remove(temp);
                    hContext1.SaveChanges();
                    result = 1;
                }
            }
            catch (Exception ex)
            {

            }
            return result;
        }
    }
}
