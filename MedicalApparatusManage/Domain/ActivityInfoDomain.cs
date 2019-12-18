using MedicalApparatusManage.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;


namespace MedicalApparatusManage.Domain
{
    public class ActivityInfoDomain : BaseDomain<ActivityInfo>
    {
        static ActivityInfoDomain _instance;
        private ActivityInfoDomain()
        {
        }
        //单例模式
        static public ActivityInfoDomain GetInstance()
        {
            if (_instance == null)
            {
                _instance = new ActivityInfoDomain();
            }
            return _instance;
        }

        public List<ActivityInfo> PageActivityInfo(ActivityInfo info, DateTime? startTime, DateTime? endTime, int pageIndex, int pageSize, out int pageCount, out int totalRecord)
        {
            Expression<Func<ActivityInfo, bool>> where = PredicateBuilder.True<ActivityInfo>();
            Func<ActivityInfo, System.Int32> order = p => p.ID;
            return base.GetPageInfo<System.Int32>(where, order, true, pageIndex, pageSize, out pageCount, out totalRecord);
        }
    }
}
