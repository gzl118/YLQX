using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MedicalApparatusManage.Domain
{
    public class T_YLCPZZDomain : BaseDomain<T_YLCPZZ>
    {
        static T_YLCPZZDomain _instance;
        private T_YLCPZZDomain()
        {
        }
        static public T_YLCPZZDomain GetInstance()
        {
            if (_instance == null)
            {
                _instance = new T_YLCPZZDomain();
            }
            return _instance;
        }

        public List<T_YLCPZZ> GetCPZZByCpid(int cpid)
        {
            List<T_YLCPZZ> list = new List<T_YLCPZZ>();
            using (MedicalApparatusManageEntities hContext1 = new MedicalApparatusManageEntities())
            {
                try
                {
                    list = hContext1.Set<T_YLCPZZ>().Where(p => p.CPID == cpid).ToList();
                }
                catch (Exception)
                {

                }
            }
            return list;
        }
    }
}