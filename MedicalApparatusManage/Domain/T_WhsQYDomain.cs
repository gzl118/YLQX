﻿using MedicalApparatusManage.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;


namespace MedicalApparatusManage.Domain
{
    public class T_WhsQYDomain : BaseDomain<T_WhsQY>
    {
        static T_WhsQYDomain _instance;
        private T_WhsQYDomain()
        {
        }
        //单例模式
        static public T_WhsQYDomain GetInstance()
        {
            if (_instance == null)
            {
                _instance = new T_WhsQYDomain();
            }
            return _instance;
        }

        public List<T_WhsQY> PageT_WhsQY(T_WhsQY info, DateTime? startTime, DateTime? endTime, int pageIndex, int pageSize, out int pageCount, out int totalRecord)
        {
            Expression<Func<T_WhsQY, bool>> where = PredicateBuilder.True<T_WhsQY>();
            //if(!String.IsNullOrEmpty(info.WhsMC))
            //{
            //    where = where.And(p => p.WhsMC.Contains(info.WhsMC));
            //}
            //if(startTime != null && endTime != null)
            //{
            //    where = where.And(p => p.WhsZCSJ >= startTime.Value);
            //    where = where.And(p => p.WhsZCSJ <= endTime.Value);
            //}
            Func<T_WhsQY, System.Int32> order = p => p.WhsID;
            return base.GetPageInfo<System.Int32>(where, order, true, pageIndex, pageSize, out pageCount, out totalRecord);
        }

        public List<T_WhsQY> GetAllT_WhsQY(T_WhsQY info)
        {
            Expression<Func<T_WhsQY, bool>> where = PredicateBuilder.True<T_WhsQY>();
            return base.GetAllModels<System.Int32>(where);
        }


        public List<T_WhsQY> GetPageInfo<S>(Expression<Func<T_WhsQY, bool>> where, Func<T_WhsQY, S> order, bool desc, int pageIndex, int pageSize, out int pageCount, out int totalRecord)
        {
            totalRecord = 0;
            pageCount = 0;
            List<T_WhsQY> list = new List<T_WhsQY>();
            using (MedicalApparatusManageEntities hContext1 = new MedicalApparatusManageEntities())
            {
                try
                {
                    DbSet<T_WhsQY> db = hContext1.Set<T_WhsQY>();
                    DbQuery<T_WhsQY> dbq = db.Include("T_WhsQY");
                    totalRecord = db.Where(where.Compile()).Count();
                    if (totalRecord > 0)
                    {
                        pageCount = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(totalRecord / pageSize))) + 1;
                        if (desc)
                        {
                            list = dbq.Where(where.Compile()).OrderByDescending<T_WhsQY, S>(order).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
                        }
                        else
                        {
                            list = dbq.Where(where.Compile()).OrderBy<T_WhsQY, S>(order).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
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

        public void OperQyzz(int qyid, string qyzzFiles)
        {
            if (string.IsNullOrEmpty(qyzzFiles) || qyid == 0)
            {
                return;
            }
            string[] files = qyzzFiles.Split(';');
            List<T_WhsQYZZ> addFiles = new List<T_WhsQYZZ>();
            using (MedicalApparatusManageEntities hContext1 = new MedicalApparatusManageEntities())
            {
                try
                {
                    foreach (var item in files)
                    {
                        if (!string.IsNullOrEmpty(item))
                        {
                            T_WhsQYZZ qyzz = new T_WhsQYZZ() { ZZFJ = item, QYID = qyid };
                            addFiles.Add(qyzz);
                        }
                    }
                    //先删除后增加
                    DbSet<T_WhsQYZZ> db = hContext1.Set<T_WhsQYZZ>();
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
