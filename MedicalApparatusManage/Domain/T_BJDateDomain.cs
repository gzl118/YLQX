using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MedicalApparatusManage.Domain
{
    public class T_BJDateDomain : BaseDomain<T_BJDate>
    {
        static T_BJDateDomain _instance;
        private T_BJDateDomain()
        {
        }
        //单例模式
        static public T_BJDateDomain GetInstance()
        {
            if (_instance == null)
            {
                _instance = new T_BJDateDomain();
            }
            return _instance;
        }

        public T_BJDate GetFirstBJDate()
        {
            T_BJDate model = new T_BJDate();
            using (MedicalApparatusManageEntities hContext1 = new MedicalApparatusManageEntities())
            {
                try
                {
                    model = hContext1.Set<T_BJDate>().FirstOrDefault() ?? new T_BJDate();
                }
                catch (Exception)
                {
                }
            }
            return model;
        }
        public int UpdateList(List<T_BJDate> list)
        {
            var result = 0;
            using (MedicalApparatusManageEntities hContext1 = new MedicalApparatusManageEntities())
            {
                try
                {
                    foreach (var item in list)
                    {
                        var temp = hContext1.Set<T_BJDate>().Find(item.ID);
                        if (temp != null)
                        {
                            temp.BJDATE = item.BJDATE;
                        }
                    }
                    hContext1.SaveChanges();
                    result = 1;
                }
                catch (Exception)
                {
                }
            }

            return result;
        }
    }
}