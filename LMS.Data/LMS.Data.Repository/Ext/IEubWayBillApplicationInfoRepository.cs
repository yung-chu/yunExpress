using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Linq.Expressions;
using System.Text;
using LMS.Data.Context;
using LMS.Data.Entity;
using LighTake.Infrastructure.Common;

namespace LMS.Data.Repository
{
    public partial interface IEubWayBillApplicationInfoRepository
    {
        PagedList<EubWayBillApplicationInfoExt> GetEubWayBillList(EubWayBillApplicationInfoParam param, int maxCustomerOrderId = 0);
        List<EubWayBillApplicationInfo> GetEubWayBillInfoList(List<string> wayBillNumbers);
    }

    public partial class EubWayBillApplicationInfoRepository
    {
        public List<EubWayBillApplicationInfo> GetEubWayBillInfoList(List<string> wayBillNumbers)
        {
            var ctx = this.UnitOfWork as LMS_DbContext;
            Check.Argument.IsNotNull(ctx, "数据库对象");
            Check.Argument.IsNotNull(wayBillNumbers, "wayBillNumbers为空");

            var list = from e in ctx.EubWayBillApplicationInfos.Where(p => wayBillNumbers.Contains(p.WayBillNumber)) select e;

            return list.ToList();
        }
        public PagedList<EubWayBillApplicationInfoExt> GetEubWayBillList(EubWayBillApplicationInfoParam param, int maxCustomerOrderId = 0)
        {
            var ctx = this.UnitOfWork as LMS_DbContext;
            Check.Argument.IsNotNull(ctx, "数据库对象");

            int deleteSatus = CustomerOrder.StatusEnum.Delete.GetStatusValue();
            Expression<Func<CustomerOrderInfo, bool>> filter = o => o.Status != deleteSatus;
            filter = filter.AndIf(o => o.CustomerOrderID <= maxCustomerOrderId, maxCustomerOrderId > 0);
            Expression<Func<WayBillInfo, bool>> filterWayBill = o => true;
            Expression<Func<EubWayBillApplicationInfo, bool>> filterEub = o => true;
            bool isBathNumber = false;
            bool isfilterEub = false; 
            #region 条件
            if (!param.WayBillNumber.IsNullOrWhiteSpace())
            {
                var numberList = param.WayBillNumber.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
                         .ToList();

                if (numberList.Count > 0)
                {
                    string number = numberList[0];
                    filterWayBill = numberList.Count == 1 ? filterWayBill.And(a => a.WayBillNumber == number) : filterWayBill.And(a => numberList.Contains(a.WayBillNumber));
                }
            }
            else if (!param.TrackNumber.IsNullOrWhiteSpace())
            {
                  var trackList = param.TrackNumber.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
                         .ToList();
                  if (trackList.Count > 0)
                {
                    string number = trackList[0];
                    filterWayBill = trackList.Count == 1
                                        ? filterWayBill.And(a => a.TrackingNumber == number)
                                        : filterWayBill.And(a => trackList.Contains(a.TrackingNumber));
                }
            }
            else if (!param.CustomerOrderNumber.IsNullOrWhiteSpace())
            {
                var numberList =
                    param.CustomerOrderNumber.Split(Environment.NewLine.ToCharArray(),
                                                    StringSplitOptions.RemoveEmptyEntries)
                         .ToList();

                if (numberList.Count > 0)
                {
                    string number = numberList[0];
                    filter = numberList.Count == 1
                                 ? filter.And(o => o.CustomerOrderNumber == number)
                                 : filter.And(o => numberList.Contains(o.CustomerOrderNumber));
                }
            }
            else if (!param.BatchNumber.IsNullOrWhiteSpace())
            {
                var batchList =
                  param.BatchNumber.Split(Environment.NewLine.ToCharArray(),
                                                  StringSplitOptions.RemoveEmptyEntries)
                       .ToList();

                if (batchList.Count > 0)
                {
                    string number = batchList[0];
                    filterEub = batchList.Count == 1
                                 ? filterEub.And(o => o.BatchNumber == number)
                                 : filterEub.And(o => batchList.Contains(o.BatchNumber));
                    isfilterEub = true;
                }
            }
            #endregion
            else
            {
                if (param.Status.HasValue)
                {
                    filterEub = filterEub.AndIf(o => o.Status == param.Status, param.Status.Value != 1);
                    if (param.Status.Value != 1)
                    {
                        isfilterEub = true;
                    }
                    else
                    {
                        isBathNumber = true;
                    }
                }
                
                filterWayBill = filterWayBill.AndIf(o => o.CountryCode == param.CountryCode,
                                              !param.CountryCode.IsNullOrWhiteSpace());
                filter = filter.AndIf(o => o.CustomerCode == param.CustomerCode, !param.CustomerCode.IsNullOrWhiteSpace());
                if (param.TimeType == (int) EubWayBillApplicationInfo.TimeQueryEnum.ApplyTime)
                {
                    filterEub = filterEub.AndIf(o => o.CreatedOn >= param.CreatedOnFrom.Value, param.CreatedOnFrom.HasValue)
                               .AndIf(o => o.CreatedOn <= param.CreatedOnTo.Value, param.CreatedOnTo.HasValue);
                    if (param.CreatedOnFrom.HasValue || param.CreatedOnTo.HasValue)
                    {
                        isfilterEub = true;
                    }
                }
                else
                {
                    filterWayBill = filterWayBill.AndIf(o => o.CreatedOn >= param.CreatedOnFrom.Value,
                                                       param.CreatedOnFrom.HasValue)
                                                .AndIf(o => o.CreatedOn <= param.CreatedOnTo.Value,
                                                       param.CreatedOnTo.HasValue);
                }
            }

            filterWayBill = filterWayBill.AndIf(o => o.InShippingMethodID == param.ShippingMethodId, param.ShippingMethodId.HasValue);
            filterWayBill = filterWayBill.AndIf(o => param.ShippingMethods.Contains(o.InShippingMethodID.Value)
                                   , !param.ShippingMethodId.HasValue);

            if (!isfilterEub)
            {
                if (!isBathNumber)
                {
                    var list = (from w in ctx.WayBillInfos.Where(filterWayBill)
                                join o in ctx.CustomerOrderInfos.Where(filter) on w.CustomerOrderID equals
                                    o.CustomerOrderID
                                orderby w.WayBillNumber descending
                                select new EubWayBillApplicationInfoExt
                                    {
                                        EubOrderId = null,
                                        CustomerOrderNumber = o.CustomerOrderNumber,
                                        WayBillNumber = w.WayBillNumber,
                                        TrackingNumber = w.TrackingNumber,
                                        BatchNumber = "",
                                        PrintFormat = null,
                                        LocalDownLoad = "",
                                        EubDownLoad = "",
                                        ApplyDate = null,
                                        Status = null,
                                        CountryCode = w.CountryCode,
                                        ShippingMethodID = w.InShippingMethodID ?? 0,
                                        ShippingMethodName = w.InShippingMethodName,
                                        CreatedOn = w.CreatedOn
                                    }).ToPagedList(param.Page, param.PageSize);
                    if (list.InnerList.Any())
                    {
                        var waybillNumbers = list.InnerList.Select(o => o.WayBillNumber).ToList();
                        var eublist =
                            ctx.EubWayBillApplicationInfos.Where(
                                p => waybillNumbers.Contains(p.WayBillNumber)).ToList();
                        if (eublist.Any())
                        {
                            var eubdiction = new Dictionary<string, EubWayBillApplicationInfo>();
                            eublist.Each(p => eubdiction.Add(p.WayBillNumber, p));
                            foreach (var infoExt in list)
                            {
                                if (eubdiction.ContainsKey(infoExt.WayBillNumber))
                                {
                                    infoExt.EubOrderId = eubdiction[infoExt.WayBillNumber].EubOrderId;
                                    infoExt.BatchNumber = eubdiction[infoExt.WayBillNumber].BatchNumber;
                                    infoExt.PrintFormat = eubdiction[infoExt.WayBillNumber].PrintFormat;
                                    infoExt.LocalDownLoad = eubdiction[infoExt.WayBillNumber].LocalDownLoad;
                                    infoExt.EubDownLoad = eubdiction[infoExt.WayBillNumber].EubDownLoad;
                                    infoExt.ApplyDate = eubdiction[infoExt.WayBillNumber].CreatedOn;
                                    infoExt.Status = eubdiction[infoExt.WayBillNumber].Status;
                                }
                            }
                        }
                    }

                    return list;
                }
                else
                {
                    var list = (from w in ctx.WayBillInfos.Where(filterWayBill)
                                join o in ctx.CustomerOrderInfos.Where(filter) on w.CustomerOrderID equals
                                    o.CustomerOrderID
                                where !(from e in ctx.EubWayBillApplicationInfos select e.WayBillNumber).Contains(w.WayBillNumber)
                                orderby w.WayBillNumber descending
                                select new EubWayBillApplicationInfoExt
                                {
                                    EubOrderId = null,
                                    CustomerOrderNumber = o.CustomerOrderNumber,
                                    WayBillNumber = w.WayBillNumber,
                                    TrackingNumber = w.TrackingNumber,
                                    BatchNumber = "",
                                    PrintFormat = null,
                                    LocalDownLoad = "",
                                    EubDownLoad = "",
                                    ApplyDate = null,
                                    Status = null,
                                    CountryCode = w.CountryCode,
                                    ShippingMethodID = w.InShippingMethodID ?? 0,
                                    ShippingMethodName = w.InShippingMethodName,
                                    CreatedOn = w.CreatedOn
                                }).ToPagedList(param.Page, param.PageSize);
                    return list;
                }
            }
            else
            {
                var list = (from w in ctx.WayBillInfos.Where(filterWayBill)
                            join o in ctx.CustomerOrderInfos.Where(filter) on w.CustomerOrderID equals o.CustomerOrderID
                            join e in ctx.EubWayBillApplicationInfos.Where(filterEub) on w.WayBillNumber equals
                                e.WayBillNumber
                            orderby e.EubOrderId descending
                            select new EubWayBillApplicationInfoExt
                            {
                                EubOrderId = e.EubOrderId,
                                CustomerOrderNumber = o.CustomerOrderNumber,
                                WayBillNumber = w.WayBillNumber,
                                TrackingNumber = w.TrackingNumber,
                                BatchNumber = e.BatchNumber,
                                PrintFormat = e.PrintFormat,
                                LocalDownLoad = e.LocalDownLoad,
                                EubDownLoad = e.EubDownLoad,
                                ApplyDate = w.CreatedOn,
                                Status = e.Status,
                                CountryCode = w.CountryCode,
                                ShippingMethodID = w.InShippingMethodID ?? 0,
                                ShippingMethodName = w.InShippingMethodName,
                                CreatedOn = o.CreatedOn
                            }).ToPagedList(param.Page, param.PageSize);
                return list;
            }

            //if (!isBathNumber&&(param.Status == null || param.Status.Value == 1))
            //{
            //    var list = from w in ctx.WayBillInfos.Where(filterWayBill)
            //               join o in ctx.CustomerOrderInfos.Where(filter) on w.CustomerOrderID equals o.CustomerOrderID
            //               join e in ctx.EubWayBillApplicationInfos.Where(filterEub) on w.WayBillNumber equals
            //                   e.WayBillNumber into temp
            //               from t in temp.DefaultIfEmpty()
            //               orderby w.CreatedOn descending
            //               select new EubWayBillApplicationInfoExt
            //               {
            //                   EubOrderId = t.EubOrderId,
            //                   CustomerOrderNumber = o.CustomerOrderNumber,
            //                   WayBillNumber = w.WayBillNumber,
            //                   TrackingNumber = w.TrackingNumber,
            //                   BatchNumber = t.BatchNumber,
            //                   PrintFormat = t.PrintFormat,
            //                   LocalDownLoad = t.LocalDownLoad,
            //                   EubDownLoad = t.EubDownLoad,
            //                   ApplyDate = t.CreatedOn,
            //                   Status = t.Status,
            //                   CountryCode = w.CountryCode,
            //                   ShippingMethodID = w.InShippingMethodID ?? 0,
            //                   ShippingMethodName = w.InShippingMethodName,
            //                   CreatedOn = w.CreatedOn
            //               };
            //    var list1 = list.WhereIf(p => p.Status.Equals(null), param.Status != null);
            //    return list1.ToPagedList(param.Page, param.PageSize);

            //}
            //var lists = from w in ctx.WayBillInfos.Where(filterWayBill)
            //            join o in ctx.CustomerOrderInfos.Where(filter) on w.CustomerOrderID equals o.CustomerOrderID
            //            join e in ctx.EubWayBillApplicationInfos.Where(filterEub) on w.WayBillNumber equals
            //                e.WayBillNumber
            //            orderby e.CreatedOn descending
            //            select new EubWayBillApplicationInfoExt
            //                {
            //                    EubOrderId = e.EubOrderId,
            //                    CustomerOrderNumber = o.CustomerOrderNumber,
            //                    WayBillNumber = w.WayBillNumber,
            //                    TrackingNumber = w.TrackingNumber,
            //                    BatchNumber = e.BatchNumber,
            //                    PrintFormat = e.PrintFormat,
            //                    LocalDownLoad = e.LocalDownLoad,
            //                    EubDownLoad = e.EubDownLoad,
            //                    ApplyDate = w.CreatedOn,
            //                    Status = e.Status,
            //                    CountryCode = w.CountryCode,
            //                    ShippingMethodID = w.InShippingMethodID ?? 0,
            //                    ShippingMethodName = w.InShippingMethodName,
            //                    CreatedOn = o.CreatedOn
            //                };

            //return lists.ToPagedList(param.Page, param.PageSize);


        }
    }
}
