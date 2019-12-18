using MedicalApparatusManage.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;


namespace MedicalApparatusManage.Domain
{
    public class SysFavoriteDomain : BaseDomain<SysFavorite>
    {
        static SysFavoriteDomain _instance;
        private SysFavoriteDomain()
        {
        }
        //单例模式
        static public SysFavoriteDomain GetInstance()
        {
            if (_instance == null)
            {
                _instance = new SysFavoriteDomain();
            }
            return _instance;
        }

        public List<SysFavorite> PageSysFavorite(SysFavorite info, DateTime? startTime, DateTime? endTime, int pageIndex, int pageSize, out int pageCount, out int totalRecord)
        {
            Expression<Func<SysFavorite, bool>> where = PredicateBuilder.True<SysFavorite>();
            Func<SysFavorite, System.Int32> order = p => p.FavoriteID;
            return base.GetPageInfo<System.Int32>(where, order, true, pageIndex, pageSize, out pageCount, out totalRecord);
        }

        /// <summary>
        /// 获取快捷菜单列表
        /// </summary>
        /// <param name="用户ID"></param>
        /// <returns></returns>
        public List<SysFavorite> GetFavoriteList(int userid)
        {
            List<SysFavorite> list = new List<SysFavorite>();
            string sql = string.Format(@"SELECT * FROM SysFavorite WHERE UserID = {0}", userid);
            using (MedicalApparatusManageEntities gContext = new MedicalApparatusManageEntities())
            {
                try
                {
                    list = gContext.Database.SqlQuery<SysFavorite>(sql).ToList();
                }
                catch (Exception)
                {

                }
            }
            return list;
        }
    }
}
