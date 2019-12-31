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
            if (startTime != null)
            {
                where = where.And(p => p.CreateTime >= startTime.Value);
            }
            if (endTime != null)
            {
                where = where.And(p => p.CreateTime <= endTime.Value);
            }
            Func<ActivityInfo, System.DateTime?> order = p => p.CreateTime;
            return base.GetPageInfo<System.DateTime?>(where, order, true, pageIndex, pageSize, out pageCount, out totalRecord);
        }
    }
}
