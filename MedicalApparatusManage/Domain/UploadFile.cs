using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MedicalApparatusManage.Domain
{
    /// <summary>
    /// 记录上传文件信息
    /// </summary>
    public partial class UploadFile
    {
        public string ContentType { get; set; }
        public int ContentLength { get; set; }
        /// <summary>
        /// 文件存放地址
        /// </summary>
        public string FileReference { get; set; }
        public string FileName { get; set; }
        public string RecordPerson { get; set; }
        public string RecordTime { get; set; }
    }
}
