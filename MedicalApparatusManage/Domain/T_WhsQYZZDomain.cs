using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MedicalApparatusManage.Domain
{
    public class T_WhsQYZZDomain : BaseDomain<T_WhsQYZZ>
    {
        static T_WhsQYZZDomain _instance;
        private T_WhsQYZZDomain()
        {
        }
        static public T_WhsQYZZDomain GetInstance()
        {
            if (_instance == null)
            {
                _instance = new T_WhsQYZZDomain();
            }
            return _instance;
        }

        public List<T_WhsQYZZ> GetQYZZByQyid(int qyid)
        {
            List<T_WhsQYZZ> list = new List<T_WhsQYZZ>();
            using (MedicalApparatusManageEntities hContext1 = new MedicalApparatusManageEntities())
            {
                try
                {
                    list = hContext1.Set<T_WhsQYZZ>().Where(p => p.QYID == qyid).ToList();
                }
                catch (Exception)
                {

                }
            }
            return list;
        }
    }
}