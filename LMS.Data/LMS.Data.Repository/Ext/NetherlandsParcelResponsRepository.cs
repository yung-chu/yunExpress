using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LMS.Data.Context;
using LMS.Data.Entity;
using LighTake.Infrastructure.Common;

namespace LMS.Data.Repository
{
    public partial class NetherlandsParcelResponsRepository
    {
        public List<AgentNumberInfo> GetAgentNumbers(List<string> orderIds, string customerCode)
        {
            var ctx = this.UnitOfWork as LMS_DbContext;
            Check.Argument.IsNotNull(ctx, "数据库对象");
            Check.Argument.IsNullOrWhiteSpace(customerCode,"客户编码");
            var list = new List<AgentNumberInfo>();
            if (orderIds.Any())
            {
                list =
                    (from w in
                         ctx.WayBillInfos.Where(
                             p => p.CustomerCode == customerCode && orderIds.Contains(p.CustomerOrderNumber))
                     join n in ctx.NetherlandsParcelResponses on w.WayBillNumber equals n.WayBillNumber into g
                     from l in g.DefaultIfEmpty()
                     select new AgentNumberInfo
                         {
                             AgentNumber = l.MailNo,
                             OrderNumber = w.CustomerOrderNumber,
                             TrackingNumber = l.AgentMailNo
                         }).ToList();
            }
            return list;
        }
    }
}
