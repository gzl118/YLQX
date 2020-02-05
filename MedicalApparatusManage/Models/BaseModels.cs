using System;
using System.Collections.Generic;

namespace MedicalApparatusManage.Models
{
    public class BaseModels<T> where T : class
    {

        public virtual string Tag { get; set; }

        public virtual T DataModel { get; set; }

        public virtual List<T> DataList { get; set; }

        public virtual List<T_CGMX> CGMXList { get; set; }

        public virtual List<T_XSMX> XSMXList { get; set; }

        public virtual List<T_QYZZ> QYZZList { get; set; }

        public virtual List<T_CusQYZZ> CusQYZZList { get; set; }

        public virtual T_CGD CGDmodel { get; set; }

        private DateTime? startTime;

        public virtual DateTime? StartTime
        {
            get { return startTime; }
            set { startTime = value; }
        }

        private DateTime? endTime;

        public virtual DateTime? EndTime
        {
            get { return endTime; }
            set { endTime = value; }
        }

        private System.Nullable<int> _pageSize = 20;
        public System.Nullable<int> pageSize
        {
            get
            {
                return _pageSize;
            }
            set { _pageSize = value; }
        }
        private System.Nullable<int> _pagecount = 5;
        public System.Nullable<int> pagecount
        {
            get
            {
                return _pagecount;
            }
            set { _pagecount = value; }
        }
        private System.Nullable<int> _currentPage = 1;
        public System.Nullable<int> currentPage
        {
            get
            {
                return _currentPage;
            }
            set { _currentPage = value; }
        }
        private System.Nullable<int> _resultCount = null;
        public System.Nullable<int> resultCount
        {
            get
            {
                return _resultCount;
            }
            set { _resultCount = value; }
        }
        private string _SortCol = "";
        public string SortCol
        {
            get
            {
                return _SortCol;
            }
            set { _SortCol = value; }
        }
        private string _SortKey = "";
        public string SortKey
        {
            get
            {
                return _SortKey;
            }
            set { _SortKey = value; }
        }
        private string _AppCon = "/";
        public string AppCon
        {
            get
            {
                return _AppCon;
            }
            set { _AppCon = value; }
        }
        private string _RoleID = "";

        public string RoleID
        {
            get { return _RoleID; }
            set { _RoleID = value; }
        }
        /// <summary>
        /// 角色Code,1超级管理员，2部门领导，3普通职员
        /// </summary>
        public string RoleCode { get; set; }
     
    }
}