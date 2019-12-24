using MedicalApparatusManage.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;

namespace MedicalApparatusManage.Domain
{
    public class T_PackingUnitDomain : BaseDomain<T_PackingUnit>
    {
        static T_PackingUnitDomain _instance;
        private T_PackingUnitDomain()
        {
        }
        //单例模式
        static public T_PackingUnitDomain GetInstance()
        {
            if (_instance == null)
            {
                _instance = new T_PackingUnitDomain();
            }
            return _instance;
        }
        public List<T_PackingUnit> PageT_PackingUnit(T_PackingUnit info, int pageIndex, int pageSize, out int pageCount, out int totalRecord)
        {
            Expression<Func<T_PackingUnit, bool>> where = PredicateBuilder.True<T_PackingUnit>();
            Func<T_PackingUnit, System.String> order = p => p.PUName;
            return base.GetPageInfo<System.String>(where, order, true, pageIndex, pageSize, out pageCount, out totalRecord);
        }
    }
}