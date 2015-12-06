using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using LMS.Data.Context;
using LMS.Data.Entity;
using LMS.Data.Entity.ExtModel;
using LighTake.Infrastructure.Common;
using System.Linq.Dynamic;

namespace LMS.Data.Repository
{
    public partial class NoForecastAbnormalRepository
    {
        public IPagedList<NoForecastAbnormalExt> GetNoForecastAbnormalExtPagedList(NoForecastAbnormalParam param)
        {
            var ctx = this.UnitOfWork as LMS_DbContext;
            Check.Argument.IsNotNull(ctx, "数据库对象");
            var numberList = new List<string>();
            if (!string.IsNullOrWhiteSpace(param.SearchContext))
            {
                numberList =
                    param.SearchContext.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
                         .ToList();
            }

            Expression<Func<NoForecastAbnormal, bool>> filter = p => true;

            if (numberList.Any())
            {
                filter = filter.And(p => numberList.Contains(p.Number));

                //全部状态
                param.Status = 0;
            }
            else
            {
                filter = filter.AndIf(p => p.CustomerCode == param.CustomerCode, !string.IsNullOrWhiteSpace(param.CustomerCode))
                               .AndIf(p => p.ShippingMethodId == param.ShippingMethodId, param.ShippingMethodId.HasValue)
                               .AndIf(p => p.CreatedOn >= param.StartTime, param.StartTime.HasValue)
                               .AndIf(p => p.CreatedOn <= param.EndTime, param.EndTime.HasValue)
                               .AndIf(p => p.IsReturn, param.Status == (int) WayBill.NoForecastAbnormalEnum.NoForecastRetrun);
            }

            var query = from w in ctx.NoForecastAbnormals.Where(filter)
                         orderby w.CreatedOn
                         select new NoForecastAbnormalExt
                             {
                                 NoForecastAbnormalId = w.NoForecastAbnormalId,
                                 CustomerCode = w.CustomerCode,
                                 Number = w.Number,
                                 ShippingMethodId = w.ShippingMethodId,
                                 Weight = w.Weight,
                                 CreatedOn = w.CreatedOn,
                                 CreatedBy = w.CreatedBy,
                                 LastUpdatedOn = w.LastUpdatedOn,
                                 LastUpdatedBy = w.LastUpdatedBy,
                                 IsReturn = w.IsReturn,
                                 WayBillInfos = ctx.WayBillInfos.Where(p => (p.TrackingNumber == w.Number || p.CustomerOrderNumber == w.Number)&&p.CustomerCode==w.CustomerCode&&p.InShippingMethodID==w.ShippingMethodId)
                             };

            IQueryable<NoForecastAbnormalExt> list = query;

            //有预报
            if (param.Status == (int) WayBill.NoForecastAbnormalEnum.Forecasted)
            {
                list = from w in query
                       where w.WayBillInfos.Any(p => p.Status == (int)WayBill.StatusEnum.Submitted)
                       && w.IsReturn==false
                       select w;
            }
            //无预报
            else if (param.Status == (int) WayBill.NoForecastAbnormalEnum.NoForecast)
            {
                list = from w in query
                       where !w.WayBillInfos.Any(p => p.Status == (int)WayBill.StatusEnum.Submitted || p.InStorageCreatedOn.HasValue)
                       && w.IsReturn == false
                       select w;
            }
            else if (param.Status == (int)WayBill.NoForecastAbnormalEnum.NoForecastRetrun)
            {
                list = from w in query
                       where w.IsReturn
                       select w;
            }
            else if (param.Status==0)
            {
                list = from w in query
                       where w.IsReturn || (!w.WayBillInfos.Any(p => p.InStorageCreatedOn.HasValue && p.Status != (int)WayBill.StatusEnum.Delete && p.Status != (int)WayBill.StatusEnum.Return))
                       select w;
            }

            var pagedList = list.ToPagedList(param.Page, param.PageSize);
            pagedList.InnerList.ForEach(p =>
                {
                    var wayBillInfos = p.WayBillInfos.ToList();
                    if (wayBillInfos.Any(pp => pp.Status != (int)WayBill.StatusEnum.Delete && pp.Status != (int)WayBill.StatusEnum.Return))
                    {
                        p.Status = (int)WayBill.NoForecastAbnormalEnum.Forecasted;
                    }
                    else if (p.IsReturn)
                    {
                        p.Status = (int)WayBill.NoForecastAbnormalEnum.NoForecastRetrun;
                    }
                    else
                    {
                        p.Status = (int)WayBill.NoForecastAbnormalEnum.NoForecast;
                    }

                    if (p.Status == (int) WayBill.NoForecastAbnormalEnum.NoForecast)
                    {
                        if (wayBillInfos.All(pp => pp.Status != (int)WayBill.StatusEnum.Delete && pp.Status != (int)WayBill.StatusEnum.Return))
                        {
                            p.Description = "无预报异常";
                            //p.Description = "";
                        }
                        else if (wayBillInfos.Any(pp => pp.Status == (int)WayBill.StatusEnum.Delete) && wayBillInfos.All(pp => pp.Status != (int)WayBill.StatusEnum.Return))
                        {
                            p.Description = "无预报异常，存在已删除的单";
                        }
                        else if (wayBillInfos.Any(pp => pp.Status == (int)WayBill.StatusEnum.Return) && wayBillInfos.All(pp => pp.Status != (int)WayBill.StatusEnum.Delete))
                        {
                            p.Description = "无预报异常，存在已退回的单";
                        }
                        else if (wayBillInfos.Any(pp => pp.Status == (int)WayBill.StatusEnum.Return) && wayBillInfos.Any(pp => pp.Status == (int)WayBill.StatusEnum.Delete))
                        {
                            p.Description = "无预报异常，存在已删除和已退回的单";
                        }
                    }
                    else
                    {
                        p.Description = "";
                    }
                    

                });
            return pagedList;
        }

        public List<NoForecastAbnormalExt> GetNoForecastList(IEnumerable<int> noForecastAbnormalIds)
        {
            var ctx = this.UnitOfWork as LMS_DbContext;
            Check.Argument.IsNotNull(ctx, "数据库对象");

            var query = from w in ctx.NoForecastAbnormals
                        where noForecastAbnormalIds.Contains(w.NoForecastAbnormalId)
                        orderby w.CreatedOn
                        select new NoForecastAbnormalExt
                        {
                            NoForecastAbnormalId = w.NoForecastAbnormalId,
                            CustomerCode = w.CustomerCode,
                            Number = w.Number,
                            ShippingMethodId = w.ShippingMethodId,
                            Weight = w.Weight,
                            CreatedOn = w.CreatedOn,
                            CreatedBy = w.CreatedBy,
                            LastUpdatedOn = w.LastUpdatedOn,
                            LastUpdatedBy = w.LastUpdatedBy,
                            IsReturn = w.IsReturn,
                            WayBillInfos = ctx.WayBillInfos.Where(p => (p.TrackingNumber == w.Number || p.CustomerOrderNumber == w.Number) && p.CustomerCode == w.CustomerCode && p.InShippingMethodID == w.ShippingMethodId)
                        };

            var list = from w in query
                       where !w.WayBillInfos.Any(p => p.Status == (int)WayBill.StatusEnum.Submitted || p.InStorageCreatedOn.HasValue)
                       && w.IsReturn == false
                       select w;

            return list.ToList();
        }
    }
}
