using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LMS.Data.Entity;

namespace LMS.Data.Repository
{
    public partial interface IFuzhouPostLogRepository
    {
        /// <summary>
        /// 更申请记录
        ///  Add By zhengsong
        /// Time"2014-11-04
        /// </summary>
        void bulkInsertFuzhouPostLog(List<FuzhouPostLog> addfuzhouPostLogs);
    }
}
