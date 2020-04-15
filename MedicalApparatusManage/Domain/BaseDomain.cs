using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Web;
using System.Data.Entity.Validation;

namespace MedicalApparatusManage.Domain
{
    public class BaseDomain<T> where T : class, new()
    {

        public virtual T GetModelById(params object[] keyValues)
        {
            using (MedicalApparatusManageEntities hContext1 = new MedicalApparatusManageEntities())
            {
                try
                {
                    return hContext1.Set<T>().Find(keyValues);
                }
                catch (Exception)
                {
                    return new T();
                }
            }
        }

        public virtual int DeleteModelById(params object[] keyValues)
        {
            using (MedicalApparatusManageEntities hContext1 = new MedicalApparatusManageEntities())
            {
                try
                {
                    DbSet<T> db = hContext1.Set<T>();
                    T model = db.Find(keyValues);
                    db.Remove(model);
                    return hContext1.SaveChanges();

                }
                catch (Exception ex)
                {
                    return 0;
                }
            }
        }

        public virtual int AddModel(T model)
        {
            using (MedicalApparatusManageEntities hContext1 = new MedicalApparatusManageEntities())
            {
                try
                {
                    hContext1.Set<T>().Add(model);
                    return hContext1.SaveChanges();
                }
                catch (DbEntityValidationException ex)
                {
                    return 0;
                }
                catch (Exception ex)
                {
                    return 0;
                }

            }
        }

        public virtual int UpdateModel(T model, params object[] keyValues)
        {
            using (MedicalApparatusManageEntities hContext1 = new MedicalApparatusManageEntities())
            {
                try
                {
                    DbSet<T> db = hContext1.Set<T>();
                    T oldModel = db.Find(keyValues);
                    foreach (var item in model.GetType().GetProperties())
                    {
                        item.SetValue(oldModel, item.GetValue(model, null), null);
                    }
                    hContext1.SaveChanges();
                    return 1;
                }
                catch (DbEntityValidationException dbEx)
                {
                    return 0;
                }
                catch (Exception ex)
                {
                    return 0;
                }
            }
        }

        public virtual List<T> GetPageInfo<S>(Expression<Func<T, bool>> where, Func<T, S> order, bool desc, int pageIndex, int pageSize, out int pageCount, out int totalRecord)
        {
            totalRecord = 0;
            pageCount = 0;
            List<T> list = new List<T>();
            using (MedicalApparatusManageEntities hContext1 = new MedicalApparatusManageEntities())
            {
                try
                {
                    DbSet<T> db = hContext1.Set<T>();
                    totalRecord = db.Where(where.Compile()).Count();
                    if (totalRecord > 0)
                    {
                        pageCount = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(totalRecord / pageSize))) + 1;
                        if (desc)
                        {
                            list = db.Where(where.Compile()).OrderByDescending<T, S>(order).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
                        }
                        else
                        {
                            list = db.Where(where.Compile()).OrderBy<T, S>(order).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
                        }
                    }
                }
                catch (Exception ex)
                {
                    int a = 0;
                }
                return list;
            }
        }

        public virtual List<T> GetAllModels<S>(Expression<Func<T, bool>> where)
        {
            List<T> list = new List<T>();
            using (MedicalApparatusManageEntities hContext1 = new MedicalApparatusManageEntities())
            {
                try
                {
                    DbSet<T> db = hContext1.Set<T>();
                    list = db.Where(where.Compile()).ToList();
                }
                catch (Exception)
                {

                }
                return list;
            }
        }



    }
}