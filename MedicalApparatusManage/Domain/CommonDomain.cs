using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MedicalApparatusManage.Domain
{
    public class CommonDomain
    {
        public static void ExecProc()
        {
            using (MedicalApparatusManageEntities hContext1 = new MedicalApparatusManageEntities())
            {
                try
                {
                    hContext1.proc_get_bjd();
                }
                catch (Exception ex)
                {
                }
            }
        }
    }
}