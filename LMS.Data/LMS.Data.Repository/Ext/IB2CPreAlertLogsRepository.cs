using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LMS.Data.Entity;
using LighTake.Infrastructure.Common;

namespace LMS.Data.Repository
{
    public partial interface IB2CPreAlertLogsRepository
    {
        /// <summary>
        /// 查询B2C预报信息列表
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        PagedList<B2CPreAlterExt> GetB2CPreAlterExtList(B2CPreAlterListParam param);

        bool PreAlterB2CBySearch(B2CPreAlterListParam param);
        bool PreAlterB2CByWayBillNumber(List<string> wayBillNumbers);
    }
}
