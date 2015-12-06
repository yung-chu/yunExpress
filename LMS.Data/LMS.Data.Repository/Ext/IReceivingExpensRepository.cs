using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LMS.Data.Entity;
using LMS.Data.Entity.ExtModel;
using LighTake.Infrastructure.Common;

namespace LMS.Data.Repository
{
    public partial interface IReceivingExpensRepository
    {
        /// <summary>
        /// 收货费用明细
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        IPagedList<ReceivingExpenseExt> GetInFeeInfoExtPagedList(FinancialParam param);

        /// <summary>
        /// 收货费用
        /// Add By zhengsong
        /// Time:2014-06-30
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        IPagedList<InFeeInfoAuditListExt> GetAuditPagedList(InFeeInfoAuditParam param);

        IList<InFeeInfoAuditListExt> GetAuditList(InFeeInfoAuditParam param);

        IList<InFeeInfoAuditListExt> GetInFeeInfoExport(InFeeInfoAuditParam param);

        int GetInFeeInfoExportTotalCount(InFeeInfoAuditParam param);

        ReceivingExpensesEditExt GetReceivingExpensesEditEx(string wayBillNumber);

    }
}
