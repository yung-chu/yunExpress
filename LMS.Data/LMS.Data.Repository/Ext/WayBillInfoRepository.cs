using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Dynamic;
using System.Linq.Expressions;
using System.Text;
using LMS.Core;
using LMS.Data.Context;
using LMS.Data.Entity;
using LMS.Data.Entity.ExtModel;
using LighTake.Infrastructure.Common;

namespace LMS.Data.Repository
{
    public partial class WayBillInfoRepository
    {
        public IPagedList<InFeeInfoExt> GetInFeeInfoExtPagedList(InFeeListParam param, out decimal totalFee)
        {

            var ctx = this.UnitOfWork as LMS_DbContext;
            Check.Argument.IsNotNull(ctx, "数据库对象");
            var numberlist = new List<string>();
            totalFee = 0;
            var list = string.Empty;
            if (!string.IsNullOrWhiteSpace(param.SearchContext))
            {
                numberlist =
                    param.SearchContext.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
                         .ToList();
                list = string.Join(",", numberlist.Distinct());
            }
            var CustomerCode = new SqlParameter
                {
                    ParameterName = "CustomerCode",
                    Value = param.CustomerCode,
                    DbType = DbType.String
                };
            var ShippingMethodId = new SqlParameter
                {
                    ParameterName = "ShippingMethodId",
                    Value = param.ShippingMethodId,
                    DbType = DbType.Int32
                };
            var StartTime = new SqlParameter
                {
                    ParameterName = "StartTime",
                    Value = param.StartTime,
                    DbType = DbType.Time
                };
            var EndTime = new SqlParameter {ParameterName = "EndTime", Value = param.EndTime, DbType = DbType.Time};
            var CountryCode = new SqlParameter
                {
                    ParameterName = "CountryCode",
                    Value = param.CountryCode,
                    DbType = DbType.String
                };
            var SearchWhere = new SqlParameter
                {
                    ParameterName = "SearchWhere",
                    Value = param.SearchWhere,
                    DbType = DbType.Int32
                };
            var SearchContext = new SqlParameter {ParameterName = "SearchContext", Value = list, DbType = DbType.String};
            var TotalRecord = new SqlParameter
                {
                    ParameterName = "TotalRecord",
                    Value = 0,
                    DbType = DbType.Int32,
                    Direction = ParameterDirection.Output
                };
            var PageSize = new SqlParameter {ParameterName = "PageSize", Value = param.PageSize, DbType = DbType.Int32};
            var PageIndex = new SqlParameter {ParameterName = "PageIndex", Value = param.Page, DbType = DbType.Int32};
            var TotalPage = new SqlParameter
                {
                    ParameterName = "TotalPage",
                    Value = 0,
                    DbType = DbType.Int32,
                    Direction = ParameterDirection.Output
                };
            var TotalFee = new SqlParameter
                {
                    ParameterName = "TotalFee",
                    Value = 0.00,
                    DbType = DbType.Decimal,
                    Direction = ParameterDirection.Output,
                    Precision = 18,
                    Scale = 2
                };
            if (ctx != null)
            {
                var obj = ctx.ExecuteStoredProcedureList<InFeeInfoExt>("P_GetInFeeInfoPageList"
                                                                       , CustomerCode, ShippingMethodId, StartTime,
                                                                       EndTime, CountryCode, SearchWhere, SearchContext,
                                                                       TotalRecord, PageSize, PageIndex, TotalPage,
                                                                       TotalFee);
                if (obj != null && obj.Count > 0)
                {
                    totalFee = decimal.Parse(TotalFee.Value.ToString());
                    return new PagedList<InFeeInfoExt>()
                        {
                            InnerList = obj.ToList(),
                            PageIndex = param.Page,
                            PageSize = param.PageSize,
                            TotalCount = Int32.Parse(TotalRecord.Value.ToString()),
                            TotalPages = Int32.Parse(TotalPage.Value.ToString())
                        };
                }
            }
            return new PagedList<InFeeInfoExt>()
                {
                    InnerList = null,
                    PageIndex = param.Page,
                    PageSize = param.PageSize,
                    TotalCount = 0,
                    TotalPages = 0
                };
        }


        public IList<InFeeInfoExt> GetInFeeInfoExtList(InFeeListParam param, out decimal totalFee)
        {
            var ctx = this.UnitOfWork as LMS_DbContext;
            Check.Argument.IsNotNull(ctx, "数据库对象");
            var numberlist = new List<string>();
            totalFee = 0;
            var list = string.Empty;
            if (!string.IsNullOrWhiteSpace(param.SearchContext))
            {
                numberlist =
                    param.SearchContext.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
                         .ToList();
                list = string.Join(",", numberlist.Distinct());
            }
            var CustomerCode = new SqlParameter
                {
                    ParameterName = "CustomerCode",
                    Value = param.CustomerCode,
                    DbType = DbType.String
                };
            var ShippingMethodId = new SqlParameter
                {
                    ParameterName = "ShippingMethodId",
                    Value = param.ShippingMethodId,
                    DbType = DbType.Int32
                };
            var StartTime = new SqlParameter
                {
                    ParameterName = "StartTime",
                    Value = param.StartTime,
                    DbType = DbType.Time
                };
            var EndTime = new SqlParameter {ParameterName = "EndTime", Value = param.EndTime, DbType = DbType.Time};
            var CountryCode = new SqlParameter
                {
                    ParameterName = "CountryCode",
                    Value = param.CountryCode,
                    DbType = DbType.String
                };
            var SearchWhere = new SqlParameter
                {
                    ParameterName = "SearchWhere",
                    Value = param.SearchWhere,
                    DbType = DbType.Int32
                };
            var SearchContext = new SqlParameter {ParameterName = "SearchContext", Value = list, DbType = DbType.String};
            var TotalFee = new SqlParameter
                {
                    ParameterName = "TotalFee",
                    Value = 0.00,
                    DbType = DbType.Decimal,
                    Direction = ParameterDirection.Output,
                    Precision = 18,
                    Scale = 2
                };
            if (ctx != null)
            {
                var obj = ctx.ExecuteStoredProcedureList<InFeeInfoExt>("P_GetInFeeInfo"
                                                                       , CustomerCode, ShippingMethodId, StartTime,
                                                                       EndTime, CountryCode, SearchWhere, SearchContext,
                                                                       TotalFee);
                if (obj != null && obj.Count > 0)
                {
                    totalFee = decimal.Parse(TotalFee.Value.ToString());
                    return obj;
                }
            }
            return null;
        }

        public IList<InFeeTotalInfoExt> GetInFeeTotalInfoExtList(InFeeTotalListParam param, out decimal totalFee)
        {
            var ctx = this.UnitOfWork as LMS_DbContext;
            Check.Argument.IsNotNull(ctx, "数据库对象");
            totalFee = 0;
            var number = new SqlParameter {ParameterName = "Number", Value = param.Number, DbType = DbType.String};
            //var customerCode = new SqlParameter { ParameterName = "CustomerCode", Value = param.CustomerCode, DbType = DbType.String };
            var TotalFee = new SqlParameter
                {
                    ParameterName = "TotalFee",
                    Value = 0.00,
                    DbType = DbType.Decimal,
                    Direction = ParameterDirection.Output,
                    Precision = 18,
                    Scale = 2
                };
            if (ctx != null)
            {
                var obj = ctx.ExecuteStoredProcedureList<InFeeTotalInfoExt>("P_GetInFeeTotalInfo", number, TotalFee);
                if (obj != null && obj.Count > 0)
                {
                    totalFee = decimal.Parse(TotalFee.Value.ToString());
                    return obj;
                }
            }
            return null;
        }



        public IPagedList<OutFeeInfoExt> GetOutFeeInfoExtPagedList(OutFeeListParam param, out decimal totalFee)
        {
            var ctx = this.UnitOfWork as LMS_DbContext;
            Check.Argument.IsNotNull(ctx, "数据库对象");
            var numberlist = new List<string>();
            totalFee = 0;
            var list = string.Empty;
            if (!string.IsNullOrWhiteSpace(param.SearchContext))
            {
                numberlist =
                    param.SearchContext.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
                         .ToList();
                list = string.Join(",", numberlist.Distinct());
            }
            var VenderCode = new SqlParameter
                {
                    ParameterName = "VenderCode",
                    Value = param.VenderCode,
                    DbType = DbType.String
                };
            var ShippingMethodId = new SqlParameter
                {
                    ParameterName = "ShippingMethodId",
                    Value = param.ShippingMethodId,
                    DbType = DbType.Int32
                };
            var StartTime = new SqlParameter
                {
                    ParameterName = "StartTime",
                    Value = param.StartTime,
                    DbType = DbType.Time
                };
            var EndTime = new SqlParameter {ParameterName = "EndTime", Value = param.EndTime, DbType = DbType.Time};
            var CountryCode = new SqlParameter
                {
                    ParameterName = "CountryCode",
                    Value = param.CountryCode,
                    DbType = DbType.String
                };
            var SearchWhere = new SqlParameter
                {
                    ParameterName = "SearchWhere",
                    Value = param.SearchWhere,
                    DbType = DbType.Int32
                };
            var SearchContext = new SqlParameter {ParameterName = "SearchContext", Value = list, DbType = DbType.String};
            var TotalRecord = new SqlParameter
                {
                    ParameterName = "TotalRecord",
                    Value = 0,
                    DbType = DbType.Int32,
                    Direction = ParameterDirection.Output
                };
            var PageSize = new SqlParameter {ParameterName = "PageSize", Value = param.PageSize, DbType = DbType.Int32};
            var PageIndex = new SqlParameter {ParameterName = "PageIndex", Value = param.Page, DbType = DbType.Int32};
            var TotalPage = new SqlParameter
                {
                    ParameterName = "TotalPage",
                    Value = 0,
                    DbType = DbType.Int32,
                    Direction = ParameterDirection.Output
                };
            var TotalFee = new SqlParameter
                {
                    ParameterName = "TotalFee",
                    Value = 0,
                    DbType = DbType.Decimal,
                    Direction = ParameterDirection.Output,
                    Precision = 18,
                    Scale = 2
                };
            if (ctx != null)
            {
                var obj = ctx.ExecuteStoredProcedureList<OutFeeInfoExt>("P_GetOutFeeInfoPageList"
                                                                        , VenderCode, ShippingMethodId, StartTime,
                                                                        EndTime, CountryCode, SearchWhere, SearchContext,
                                                                        TotalRecord, PageSize, PageIndex, TotalPage,
                                                                        TotalFee);
                if (obj != null && obj.Count > 0)
                {
                    totalFee = decimal.Parse(TotalFee.Value.ToString());
                    return new PagedList<OutFeeInfoExt>()
                        {
                            InnerList = obj.ToList(),
                            PageIndex = param.Page,
                            PageSize = param.PageSize,
                            TotalCount = Int32.Parse(TotalRecord.Value.ToString()),
                            TotalPages = Int32.Parse(TotalPage.Value.ToString())
                        };
                }
            }
            return new PagedList<OutFeeInfoExt>()
                {
                    InnerList = null,
                    PageIndex = param.Page,
                    PageSize = param.PageSize,
                    TotalCount = 0,
                    TotalPages = 0
                };

        }


        public IList<OutFeeInfoExt> GetOutFeeInfoExtList(OutFeeListParam param, out decimal totalFee)
        {
            var ctx = this.UnitOfWork as LMS_DbContext;
            Check.Argument.IsNotNull(ctx, "数据库对象");
            var numberlist = new List<string>();
            totalFee = 0;
            var list = string.Empty;
            if (!string.IsNullOrWhiteSpace(param.SearchContext))
            {
                numberlist =
                    param.SearchContext.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
                         .ToList();
                list = string.Join(",", numberlist.Distinct());
            }
            var VenderCode = new SqlParameter
                {
                    ParameterName = "VenderCode",
                    Value = param.VenderCode,
                    DbType = DbType.String
                };
            var ShippingMethodId = new SqlParameter
                {
                    ParameterName = "ShippingMethodId",
                    Value = param.ShippingMethodId,
                    DbType = DbType.Int32
                };
            var StartTime = new SqlParameter
                {
                    ParameterName = "StartTime",
                    Value = param.StartTime,
                    DbType = DbType.Time
                };
            var EndTime = new SqlParameter {ParameterName = "EndTime", Value = param.EndTime, DbType = DbType.Time};
            var CountryCode = new SqlParameter
                {
                    ParameterName = "CountryCode",
                    Value = param.CountryCode,
                    DbType = DbType.String
                };
            var SearchWhere = new SqlParameter
                {
                    ParameterName = "SearchWhere",
                    Value = param.SearchWhere,
                    DbType = DbType.Int32
                };
            var SearchContext = new SqlParameter {ParameterName = "SearchContext", Value = list, DbType = DbType.String};
            var TotalFee = new SqlParameter
                {
                    ParameterName = "TotalFee",
                    Value = 0,
                    DbType = DbType.Decimal,
                    Direction = ParameterDirection.Output,
                    Precision = 18,
                    Scale = 2
                };
            if (ctx != null)
            {

                var obj = ctx.ExecuteStoredProcedureList<OutFeeInfoExt>("P_GetOutFeeInfo"
                                                                        , VenderCode, ShippingMethodId, StartTime,
                                                                        EndTime, CountryCode, SearchWhere, SearchContext,
                                                                        TotalFee);
                if (obj != null && obj.Count > 0)
                {
                    totalFee = decimal.Parse(TotalFee.Value.ToString());
                    return obj;

                }
            }
            return null;


        }

        public IList<ExportOutStorageInfo> GetExportOutStorageInfo(OutStorageListParam param)
        {
            var ctx = this.UnitOfWork as LMS_DbContext;
            Check.Argument.IsNotNull(ctx, "数据库对象");
            var VenderCode = new SqlParameter
                {
                    ParameterName = "VenderCode",
                    Value = param.VenderCode,
                    DbType = DbType.String
                };
            var OutStorageID = new SqlParameter
                {
                    ParameterName = "OutStorageID",
                    Value = param.OutStorageID,
                    DbType = DbType.String
                };
            var StartTime = new SqlParameter
                {
                    ParameterName = "StartCreatedOn",
                    Value = param.StartTime ?? new DateTime(1900, 1, 1),
                    DbType = DbType.Time
                };
            var EndTime = new SqlParameter
                {
                    ParameterName = "EndCreatedOn",
                    Value = param.EndTime ?? DateTime.Now,
                    DbType = DbType.Time
                };
            var Total = new SqlParameter
                {
                    ParameterName = "Total",
                    Value = 0,
                    DbType = DbType.Int32,
                    Direction = ParameterDirection.Output
                };
            SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("@OutStorageID", param.OutStorageID),
                    new SqlParameter("@VenderCode", param.VenderCode),
                    new SqlParameter("@StartCreatedOn", param.StartTime ?? new DateTime(1900, 1, 1)),
                    new SqlParameter("@EndCreatedOn", param.EndTime ?? DateTime.Now)

                };
            if (ctx != null)
            {
                //var obj = ctx.Database.SqlQuery<ExportOutStorageInfo>("exec  P_ExportOutStorageInfo  @OutStorageID,@VenderCode,@StartCreatedOn,@EndCreatedOn", parameters).ToList();
                var obj = ctx.ExecuteStoredProcedureList<ExportOutStorageInfo>("P_ExportOutStorageInfo", OutStorageID,
                                                                               VenderCode, StartTime, EndTime, Total);
                if (obj.Count > 0)
                {
                    return obj;
                }
            }
            return null;
        }

        public IList<PrintInStorageInvoiceExt> GetPrintInStorageInvoice(PrintInStorageInvoiceParam param)
        {
            var ctx = this.UnitOfWork as LMS_DbContext;
            Check.Argument.IsNotNull(ctx, "数据库对象");
            var InStorageID = new SqlParameter
                {
                    ParameterName = "InStorageID",
                    Value = param.InStorageID,
                    DbType = DbType.String
                };
            SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("@InStorageID", param.InStorageID),
                };
            if (ctx != null)
            {
                var obj =
                    ctx.Database.SqlQuery<PrintInStorageInvoiceExt>("exec  P_PrintInStorageInvoice  @InStorageID",
                                                                    parameters).ToList();
                //var obj = ctx.ExecuteStoredProcedureList<PrintInStorageInvoiceExt>("P_PrintInStorageInvoice", InStorageID);
                if (obj.Count > 0)
                {
                    return obj;
                }
            }
            return null;
        }

        public bool UpdateOutStoragePrice(List<string> wayBillNumbers)
        {
            var ctx = this.UnitOfWork as LMS_DbContext;
            Check.Argument.IsNotNull(ctx, "数据库对象");
            int isSuccess = 0;
            var list = string.Join(",", wayBillNumbers);
            var wayBillNumber = new SqlParameter
                {
                    ParameterName = "wayBillNumbers",
                    Value = list,
                    DbType = DbType.String
                };
            var isReturn = new SqlParameter
                {
                    ParameterName = "isReturn",
                    Value = 0,
                    DbType = DbType.Int32,
                    Direction = ParameterDirection.Output
                };
            if (ctx != null)
            {
                ctx.ExecuteCommand(
                    "Exec P_UpdateOutStoragePrice @wayBillNumbers,@isReturn output",
                    wayBillNumber, isReturn);
                Int32.TryParse(isReturn.Value.ToString(), out isSuccess);
            }
            return isSuccess == 1;
        }

        public List<int> GetCustomerId(List<string> wayBillNumbers)
        {
            var ctx = this.UnitOfWork as LMS_DbContext;
            Check.Argument.IsNotNull(ctx, "数据库对象");
            List<int> customerIds = new List<int>();
            foreach (var row in wayBillNumbers)
            {
                string wayBillNumber = row;
                var list = (from w in ctx.WayBillInfos
                            where w.WayBillNumber == wayBillNumber
                            select new
                                {
                                    customerId = w.CustomerOrderID
                                }).FirstOrDefault();
                if (list != null)
                {
                    customerIds.Add((list.customerId == null ? 0 : list.customerId.Value));
                }
            }
            return customerIds;
        }

        public IList<WayBillInfo> GetWayBillList(string[] wayBillNumber, string customerCode)
        {
            var ctx = this.UnitOfWork as LMS_DbContext;
            Check.Argument.IsNotNull(ctx, "数据库对象");
            int deleteSatus = CustomerOrder.StatusEnum.Delete.GetStatusValue();

            //var list = from w in ctx.WayBillInfos
            //           join o in ctx.CustomerOrderInfos.Where(p => p.Status != deleteSatus&&p.CustomerCode==customerCode) on w.CustomerOrderID equals
            //               o.CustomerOrderID
            //           where
            //               wayBillNumber.Contains(w.WayBillNumber) || wayBillNumber.Contains(w.TrackingNumber) ||
            //               wayBillNumber.Contains(o.CustomerOrderNumber)
            //           orderby o.CreatedOn descending
            //           select w;

            //by zxq 2014.6.18 性能优化

            var list = from w in ctx.WayBillInfos
                       join o in ctx.CustomerOrderInfos on w.CustomerOrderID equals
                           o.CustomerOrderID
                       where
                           (wayBillNumber.Contains(w.WayBillNumber) || wayBillNumber.Contains(w.TrackingNumber) ||
                            wayBillNumber.Contains(w.CustomerOrderNumber)) &&
                           w.Status != (int) WayBill.StatusEnum.Delete
                           && w.CustomerCode == customerCode
                       orderby o.CreatedOn descending
                       select w;
            return list.ToList();
        }

        public IList<WayBillInfo> GetWayBillListByWayBillNumbers(string[] wayBillNumber, string customerCode)
        {
            var ctx = this.UnitOfWork as LMS_DbContext;
            Check.Argument.IsNotNull(ctx, "数据库对象");
            var list = from w in ctx.WayBillInfos
                       join o in ctx.CustomerOrderInfos on w.CustomerOrderID equals
                           o.CustomerOrderID
                       where
                           (wayBillNumber.Contains(w.WayBillNumber)) && w.Status != (int) WayBill.StatusEnum.Delete
                           && w.CustomerCode == customerCode &&
                           !ctx.EubWayBillApplicationInfos.Any(p => p.WayBillNumber == w.WayBillNumber)
                       orderby o.CreatedOn descending
                       select w;
            return list.ToList();
        }

        public WayBillInfo GetWayBill(string number)
        {
            var ctx = this.UnitOfWork as LMS_DbContext;
            Check.Argument.IsNotNull(ctx, "数据库对象");
            Check.Argument.IsNullOrWhiteSpace(number, "单号");

            var list = from w in ctx.WayBillInfos
                       where
                           (w.WayBillNumber == number || w.TrackingNumber == number ||
                            w.CustomerOrderNumber == number) && w.Status != (int) WayBill.StatusEnum.Delete
                       select w;

            return list.FirstOrDefault();
        }

        public WayBillInfoExtSilm GetWayBillInfoExtSilm(string number)
        {
            var ctx = this.UnitOfWork as LMS_DbContext;
            Check.Argument.IsNotNull(ctx, "数据库对象");
            Check.Argument.IsNullOrWhiteSpace(number, "单号");

            string returnStr = ((int) WayBill.StatusEnum.Return).ToString();
            string deleteStr = ((int) WayBill.StatusEnum.Delete).ToString();
            var list =
                ctx.ExecuteQuery<WayBillInfoExtSilm>(
                    "select top 1 WayBillNumber,CustomerCode,TrackingNumber,GoodsTypeID,IsHold,Status,InShippingMethodID,InShippingMethodName,CountryCode,CustomerOrderNumber,Weight,EnableTariffPrepay,CustomerOrderID,Length,Width,Height from [WayBillInfos] (nolock) where (WayBillNumber=@number or TrackingNumber=@number or CustomerOrderNumber=@number or TrueTrackingNumber=@number) and Status!=" +
                    returnStr + " and Status!=" + deleteStr,
                    new SqlParameter {ParameterName = "number", Value = number, DbType = DbType.String});

            return list.FirstOrDefault();
        }


        public List<string> GetWaybillNumberList(List<string> trackingNumberList)
        {
            var ctx = this.UnitOfWork as LMS_DbContext;
            Check.Argument.IsNotNull(ctx, "数据库对象");
            Check.Argument.IsNotNull(trackingNumberList, "单号");

            var sb = new StringBuilder();
            trackingNumberList.ForEach(a =>sb.AppendFormat("'{0}',", a.Trim()));
            var getNumberString=   sb.ToString().TrimEnd(',');

            string sqlStr = string.Format("select WayBillNumber from WayBillInfos (nolock) where  trackingnumber  in({0})", getNumberString);

            var list =ctx.Database.SqlQuery<string>(sqlStr);

            return list.ToList();
        }


        public List<WayBillEventLogExt> GetWayBillEventLogExtList(int maxCount)
        {
            var ctx = this.UnitOfWork as LMS_DbContext;
            Check.Argument.IsNotNull(ctx, "数据库对象");
            var list = (from we in ctx.WayBillEventLogs
                        where we.TrackingLogCreated == false
                        join w in ctx.WayBillInfos on we.WayBillNumber equals w.WayBillNumber

                        select new WayBillEventLogExt()
                        {
                            WayBillNumber = w.WayBillNumber,
                            ShippingMethodId = w.InShippingMethodID,
                            IsHold = w.IsHold,
                            Status = w.Status,
                            EventDate = we.EventDate,
                            Description = we.Description,
                            TrackingLogCreated = we.TrackingLogCreated,
                            EventCode = we.EventCode,
                            Operator = we.Operator,
                            TrackingLogProgress = we.TrackingLogProgress,
                            WayBillEventLogId = we.WayBillEventLogId,
                            LastUpdatedOn = we.LastUpdatedOn,
                            CountryCode = w.CountryCode,
                        }).Take(maxCount).ToList();

            return list;
        }

        public List<WayBillEventLogExt> GetWayBillEventLogExtList(int startWayBillEventLogId, int maxCount)
        {
            var ctx = this.UnitOfWork as LMS_DbContext;
            Check.Argument.IsNotNull(ctx, "数据库对象");
            var list = (from we in ctx.WayBillEventLogs
                        where we.TrackingLogCreated == false && we.WayBillEventLogId > startWayBillEventLogId
                        join w in ctx.WayBillInfos on we.WayBillNumber equals w.WayBillNumber
                        orderby we.WayBillEventLogId
                        select new WayBillEventLogExt()
                        {
                            WayBillNumber = w.WayBillNumber,
                            ShippingMethodId = w.InShippingMethodID,
                            IsHold = w.IsHold,
                            Status = w.Status,
                            EventDate = we.EventDate,
                            Description = we.Description,
                            TrackingLogCreated = we.TrackingLogCreated,
                            EventCode = we.EventCode,
                            Operator = we.Operator,
                            TrackingLogProgress = we.TrackingLogProgress,
                            WayBillEventLogId = we.WayBillEventLogId,
                            LastUpdatedOn = we.LastUpdatedOn,
                            CountryCode = w.CountryCode,
                            OutStorageID = w.OutStorageID,
                            Remarks = we.Remarks
                        }).Take(maxCount).ToList();

            return list;
        }

        public WayBillInfo GetWayBillByTrackingNumber(string trackingNumber)
        {
            var ctx = this.UnitOfWork as LMS_DbContext;
            Check.Argument.IsNotNull(ctx, "数据库对象");
            Check.Argument.IsNullOrWhiteSpace(trackingNumber, "单号");

            var list = from w in ctx.WayBillInfos
                       where w.TrackingNumber == trackingNumber && w.Status != (int) WayBill.StatusEnum.Delete
                       select w;

            return list.FirstOrDefault();
        }

        public WayBillInfo GetWayBill(string wayBillNumber, string customerCode)
        {
            var ctx = this.UnitOfWork as LMS_DbContext;
            Check.Argument.IsNotNull(ctx, "数据库对象");
            Check.Argument.IsNullOrWhiteSpace(wayBillNumber, "单号");
            //if (string.IsNullOrWhiteSpace(customerCode))
            //    customerCode = "C48233";
            Check.Argument.IsNullOrWhiteSpace(customerCode, "客户编码");
            int deleteSatus = CustomerOrder.StatusEnum.Delete.GetStatusValue();

            //var list = from w in ctx.WayBillInfos
            //           join o in ctx.CustomerOrderInfos.Where(p => p.Status != deleteSatus) on w.CustomerOrderID equals
            //               o.CustomerOrderID
            //           where
            //               w.WayBillNumber == wayBillNumber || w.TrackingNumber == wayBillNumber ||
            //               o.CustomerOrderNumber == wayBillNumber
            //           select w;

            //by zxq 2014.6.18 性能优化

            var list = from w in ctx.WayBillInfos
                       where
                           (w.WayBillNumber == wayBillNumber || w.TrackingNumber == wayBillNumber
                            || w.CustomerOrderNumber == wayBillNumber) && w.Status != (int) WayBill.StatusEnum.Delete
                       select w;

            return list.FirstOrDefault();
        }

        /// <summary>
        /// 获取所有EUB运单
        /// </summary>
        /// <param name="shippingMethodIds">EUB运输方式列表</param>
        /// <param name="customerCode">客户代码</param>
        /// <returns></returns>
        public IList<WayBillInfo> GetEubWayBillList(List<int> shippingMethodIds, string customerCode)
        {
            var ctx = this.UnitOfWork as LMS_DbContext;
            Check.Argument.IsNotNull(ctx, "数据库对象");
            int deleteSatus = CustomerOrder.StatusEnum.Delete.GetStatusValue();

            //var list = from w in ctx.WayBillInfos
            //           join o in ctx.CustomerOrderInfos.Where(p => p.Status != deleteSatus && p.CustomerCode == customerCode) on w.CustomerOrderID equals
            //               o.CustomerOrderID
            //           where
            //               shippingMethodIds.Contains(w.InShippingMethodID??0)
            //           orderby o.CreatedOn descending
            //           select w;

            //by zxq 2014.6.18 性能优化
            var list = from w in ctx.WayBillInfos
                       where
                           shippingMethodIds.Contains(w.InShippingMethodID ?? 0) &&
                           w.Status != (int) WayBill.StatusEnum.Delete
                           && w.CustomerCode == customerCode
                       orderby w.CreatedOn descending
                       select w;

            return list.ToList();
        }

        /// <summary>
        /// add huhaiyou 2014-07-03
        /// </summary>
        /// <param name="shippingMethodIds"></param>
        /// <param name="customerCode"></param>
        /// <returns></returns>
        public int GetEubWayBillCount(List<int> shippingMethodIds, string customerCode)
        {
            var ctx = this.UnitOfWork as LMS_DbContext;
            Check.Argument.IsNotNull(ctx, "数据库对象");
            int deleteSatus = CustomerOrder.StatusEnum.Delete.GetStatusValue();

            var Count =
                ctx.WayBillInfos.Count(
                    p =>
                    shippingMethodIds.Contains(p.InShippingMethodID ?? 0) && p.Status != (int) WayBill.StatusEnum.Delete
                                                    && p.CustomerCode == customerCode);

            return Count;
        }


        public WayBillInfo GetWayBillInfo(string numberStr, string customerCode)
        {
            var ctx = this.UnitOfWork as LMS_DbContext;
            Check.Argument.IsNotNull(ctx, "数据库对象");
            Check.Argument.IsNullOrWhiteSpace(numberStr, "单号");
            Check.Argument.IsNullOrWhiteSpace(numberStr, "客户编码");
            int deleteStatus = CustomerOrder.StatusEnum.Delete.GetStatusValue();
            int returnStatus = CustomerOrder.StatusEnum.Return.GetStatusValue();



            //var list = from w in ctx.WayBillInfos
            //           join o in ctx.CustomerOrderInfos.Where(p => p.Status != deleteStatus&&p.Status!=returnStatus) on w.CustomerOrderID equals
            //               o.CustomerOrderID
            //           where
            //               w.WayBillNumber == numberStr || w.TrackingNumber == numberStr ||w.TrueTrackingNumber==numberStr||
            //               o.CustomerOrderNumber == numberStr
            //           select w;

            //by zxq 2014.6.18 性能优化

            var list = from w in ctx.WayBillInfos
                       where
                           (w.WayBillNumber == numberStr || w.TrackingNumber == numberStr ||
                            w.TrueTrackingNumber == numberStr ||
                            w.CustomerOrderNumber == numberStr) && w.Status != (int) WayBill.StatusEnum.Delete &&
                           w.Status != (int) WayBill.StatusEnum.Return
                       select w;

            return list.FirstOrDefault();
        }


        public decimal GetWayBillWeight(string wayBillNumber)
        {
            var ctx = this.UnitOfWork as LMS_DbContext;
            Check.Argument.IsNotNull(ctx, "数据库对象");
            var result = from a in ctx.WayBillInfos.Where(a => a.WayBillNumber==wayBillNumber)
                         join b in ctx.WeightAbnormalLogs on a.WayBillNumber equals b.WayBillNumber
                         select b.Weight;

            return result.FirstOrDefault();

        }

        /// <summary>
        /// 快递货物信息查询
        /// Add By zhengsong
        /// Time:2014-06-19
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public List<ExpressWayBillExt> GetExpressWayBillDetailList(ExpressWayBillParam param)
        {
            var ctx = this.UnitOfWork as LMS_DbContext;
            Check.Argument.IsNotNull(ctx, "数据库对象");
            var have = WayBill.StatusEnum.Have.GetStatusValue();
            var send = WayBill.StatusEnum.Send.GetStatusValue();
            var waitOrder = WayBill.StatusEnum.WaitOrder.GetStatusValue();
            var delivered = WayBill.StatusEnum.Delivered.GetStatusValue();
            Expression<Func<WayBillInfo, bool>> filterWayBill = o => true;
            if (!string.IsNullOrWhiteSpace(param.SearchContext))
            {
                var numberList =
                    param.SearchContext.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
                         .ToList();
                if (param.SearchWhere.HasValue && numberList.Count > 0)
                {
                    switch (WayBill.ParseToSearchFilter(param.SearchWhere.Value))
                    {
                        case WayBill.SearchFilterEnum.WayBillNumber:
                            filterWayBill = filterWayBill.And(p => numberList.Contains(p.WayBillNumber));
                            break;
                        case WayBill.SearchFilterEnum.TrackingNumber:
                            filterWayBill = filterWayBill.And(p => numberList.Contains(p.TrackingNumber));
                            break;
                        case WayBill.SearchFilterEnum.CustomerOrderNumber:
                            filterWayBill = filterWayBill.And(p => numberList.Contains(p.CustomerOrderNumber));
                            break;
                    }
                }
                filterWayBill =
                    filterWayBill.And(
                        p => p.Status == have || p.Status == send || p.Status == waitOrder || p.Status == delivered);
            }
            else
            {
                switch (WayBill.ParseToDateFilter(param.DateWhere))
                {
                    case WayBill.DateFilterEnum.CreatedOn:
                        filterWayBill = filterWayBill.AndIf(r => r.CreatedOn >= param.StartTime,
                                                            param.StartTime.HasValue)
                                                     .AndIf(r => r.CreatedOn <= param.EndTime, param.EndTime.HasValue);
                        break;
                    case WayBill.DateFilterEnum.TakeOverOn:
                        filterWayBill = filterWayBill.AndIf(r => r.InStorageCreatedOn >= param.StartTime,
                                                            param.StartTime.HasValue)
                                                     .AndIf(r => r.InStorageCreatedOn <= param.EndTime,
                                                            param.EndTime.HasValue);
                        break;
                    case WayBill.DateFilterEnum.DeliverOn:
                        filterWayBill = filterWayBill.AndIf(r => r.OutStorageCreatedOn >= param.StartTime,
                                                            param.StartTime.HasValue)
                                                     .AndIf(r => r.OutStorageCreatedOn <= param.EndTime,
                                                            param.EndTime.HasValue);
                        break;
                }

                filterWayBill = filterWayBill.AndIf(p => p.CustomerCode == param.CustomerCode,
                                                    !string.IsNullOrWhiteSpace(param.CustomerCode))
                                             .AndIf(p => p.InShippingMethodID == param.ShippingMethodId.Value,
                                                    param.ShippingMethodId.HasValue)
                                             .AndIf(p => p.Status == param.Status.Value, param.Status.HasValue)
                                             .AndIf(
                                                 p => p.Status == have || p.Status == send || p.Status == waitOrder ||
                                                         p.Status == delivered, !param.Status.HasValue);
            }
            // 按条件查询运单
            //var wayB = from w in ctx.WayBillInfos.Where(filterWayBill)
            //           orderby w.OutStorageCreatedOn
            //           select new ExpressWayBillViewExt
            //           {
            //               WayBillNumber = w.WayBillNumber,
            //               CountryCode = w.CountryCode,
            //               CustomerCode = w.CustomerCode,
            //               CustomerOrderNumber = w.CustomerOrderNumber,
            //               TrackingNumber = w.TrackingNumber,
            //               Status = w.Status,
            //               SettleWeight = w.SettleWeight ?? 0,
            //               Weight = w.Weight ?? 0,
            //               InShippingMethodID = w.InShippingMethodID ?? 0,
            //               InShippingMethodName = w.InShippingMethodName,
            //               OutStorageTime = w.OutStorageCreatedOn
            //           };
            //List<string> WayBillNumbers = new List<string>();
            //wayB.ToList().ForEach(p =>
            //{
            //    if (!WayBillNumbers.Contains(p.WayBillNumber))
            //    {
            //        WayBillNumbers.Add(p.WayBillNumber);
            //    }
            //});
            //// 查询出有详细信息的运单
            //var wayBillDetails = (from d in ctx.WaybillPackageDetails.Where(p => WayBillNumbers.Contains(p.WayBillNumber))
            //                      select new WayBillDetailExt
            //                      {
            //                          PackageDetailID = d.PackageDetailID,
            //                          WayBillNumber = d.WayBillNumber,
            //                          Weight = d.Weight ?? 0,
            //                          SettleWeight = d.SettleWeight ?? 0,
            //                          Length = d.Length ?? 0,
            //                          Width = d.Width ?? 0,
            //                          Height = d.Height ?? 0,
            //                          AddWeight = d.AddWeight ?? 0
            //                      }).ToList<WayBillDetailExt>();
            //List<string> returnWayBillNumbers = new List<string>();
            //wayBillDetails.ForEach(p =>
            //{
            //    if (!returnWayBillNumbers.Contains(p.WayBillNumber))
            //    {
            //        returnWayBillNumbers.Add(p.WayBillNumber);
            //    }
            //});
            var wayBills = from w in ctx.WayBillInfos.Where(filterWayBill)
                           join d in ctx.WaybillPackageDetails on w.WayBillNumber equals d.WayBillNumber
                               into wdinfo
                           from d in wdinfo.DefaultIfEmpty()
                           orderby w.OutStorageCreatedOn
                           select new ExpressWayBillExt
                               {
                                   WayBillNumber = w.WayBillNumber,
                                   CountryCode = w.CountryCode,
                                   CustomerCode = w.CustomerCode,
                                   CustomerOrderNumber = w.CustomerOrderNumber,
                                   TrackingNumber = w.TrackingNumber,
                                   Status = w.Status,
                                   WayBillSettleWeight = w.SettleWeight ?? 0,
                                   WayBillWeight = w.Weight ?? 0,
                                   InShippingMethodID = w.InShippingMethodID ?? 0,
                                   InShippingMethodName = w.InShippingMethodName,
                                   OutStorageTime = w.OutStorageCreatedOn,
                                   PackageDetailID = d == null ? 0 : d.PackageDetailID,
                                   DetailSettleWeight = d == null ? 0 : d.SettleWeight ?? 0,
                                   DetailWeight = d == null ? 0 : d.Weight ?? 0,
                                   AddWeight = d == null ? 0 : d.AddWeight ?? 0,
                                   Length = d == null ? 0 : d.Length ?? 0,
                                   Width = d == null ? 0 : d.Width ?? 0,
                                   Height = d == null ? 0 : d.Height ?? 0
                               };

            return wayBills.ToList();
        }

        public IPagedList<ExpressWayBillViewExt> GetPagedWayBillDetailList(ExpressWayBillParam param)
        {
            return GetWayBillDetailList(param);
        }

        public List<LabelPrintExt> GetLabelPrintExtList(IEnumerable<string> orderNumbers, string customercode)
        {
            var ctx = this.UnitOfWork as LMS_DbContext;
            Check.Argument.IsNotNull(ctx, "数据库对象");
            Check.Argument.IsNullOrWhiteSpace(customercode, "客户编码");
            Expression<Func<WayBillInfo, bool>> filter = p => true;
            filter = filter.And(p => p.CustomerCode == customercode)
                           .And(
                               p =>
                               orderNumbers.Contains(p.CustomerOrderNumber) || orderNumbers.Contains(p.WayBillNumber) ||
                               orderNumbers.Contains(p.TrackingNumber));
            var list = (from l in ctx.WayBillInfos.Where(filter)
                        select new LabelPrintExt
                            {
                                CustomerOrderId = l.CustomerOrderID ?? 0,
                                CustomerOrderNumber = l.CustomerOrderNumber,
                                Trackingnumber = l.TrackingNumber,
                                WayBillNumber = l.WayBillNumber
                            }).ToList<LabelPrintExt>();
            return list;
        }

        public IPagedList<ExpressWayBillViewExt> GetWayBillDetailList(ExpressWayBillParam param)
        {
            var ctx = this.UnitOfWork as LMS_DbContext;
            Check.Argument.IsNotNull(ctx, "数据库对象");
            var have = WayBill.StatusEnum.Have.GetStatusValue();
            var send = WayBill.StatusEnum.Send.GetStatusValue();
            var waitOrder = WayBill.StatusEnum.WaitOrder.GetStatusValue();
            var delivered = WayBill.StatusEnum.Delivered.GetStatusValue();
            Expression<Func<WayBillInfo, bool>> filterWayBill = o => true;
            //var wayBillDetails = (from d in ctx.WaybillPackageDetails
            //           select new WayBillDetailExt
            //           {
            //               PackageDetailID = d.PackageDetailID,
            //               WayBillNumber = d.WayBillNumber,
            //               Weight = d.Weight ?? 0,
            //               SettleWeight = d.SettleWeight ?? 0,
            //               Length = d.Length ?? 0,
            //               Width = d.Width ?? 0,
            //               Height = d.Height ?? 0,
            //               AddWeight = d.AddWeight??0
            //           }).ToList<WayBillDetailExt>();
            //List<string> WayBillNumbers = new List<string>();
            //wayBillDetails.ForEach(p =>
            //{
            //    if (!WayBillNumbers.Contains(p.WayBillNumber))
            //    {
            //        WayBillNumbers.Add(p.WayBillNumber);
            //    }
            //});

            if (!string.IsNullOrWhiteSpace(param.SearchContext))
            {
                var numberList =
                    param.SearchContext.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
                         .ToList();
                if (param.SearchWhere.HasValue && numberList.Count > 0)
                {
                    switch (WayBill.ParseToSearchFilter(param.SearchWhere.Value))
                    {
                        case WayBill.SearchFilterEnum.WayBillNumber:
                            filterWayBill = filterWayBill.And(p => numberList.Contains(p.WayBillNumber));
                            break;
                        case WayBill.SearchFilterEnum.TrackingNumber:
                            filterWayBill = filterWayBill.And(p => numberList.Contains(p.TrackingNumber));
                            break;
                        case WayBill.SearchFilterEnum.CustomerOrderNumber:
                            filterWayBill = filterWayBill.And(p => numberList.Contains(p.CustomerOrderNumber));
                            break;
                    }
                }
                filterWayBill =
                    filterWayBill.And(
                        p => p.Status == have || p.Status == send || p.Status == waitOrder || p.Status == delivered);
            }
            else
            {
                switch (WayBill.ParseToDateFilter(param.DateWhere))
                {
                    case WayBill.DateFilterEnum.CreatedOn:
                        filterWayBill = filterWayBill.AndIf(r => r.CreatedOn >= param.StartTime,
                                                            param.StartTime.HasValue)
                                                     .AndIf(r => r.CreatedOn <= param.EndTime, param.EndTime.HasValue);
                        break;
                    case WayBill.DateFilterEnum.TakeOverOn:
                        filterWayBill = filterWayBill.AndIf(r => r.InStorageCreatedOn >= param.StartTime,
                                                            param.StartTime.HasValue)
                                                     .AndIf(r => r.InStorageCreatedOn <= param.EndTime,
                                                            param.EndTime.HasValue);
                        break;
                    case WayBill.DateFilterEnum.DeliverOn:
                        filterWayBill = filterWayBill.AndIf(r => r.OutStorageCreatedOn >= param.StartTime,
                                                            param.StartTime.HasValue)
                                                     .AndIf(r => r.OutStorageCreatedOn <= param.EndTime,
                                                            param.EndTime.HasValue);
                        break;
                }

                filterWayBill = filterWayBill.AndIf(p => p.CustomerCode == param.CustomerCode,
                                                    !string.IsNullOrWhiteSpace(param.CustomerCode))
                                             .AndIf(p => p.InShippingMethodID == param.ShippingMethodId.Value,
                                                    param.ShippingMethodId.HasValue)
                                             .AndIf(p => p.Status == param.Status.Value, param.Status.HasValue)
                                             .AndIf(
                                                 p =>
                                                 p.Status == have || p.Status == send || p.Status == waitOrder ||
                                                 p.Status == delivered, !param.Status.HasValue);
            }


            var wayBills = from w in ctx.WayBillInfos.Where(filterWayBill)
                           orderby w.OutStorageCreatedOn
                           select new ExpressWayBillViewExt
                           {
                               WayBillNumber = w.WayBillNumber,
                               CountryCode = w.CountryCode,
                               CustomerCode = w.CustomerCode,
                               CustomerOrderNumber = w.CustomerOrderNumber,
                               TrackingNumber = w.TrackingNumber,
                               Status = w.Status,
                               SettleWeight = w.SettleWeight ?? 0,
                               Weight = w.Weight ?? 0,
                               InShippingMethodID = w.InShippingMethodID ?? 0,
                               InShippingMethodName = w.InShippingMethodName,
                               OutStorageTime = w.OutStorageCreatedOn
                           };
            var list = wayBills.ToPagedList(param.Page, param.PageSize);

            var wayBillNumbers = from l in list.InnerList
                                 select l.WayBillNumber;
            var wayBillDetails =
                (from d in ctx.WaybillPackageDetails.Where(w => wayBillNumbers.Contains(w.WayBillNumber))
                 select new WayBillDetailExt
                 {
                     PackageDetailID = d.PackageDetailID,
                     WayBillNumber = d.WayBillNumber,
                     Weight = d.Weight ?? 0,
                     SettleWeight = d.SettleWeight ?? 0,
                     AddWeight = d.AddWeight ?? 0,
                     Length = d.Length ?? 0,
                     Width = d.Width ?? 0,
                     Height = d.Height ?? 0
                 }).ToList<WayBillDetailExt>();
            list.InnerList.ForEach(l =>
            {
                l.wayBillDetails = wayBillDetails.Where(w => w.WayBillNumber == l.WayBillNumber).ToList();
            });
            return list;
        }



        public List<string> IsWayBillnumberInFeeInfo(List<string> waybillNumber)
        {
            var ctx = this.UnitOfWork as LMS_DbContext;
            Check.Argument.IsNotNull(ctx, "数据库对象");

            var result = from a in ctx.ReceivingExpenses.Where(a => waybillNumber.Contains(a.WayBillNumber))
                         join b in ctx.ReceivingExpenseInfos on a.ReceivingExpenseID equals b.ReceivingExpenseID
                         select a.WayBillNumber;

            return result.ToList();

        }




        public IPagedList<WaybillInfoUpdateExt> GetWaybillInfoUpdatePagedList(WaybillInfoUpdateParam param)
        {
            var ctx = this.UnitOfWork as LMS_DbContext;
            Check.Argument.IsNotNull(ctx, "数据库对象");
            var startTime = param.StartTime.HasValue ? param.StartTime.Value : new DateTime(2013, 1, 1);
            var endTime = param.EndTime.HasValue ? param.EndTime.Value : new DateTime(2020, 1, 1);
            var numberList = new List<string>();
            var submittedStutas = (int) WayBill.StatusEnum.Submitted;
            var haveStatus = (int) WayBill.StatusEnum.Have;

            if (!string.IsNullOrWhiteSpace(param.SearchContext))
            {
                numberList =
                    param.SearchContext.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
                            .ToList();
            }
            Expression<Func<WayBillInfo, bool>> filter = p => true;


            if (param.SearchWhere.HasValue && numberList.Count > 0)
            {
                //只显示已提交，收货
                filter = filter.And(p => p.Status == submittedStutas || p.Status == haveStatus);

                switch (WayBill.ParseToSearchFilter(param.SearchWhere.Value))
                {
                    case WayBill.SearchFilterEnum.WayBillNumber:

                        //搜索原运单号对应的新单号
                        var getWayBillChangeLogs =
                            from a in ctx.WayBillChangeLogs.Where(p => numberList.Contains(p.OriginalWayBillNumber))
                                                   select new
                                                   {
                                                       WayBillNumber = a.WayBillNumber
                                                   };

                        List<string> getSearchOriginalWayBillNumber = new List<string>();
                        getWayBillChangeLogs.ToList().ForEach(a => getSearchOriginalWayBillNumber.Add(a.WayBillNumber));
                        //排除搜索的原单号
                        List<string> waybillNumberList = numberList.Except(getSearchOriginalWayBillNumber).ToList();


                        if (waybillNumberList.Any() && !getSearchOriginalWayBillNumber.Any())
                        {
                            filter = filter.And(p => waybillNumberList.Contains(p.WayBillNumber));
                        }

                        if (!waybillNumberList.Any() && getSearchOriginalWayBillNumber.Any())
                        {
                            filter = filter.And(p => getSearchOriginalWayBillNumber.Contains(p.WayBillNumber));
                        }


                        if (getSearchOriginalWayBillNumber.Any() && waybillNumberList.Any())
                        {
                            filter = filter.And(p => getSearchOriginalWayBillNumber.Contains(p.WayBillNumber));
                            filter = filter.Or(p => waybillNumberList.Contains(p.WayBillNumber));
                            filter = filter.And(p => p.Status == submittedStutas || p.Status == haveStatus);
                        }

                        break;
                    case WayBill.SearchFilterEnum.TrackingNumber:
                        filter =
                            filter.And(
                                p => numberList.Contains(p.TrackingNumber) || numberList.Contains(p.TrueTrackingNumber));
                        break;
                    case WayBill.SearchFilterEnum.CustomerOrderNumber:
                        filter = filter.And(p => numberList.Contains(p.CustomerOrderNumber));
                        break;
                }
            }
            else
            {
                //没有选状态
                if (!param.Status.HasValue)
                {
                    filter = filter.And(p => p.Status == submittedStutas || p.Status == haveStatus);
                }

                filter = filter.AndIf(p => p.CustomerCode == param.CustomerCode,
                                      !string.IsNullOrWhiteSpace(param.CustomerCode))
                               .AndIf(p => p.InShippingMethodID == param.ShippingMethodId,
                                      param.ShippingMethodId.HasValue)
                        .And(p => startTime <= p.CreatedOn && p.CreatedOn <= endTime)
                        .AndIf(p => p.Status == param.Status, param.Status.HasValue);
            }



            var getWaybillList = from w in ctx.WayBillInfos.Where(filter)
                                 join k in ctx.Customers on w.CustomerCode equals k.CustomerCode into g
                                 from result in g.DefaultIfEmpty()
                                 join ww in ctx.WayBillChangeLogs on
                                 w.WayBillNumber equals ww.WayBillNumber
                                 into gg
                                 from results in gg.DefaultIfEmpty()

                                 select new WaybillInfoUpdateExt
                                 {
                                     WayBillNumber = w.WayBillNumber,
                                     RawWayBillNumber = results.OriginalWayBillNumber,
                                     CustomerOrderNumber = w.CustomerOrderNumber,
                                     TrackingNumber = w.TrackingNumber,
                                     CustomerCode = w.CustomerCode,
                                     CustomerName = result.Name,
                                     InShippingMethodName = w.InShippingMethodName,
                                     InShippingMethodID = w.InShippingMethodID,
                                     CountryCode = w.CountryCode,
                                     Status = w.Status,
                                     IsHold = w.IsHold,
                                     CreatedOn = w.CreatedOn

                                 };

            return getWaybillList.OrderByDescending(a => a.CreatedOn).ToPagedList(param.Page, param.PageSize);
        }




        public IPagedList<InStorageWeightAbnormalExt> GetInStorageWeightAbnormaPagedList(
            InStorageWeightAbnormalParam param)
        {
            var ctx = this.UnitOfWork as LMS_DbContext;
            Check.Argument.IsNotNull(ctx, "数据库对象");
            var startTime = param.StartTime.HasValue ? param.StartTime.Value : new DateTime(2013, 1, 1);
            var endTime = param.EndTime.HasValue ? param.EndTime.Value : new DateTime(2020, 1, 1);
            var numberList = new List<string>();

            if (!string.IsNullOrWhiteSpace(param.SearchContext))
            {
                numberList =
                    param.SearchContext.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
                            .ToList();
            }

            Expression<Func<WayBillInfo, bool>> filter = p => true;
            Expression<Func<WeightAbnormalLog, bool>> weightAbnormalLogFilter = p => true;


            if (param.SearchWhere.HasValue && numberList.Count > 0)
            {
                switch (WayBill.ParseToSearchFilter(param.SearchWhere.Value))
                {
                    case WayBill.SearchFilterEnum.WayBillNumber:
                        filter = filter.And(p => numberList.Contains(p.WayBillNumber));
                        break;
                    case WayBill.SearchFilterEnum.TrackingNumber:
                        filter =
                            filter.And(
                                p => numberList.Contains(p.TrackingNumber) || numberList.Contains(p.TrueTrackingNumber));
                        break;
                    case WayBill.SearchFilterEnum.CustomerOrderNumber:
                        filter = filter.And(p => numberList.Contains(p.CustomerOrderNumber));
                        break;
                }
            }
            else
            {
                weightAbnormalLogFilter = weightAbnormalLogFilter.AndIf(p => p.CustomerCode == param.CustomerCode,
                                                                        !string.IsNullOrWhiteSpace(param.CustomerCode))
                                                                 .And(
                                                                     p =>
                                                                     startTime <= p.CreatedOn &&
                                                                     p.CreatedOn <= endTime);

                filter = filter.AndIf(p => p.InShippingMethodID == param.ShippingMethodId,
                                      param.ShippingMethodId.HasValue);
            }




            //只加载该类型(入仓重量对比异常),运单为拦截的单
            filter = filter.And(a => a.IsHold);
            const int getStatus = (int) WayBill.AbnormalTypeEnum.InStorageWeightAbnormal;

            var result = from a in ctx.WayBillInfos.Where(filter)
                         join b in ctx.WeightAbnormalLogs.Where(weightAbnormalLogFilter) on a.WayBillNumber equals
                             b.WayBillNumber
                         join c in ctx.AbnormalWayBillLogs.Where(a => a.OperateType == getStatus) on a.AbnormalID equals
                             c.AbnormalID


                         select new InStorageWeightAbnormalExt
                         {
                             WayBillNumber = b.WayBillNumber,
                             CustomerOrderNumber = a.CustomerOrderNumber,
                             TrackingNumber = a.TrackingNumber ?? a.RawTrackingNumber,
                             CustomerCode = b.CustomerCode,
                             InShippingMethodName = a.InShippingMethodName,
                             InShippingMethodId = a.InShippingMethodID,
                             CountryCode = a.CountryCode,
                             Weight = b.Weight, //称重重量
                             ForecastWeight = a.Weight.HasValue ? a.Weight.Value : 0, //预报重量
                             OperateType = getStatus, //入仓重量对比异常
                             AbnormalDescription = c.AbnormalDescription,
                             CreatedOn = b.CreatedOn,
                             IsHold = a.IsHold
                 
                         
                         };

            IQueryable<InStorageWeightAbnormalExt> list = null;
            if (!string.IsNullOrEmpty(param.IsWeightGtWeight))//是否称重重量大于预报重量
            {
                list = param.IsWeightGtWeight=="true" ? result.Where(a => a.Weight > a.ForecastWeight) : result.Where(a => a.Weight < a.ForecastWeight);
            }
            else
            {
                list = result;
            }
            return list.ToList().OrderByDescending(b => b.CreatedOn).ToPagedList(param.Page, param.PageSize);
        }


        public List<ExportInStorageWeightAbnormalExt> GetExportInStorageWeightAbnormal(
            InStorageWeightAbnormalParam param)
        {
            var ctx = this.UnitOfWork as LMS_DbContext;
            Check.Argument.IsNotNull(ctx, "数据库对象");
            var startTime = param.StartTime.HasValue ? param.StartTime.Value : new DateTime(2013, 1, 1);
            var endTime = param.EndTime.HasValue ? param.EndTime.Value : new DateTime(2020, 1, 1);
            var numberList = new List<string>();

            if (!string.IsNullOrWhiteSpace(param.SearchContext))
            {
                numberList =
                    param.SearchContext.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
                            .ToList();
            }

            Expression<Func<WayBillInfo, bool>> filter = p => true;
            Expression<Func<WeightAbnormalLog, bool>> weightAbnormalLogFilter = p => true;


            if (param.SearchWhere.HasValue && numberList.Count > 0)
            {
                switch (WayBill.ParseToSearchFilter(param.SearchWhere.Value))
                {
                    case WayBill.SearchFilterEnum.WayBillNumber:
                        filter = filter.And(p => numberList.Contains(p.WayBillNumber));
                        break;
                    case WayBill.SearchFilterEnum.TrackingNumber:
                        filter =
                            filter.And(
                                p => numberList.Contains(p.TrackingNumber) || numberList.Contains(p.TrueTrackingNumber));
                        break;
                    case WayBill.SearchFilterEnum.CustomerOrderNumber:
                        filter = filter.And(p => numberList.Contains(p.CustomerOrderNumber));
                        break;
                }
            }
            else
            {
                weightAbnormalLogFilter = weightAbnormalLogFilter.AndIf(p => p.CustomerCode == param.CustomerCode,
                                                                        !string.IsNullOrWhiteSpace(param.CustomerCode))
                                                                 .And(
                                                                     p =>
                                                                     param.StartTime <= p.CreatedOn &&
                                                                     p.CreatedOn <= param.EndTime);

                filter = filter.AndIf(p => p.InShippingMethodID == param.ShippingMethodId,
                                      param.ShippingMethodId.HasValue);
            }


            //只加载该类型(入仓重量对比异常),运单为拦截的单
            filter = filter.And(a => a.IsHold);
            const int getStatus = (int) WayBill.AbnormalTypeEnum.InStorageWeightAbnormal;

            var result = from a in ctx.WayBillInfos.Where(filter)
                         join b in ctx.WeightAbnormalLogs.Where(weightAbnormalLogFilter) on a.WayBillNumber equals
                             b.WayBillNumber
                         join c in ctx.AbnormalWayBillLogs.Where(a => a.OperateType == getStatus) on a.AbnormalID equals
                             c.AbnormalID
                         join d in ctx.CustomerOrderInfos on a.CustomerOrderID equals d.CustomerOrderID
                         join e in ctx.Countries on a.CountryCode equals e.CountryCode
                         join f in ctx.Customers on a.CustomerCode equals f.CustomerCode
                         select new ExportInStorageWeightAbnormalExt
                         {
                             CustomerName = f.Name,
                             CustomerOrderNumber = d.CustomerOrderNumber,
                             WayBillNumber = a.WayBillNumber,
                             InShippingMethodName = a.InShippingMethodName,
                             TrackingNumber = a.TrackingNumber,
                             CountryCode = a.CountryCode,
                             ChineseName = e.ChineseName,
                             CreatedOn = b.CreatedOn,
                             PackageNumber = d.PackageNumber ?? 0,
                             Length = a.Length ?? 0,
                             Width = a.Width ?? 0,
                             Height = a.Height ?? 0,
                             ForecastWeight = a.Weight ?? 0,
                                 Weight = b.Weight, //称重重量
                             Deviation = (b.Weight - a.Weight ?? 0)
                         };

            return result.ToList();
        }

        /// <summary>
        /// 查询运单汇总报表--Jess
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public List<WaybillSummary> GetWaybillSummaryList(WaybillSummaryParam param)
        {
            var sbWhere = new StringBuilder();
            if (!param.Status.IsNullOrWhiteSpace())
            {
                sbWhere.AppendFormat(" AND Status IN ({0}) ", param.Status);
            }
            else
            {
                sbWhere.AppendFormat(" AND Status <> {0} ", WayBill.StatusToValue(WayBill.StatusEnum.Delete));
            }
            if (!param.CustomerCode.IsNullOrWhiteSpace())
            {
                sbWhere.AppendFormat(" AND CustomerCode='{0}' ", param.CustomerCode);
            }
            if (!param.VenderCode.IsNullOrWhiteSpace())
            {
                sbWhere.AppendFormat(" AND VenderCode='{0}' ", param.VenderCode);
            }
            if (param.ShippingMethodId.HasValue)
            {
                if (param.SelectShippingMethod == 1) //入仓运输方式
                {
                    sbWhere.AppendFormat(" AND InShippingMethodID={0}", param.ShippingMethodId.Value);
                }
                else if (param.SelectShippingMethod == 2) //出仓运输方式
                {
                    sbWhere.AppendFormat(" AND OutShippingMethodID={0}", param.ShippingMethodId.Value);
                }
            }
            if (param.StartTime.HasValue || param.EndTime.HasValue)
            {
                switch (WayBill.ParseToDateFilter(param.SelectTimeName))
                {
                    case WayBill.DateFilterEnum.CreatedOn:
                        if (param.StartTime.HasValue)
                        {
                            sbWhere.AppendFormat(" AND CreatedOn>='{0}'",
                                                 param.StartTime.Value.ToString("yyyy-MM-dd HH:mm:ss"));
                        }
                        if (param.EndTime.HasValue)
                        {
                            sbWhere.AppendFormat(" AND CreatedOn<='{0}'",
                                                 param.EndTime.Value.ToString("yyyy-MM-dd HH:mm:ss"));
                        }
                        break;
                    case WayBill.DateFilterEnum.DeliverOn:
                        if (param.StartTime.HasValue)
                        {
                            sbWhere.AppendFormat(" AND OutStorageCreatedOn>='{0}'",
                                                 param.StartTime.Value.ToString("yyyy-MM-dd HH:mm:ss"));
                        }
                        if (param.EndTime.HasValue)
                        {
                            sbWhere.AppendFormat(" AND OutStorageCreatedOn<='{0}'",
                                                 param.EndTime.Value.ToString("yyyy-MM-dd HH:mm:ss"));
                        }
                        break;
                    case WayBill.DateFilterEnum.TakeOverOn:
                        if (param.StartTime.HasValue)
                        {
                            sbWhere.AppendFormat(" AND InStorageCreatedOn>='{0}'",
                                                 param.StartTime.Value.ToString("yyyy-MM-dd HH:mm:ss"));
                        }
                        if (param.EndTime.HasValue)
                        {
                            sbWhere.AppendFormat(" AND InStorageCreatedOn<='{0}'",
                                                 param.EndTime.Value.ToString("yyyy-MM-dd HH:mm:ss"));
                        }
                        break;
                }
            }
            string sql =
                @"SELECT CustomerCode,InShippingMethodID,OutShippingMethodID,VenderCode,COUNT(1) TotalCount,SUM(CASE WHEN InStorageID IS NOT NULL THEN ISNULL(weight,0) ELSE 0 END) SumWeight,
		                    SUM(CASE WHEN InStorageID IS NOT NULL AND Status<>7 AND Status<>9 THEN 1 ELSE 0 END) InCount,SUM(CASE WHEN OutStorageID IS NOT NULL AND Status<>7 AND Status<>9 THEN 1 ELSE 0 END) OutCount,
		                    SUM(CASE WHEN Status=7 THEN 1 WHEN Status=9 THEN 1 ELSE 0 END) ReturnCount,SUM(CASE WHEN IsHold=1 THEN 1 ELSE 0 END) IsHoldCount
                    FROM WayBillInfos 
                    where 1=1 {0}
                    GROUP BY CustomerCode,InShippingMethodID,OutShippingMethodID,VenderCode
                    ORDER BY CustomerCode,InShippingMethodID,OutShippingMethodID,VenderCode";
            var list = new List<WaybillSummary>();
            using (var ctx = new LMS_DbContext())
            {
                ctx.Configuration.AutoDetectChangesEnabled = false;
                list = ctx.Database.SqlQuery<WaybillSummary>(string.Format(sql, sbWhere.ToString())).ToList();
            }
            return list;
        }


        /// <summary>
        /// 大量数据插入
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="list"></param>
        private void BulkInsert<T>(string connection, string tableName, IList<T> list)
        {
            var table = new DataTable();
            var props = TypeDescriptor.GetProperties(typeof (T))
                //Dirty hack to make sure we only have system data types 
                //i.e. filter out the relationships/collections
                                       .Cast<PropertyDescriptor>()
                                       .Where(propertyInfo => propertyInfo.PropertyType.Namespace.Equals("System"))
                                       .ToArray();

            using (var bulkCopy = new SqlBulkCopy(connection))
            {
                bulkCopy.BatchSize = list.Count;
                bulkCopy.DestinationTableName = tableName;
                bulkCopy.BulkCopyTimeout = 10000;
                foreach (var propertyInfo in props)
                {
                    bulkCopy.ColumnMappings.Add(propertyInfo.Name, propertyInfo.Name);
                    table.Columns.Add(propertyInfo.Name,
                                      Nullable.GetUnderlyingType(propertyInfo.PropertyType) ?? propertyInfo.PropertyType);
                }

                var values = new object[props.Length];
                foreach (var item in list)
                {
                    for (var i = 0; i < values.Length; i++)
                    {
                        values[i] = props[i].GetValue(item);
                    }

                    table.Rows.Add(values);
                }

                bulkCopy.WriteToServer(table);
            }
        }


        /// <summary>
        /// 大量数据插入
        /// </summary>
        public void BulkInsert<T>(string tableName, IList<T> list)
        {
            var ctx = this.UnitOfWork as LMS_DbContext;

            Check.Argument.IsNotNull(ctx, "数据库对象");

            string connection = ctx.Database.Connection.ConnectionString;

            BulkInsert(connection, tableName, list);
        }


        public List<int?> GetExistCustomerOrderNumber(List<string> customerOrderNumbers)
        {
            var ctx = this.UnitOfWork as LMS_DbContext;
            Check.Argument.IsNotNull(ctx, "数据库对象");

            var list = from w in ctx.WayBillInfos
                       where
                           customerOrderNumbers.Contains(w.CustomerOrderNumber) &&
                           w.Status != (int) WayBill.StatusEnum.Delete && w.Status != (int) WayBill.StatusEnum.Return
                       select w.CustomerOrderID;


            return list.ToList();
        }

        public IPagedList<WayBillInfo> FindPagedListExt(int pageIndex, int pageSize,
                                                        Expression<Func<WayBillInfo, bool>> expression,
                                                        Func<IQueryable<WayBillInfo>, IOrderedQueryable<WayBillInfo>>
                                                            orderByExpression)
        {
            var ctx = this.UnitOfWork as LMS_DbContext;
            Check.Argument.IsNotNull(ctx, "数据库对象");
            ctx.Configuration.LazyLoadingEnabled = false;
            var list = this.FindPagedList(pageIndex, pageSize, expression, orderByExpression);
            ctx.Configuration.LazyLoadingEnabled = true;
            return list;
        }


        public IPagedList<WayBillInfoListSilm> FindPagedListSilm(int pageIndex, int pageSize,
                                                                 Expression<Func<WayBillInfo, bool>> expression,
                                                                 Func
                                                                     <IQueryable<WayBillInfoListSilm>,
                                                                     IOrderedQueryable<WayBillInfoListSilm>>
                                                                     orderByExpression)
        {
            var ctx = this.UnitOfWork as LMS_DbContext;
            Check.Argument.IsNotNull(ctx, "数据库对象");
            ctx.Configuration.LazyLoadingEnabled = false;

            var list = orderByExpression(from w in ctx.WayBillInfos.Where(expression)
                                         select new WayBillInfoListSilm
                                             {
                                                 Status = w.Status,
                                                 WayBillNumber = w.WayBillNumber,
                                                 CustomerOrderNumber = w.CustomerOrderNumber,
                                                 CustomerCode = w.CustomerCode,
                                                 CreatedOn = w.CreatedOn,
                                                 IsHold = w.IsHold,
                                                 EnableTariffPrepay = w.EnableTariffPrepay,
                                                 TrackingNumber = w.TrackingNumber,
                                                 CountryCode = w.CountryCode,
                                                 InShippingMethodID = w.InShippingMethodID,
                                                 InShippingName = w.InShippingMethodName,
                                                 InCreatedBy = w.InStorageInfo.CreatedBy,
                                                 OutCreatedBy = w.OutStorageInfo.CreatedBy,
                                                 OutShippingMethodID = w.OutShippingMethodID,
                                                 OutShippingName = w.OutShippingMethodName,
                                                 SettleWeight = w.SettleWeight,
                                             }).ToPagedList(pageIndex, pageSize);
            ctx.Configuration.LazyLoadingEnabled = true;
            return list;
        }

        public IPagedList<AbnormalWayBillModel> FindPagedListAbnormalWayBillModel(int pageIndex, int pageSize,
                                                                                  Expression<Func<WayBillInfo, bool>>
                                                                                      expression,
                                                                                  Func
                                                                                      <IQueryable<AbnormalWayBillModel>,
                                                                                      IOrderedQueryable
                                                                                      <AbnormalWayBillModel>>
                                                                                      orderByExpression)
        {
            var ctx = this.UnitOfWork as LMS_DbContext;
            Check.Argument.IsNotNull(ctx, "数据库对象");
            ctx.Configuration.LazyLoadingEnabled = false;

            var list = orderByExpression(from w in ctx.WayBillInfos.Where(expression)
                                         select new AbnormalWayBillModel
                                             {
                                                 Status = w.Status,
                                                 WayBillNumber = w.WayBillNumber,
                                                 CustomerOrderNumber = w.CustomerOrderNumber,
                                                 CustomerCode = w.CustomerCode,
                                                 IsHold = w.IsHold,
                                                 TrackingNumber = w.TrackingNumber,
                                                 CountryCode = w.CountryCode,
                                                 InShippingMethodName = w.InShippingMethodName,
                                                 AbnormalStatus = w.AbnormalWayBillLog.AbnormalStatus,
                                                 AbnormalCreateOn = w.AbnormalWayBillLog.CreatedOn,
                                                 AbnormalCreateBy = w.AbnormalWayBillLog.CreatedBy,
                                                 AbnormalDescription = w.AbnormalWayBillLog.AbnormalDescription,
                                                 OperateType = w.AbnormalWayBillLog.OperateType
                                             }).ToPagedList(pageIndex, pageSize);
            ctx.Configuration.LazyLoadingEnabled = true;
            return list;
        }

        public List<AbnormalWayBillModel> FindListAbnormalWayBillModel(Expression<Func<WayBillInfo, bool>>
                                                                              expression,
                                                                          Func
                                                                              <IQueryable<AbnormalWayBillModel>,
                                                                              IOrderedQueryable
                                                                              <AbnormalWayBillModel>>
                                                                              orderByExpression)
        {
            var ctx = this.UnitOfWork as LMS_DbContext;
            Check.Argument.IsNotNull(ctx, "数据库对象");
            ctx.Configuration.LazyLoadingEnabled = false;

            var list = orderByExpression(from w in ctx.WayBillInfos.Where(expression)
                                         select new AbnormalWayBillModel
                                             {
                                                 Status = w.Status,
                                                 WayBillNumber = w.WayBillNumber,
                                                 CustomerOrderNumber = w.CustomerOrderNumber,
                                                 CustomerCode = w.CustomerCode,
                                                 IsHold = w.IsHold,
                                                 TrackingNumber = w.TrackingNumber,
                                                 CountryCode = w.CountryCode,
                                                 InShippingMethodName = w.InShippingMethodName,
                                                 AbnormalStatus = w.AbnormalWayBillLog.AbnormalStatus,
                                                 AbnormalCreateOn = w.AbnormalWayBillLog.CreatedOn,
                                                 AbnormalCreateBy = w.AbnormalWayBillLog.CreatedBy,
                                                 AbnormalDescription = w.AbnormalWayBillLog.AbnormalDescription,
                                                 OperateType = w.AbnormalWayBillLog.OperateType
                                             }).ToList();
            ctx.Configuration.LazyLoadingEnabled = true;
            return list;
        }

        /// <summary>
        /// 已发货运单列表
        /// Add By zhengsong
        /// Time:2014-09-13
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public IPagedList<ShippingWayBillExt> GetShippingWayBillPagedList(ShippingWayBillParam param)
        {
            //链接数据库
            var ctx = this.UnitOfWork as LMS_DbContext;
            Check.Argument.IsNotNull(ctx, "数据库对象");
            var delivered = (int) WayBill.StatusEnum.Delivered;
            var send = (int) WayBill.StatusEnum.Send;
            var waitOrder = (int) WayBill.StatusEnum.WaitOrder;

            //写查询条件
            Expression<Func<WayBillInfo, bool>> filterWayBill = o => true;
            Expression<Func<OutStorageInfo, bool>> filterOutStorage = o => true;

            if (!string.IsNullOrWhiteSpace(param.SearchContext))
            {
                var numberList =
                    param.SearchContext.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
                         .ToList();
                if (param.SearchWhere.HasValue && numberList.Count > 0)
                {
                    switch (WayBill.ParseToSearchFilter(param.SearchWhere.Value))
                    {
                        case WayBill.SearchFilterEnum.WayBillNumber:
                            filterWayBill = filterWayBill.And(p => numberList.Contains(p.WayBillNumber));
                            break;
                        case WayBill.SearchFilterEnum.TrackingNumber:
                            filterWayBill = filterWayBill.And(p => numberList.Contains(p.TrackingNumber));
                            break;
                        case WayBill.SearchFilterEnum.CustomerOrderNumber:
                            filterWayBill = filterWayBill.And(p => numberList.Contains(p.CustomerOrderNumber));
                            break;
                        case WayBill.SearchFilterEnum.OutStorageNumber:
                            filterWayBill = filterWayBill.And(p => numberList.Contains(p.OutStorageID));
                            break;
                    }
                }
                filterWayBill =
                    filterWayBill.And(p => p.Status == send || p.Status == waitOrder || p.Status == delivered);
            }
            else
            {
                filterWayBill = filterWayBill.AndIf(r => r.OutStorageCreatedOn >= param.StartTime,
                                                    param.StartTime.HasValue)
                                             .AndIf(r => r.OutStorageCreatedOn <= param.EndTime, param.EndTime.HasValue)
                                             .AndIf(r => r.InShippingMethodID == param.InShippingMehtodId,
                                                    param.InShippingMehtodId.HasValue)
                                             .AndIf(r => r.OutShippingMethodID == param.OutShippingMehtodId,
                                                    param.OutShippingMehtodId.HasValue)
                                             .AndIf(r => r.VenderCode == param.VenderCode,
                                                    !param.VenderCode.IsNullOrWhiteSpace())
                                             .And(
                                                 p => p.Status == send || p.Status == waitOrder || p.Status == delivered);
                filterOutStorage = filterOutStorage.AndIf(r => r.CreatedBy.Contains(param.OutCreateBy),
                                                          !param.OutCreateBy.IsNullOrWhiteSpace());
            }
            //写LinQ 查询
            var list = from w in ctx.WayBillInfos.Where(filterWayBill)
                       join o in ctx.OutStorageInfos.Where(filterOutStorage) on w.OutStorageID equals o.OutStorageID
                       select new ShippingWayBillExt
                           {
                               WayBillNumber = w.WayBillNumber,
                               CustomerOrderNumber = w.CustomerOrderNumber,
                               TrackingNumber = w.TrackingNumber,
                               OutStorageID = w.OutStorageID,
                               VenderName = o.VenderName,
                               InShippingMethodName = w.InShippingMethodName,
                               OutShippingMethodName = w.OutShippingMethodName,
                               CountryCode = w.CountryCode,
                               OutStorageCreatedOn = w.OutStorageCreatedOn,
                               OutStorageCreatedBy = o.CreatedBy,
                               Remark = o.Remark
                           };

            return list.OrderByDescending(p => p.OutStorageCreatedOn).ToPagedList(param.Page, param.PageSize);
        }

        /// <summary>
        /// 已发货运单导出
        /// Add By zhengsong
        /// Time:2014-09-13
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public List<ShippingWayBillExt> GetShippingWayBillList(ShippingWayBillParam param)
        {
            //链接数据库
            var ctx = this.UnitOfWork as LMS_DbContext;
            Check.Argument.IsNotNull(ctx, "数据库对象");
            var delivered = (int) WayBill.StatusEnum.Delivered;
            var send = (int) WayBill.StatusEnum.Send;
            var waitOrder = (int) WayBill.StatusEnum.WaitOrder;

            //写查询条件
            Expression<Func<WayBillInfo, bool>> filterWayBill = o => true;
            Expression<Func<OutStorageInfo, bool>> filterOutStorage = o => true;

            if (!string.IsNullOrWhiteSpace(param.SearchContext))
            {
                var numberList =
                    param.SearchContext.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
                         .ToList();
                if (param.SearchWhere.HasValue && numberList.Count > 0)
                {
                    switch (WayBill.ParseToSearchFilter(param.SearchWhere.Value))
                    {
                        case WayBill.SearchFilterEnum.WayBillNumber:
                            filterWayBill = filterWayBill.And(p => numberList.Contains(p.WayBillNumber));
                            break;
                        case WayBill.SearchFilterEnum.TrackingNumber:
                            filterWayBill = filterWayBill.And(p => numberList.Contains(p.TrackingNumber));
                            break;
                        case WayBill.SearchFilterEnum.CustomerOrderNumber:
                            filterWayBill = filterWayBill.And(p => numberList.Contains(p.CustomerOrderNumber));
                            break;
                        case WayBill.SearchFilterEnum.OutStorageNumber:
                            filterWayBill = filterWayBill.And(p => numberList.Contains(p.OutStorageID));
                            break;
                    }
                }
                filterWayBill =
                    filterWayBill.And(p => p.Status == send || p.Status == waitOrder || p.Status == delivered);
            }
            else
            {
                filterWayBill = filterWayBill.AndIf(r => r.OutStorageCreatedOn >= param.StartTime,
                                                    param.StartTime.HasValue)
                                             .AndIf(r => r.OutStorageCreatedOn <= param.EndTime, param.EndTime.HasValue)
                                             .AndIf(r => r.InShippingMethodID == param.InShippingMehtodId,
                                                    param.InShippingMehtodId.HasValue)
                                             .AndIf(r => r.OutShippingMethodID == param.OutShippingMehtodId,
                                                    param.OutShippingMehtodId.HasValue)
                                             .AndIf(r => r.VenderCode == param.VenderCode,
                                                    !param.VenderCode.IsNullOrWhiteSpace())
                                             .And(
                                                 p => p.Status == send || p.Status == waitOrder || p.Status == delivered);
                filterOutStorage = filterOutStorage.AndIf(r => r.CreatedBy.Contains(param.OutCreateBy),
                                                          !param.OutCreateBy.IsNullOrWhiteSpace());
            }
            //写LinQ 查询
            var list = from w in ctx.WayBillInfos.Where(filterWayBill)
                       join o in ctx.OutStorageInfos.Where(filterOutStorage) on w.OutStorageID equals o.OutStorageID
                       select new ShippingWayBillExt
                       {
                           WayBillNumber = w.WayBillNumber,
                           CustomerOrderNumber = w.CustomerOrderNumber,
                           TrackingNumber = w.TrackingNumber,
                           OutStorageID = w.OutStorageID,
                           VenderName = o.VenderName,
                           InShippingMethodName = w.InShippingMethodName,
                           OutShippingMethodName = w.OutShippingMethodName,
                           CountryCode = w.CountryCode,
                           OutStorageCreatedOn = w.OutStorageCreatedOn,
                           OutStorageCreatedBy = o.CreatedBy,
                           CustomerOrderID = w.CustomerOrderID,
                           Status = w.Status,
                           InsuredID = w.InsuredID,
                           IsReturn = w.IsReturn,
                           SenderInfoID = w.SenderInfoID,
                           ShippingInfoID = w.ShippingInfoID,
                           EnableTariffPrepay = w.EnableTariffPrepay,
                           InStorageCreatedOn = w.InStorageCreatedOn,
                           CreatedOn = w.CreatedOn,
                           SettleWeight = w.SettleWeight,
                           Weight = w.Weight,
                           Length = w.Length,
                           Width = w.Width,
                           Height = w.Height
                       };

            return list.ToList();
        }

        //根据条件查询所有可以修改出仓方式的运单
        //Add By zhengsong /2014-09-13
        public string GetAllShippingWayBillList(ShippingWayBillParam param)
        {
            //链接数据库
            var ctx = this.UnitOfWork as LMS_DbContext;
            Check.Argument.IsNotNull(ctx, "数据库对象");
            var delivered = (int) WayBill.StatusEnum.Delivered;
            var send = (int) WayBill.StatusEnum.Send;
            var waitOrder = (int) WayBill.StatusEnum.WaitOrder;
            //写查询条件
            Expression<Func<WayBillInfo, bool>> filterWayBill = o => true;
            Expression<Func<OutStorageInfo, bool>> filterOutStorage = o => true;

            if (!string.IsNullOrWhiteSpace(param.SearchContext))
            {
                var numberList =
                    param.SearchContext.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
                         .ToList();
                if (param.SearchWhere.HasValue && numberList.Count > 0)
                {
                    switch (WayBill.ParseToSearchFilter(param.SearchWhere.Value))
                    {
                        case WayBill.SearchFilterEnum.WayBillNumber:
                            filterWayBill = filterWayBill.And(p => numberList.Contains(p.WayBillNumber));
                            break;
                        case WayBill.SearchFilterEnum.TrackingNumber:
                            filterWayBill = filterWayBill.And(p => numberList.Contains(p.TrackingNumber));
                            break;
                        case WayBill.SearchFilterEnum.CustomerOrderNumber:
                            filterWayBill = filterWayBill.And(p => numberList.Contains(p.CustomerOrderNumber));
                            break;
                        case WayBill.SearchFilterEnum.OutStorageNumber:
                            filterWayBill = filterWayBill.And(p => numberList.Contains(p.OutStorageID));
                            break;
                    }
                }
                filterWayBill =
                    filterWayBill.And(p => p.Status == send || p.Status == waitOrder || p.Status == delivered);
            }
            else
            {
                filterWayBill = filterWayBill.AndIf(r => r.OutStorageCreatedOn >= param.StartTime,
                                                    param.StartTime.HasValue)
                                             .AndIf(r => r.OutStorageCreatedOn <= param.EndTime, param.EndTime.HasValue)
                                             .AndIf(r => r.InShippingMethodID == param.InShippingMehtodId,
                                                    param.InShippingMehtodId.HasValue)
                                             .AndIf(r => r.OutShippingMethodID == param.OutShippingMehtodId,
                                                    param.OutShippingMehtodId.HasValue)
                                             .AndIf(r => r.VenderCode == param.VenderCode,
                                                    !param.VenderCode.IsNullOrWhiteSpace())
                                             .And(
                                                 p => p.Status == send || p.Status == waitOrder || p.Status == delivered);
                filterOutStorage = filterOutStorage.AndIf(r => r.CreatedBy.Contains(param.OutCreateBy),
                                                          !param.OutCreateBy.IsNullOrWhiteSpace());
            }
            //写LinQ 查询
            var list = from w in ctx.WayBillInfos.Where(filterWayBill)
                       join o in ctx.OutStorageInfos.Where(filterOutStorage) on w.OutStorageID equals o.OutStorageID
                           into outs
                       from oo in outs.DefaultIfEmpty()
                       join cc in ctx.DeliveryFees on w.WayBillNumber equals cc.WayBillNumber into rec
                       from cp in rec.DefaultIfEmpty()
                       join p in ctx.DeliveryFeeInfos on cp.DeliveryFeeID equals p.DeliveryFeeInfoID
                           into custPurchases
                       where w.OutStorageID != null && !custPurchases.Any()
                       select new ShippingWayBillExt
                       {
                           WayBillNumber = w.WayBillNumber
                       };
            string waybillList = "";
            list.ToList().ForEach(p =>
                {
                    waybillList += p.WayBillNumber + ",";
                });
            return waybillList;
        }

        public string GetIsUpdateShippingWayBillList(List<string> wayBillList)
        {
            //链接数据库
            var ctx = this.UnitOfWork as LMS_DbContext;
            Check.Argument.IsNotNull(ctx, "数据库对象");
            //写查询条件
            Expression<Func<WayBillInfo, bool>> filterWayBill = o => true;

            filterWayBill = filterWayBill.And(p => wayBillList.Contains(p.WayBillNumber));

            //写LinQ 查询
            var list = from w in ctx.WayBillInfos.Where(filterWayBill)
                       join o in ctx.OutStorageInfos on w.OutStorageID equals o.OutStorageID into outs
                       from oo in outs.DefaultIfEmpty()
                       join cc in ctx.DeliveryFees on w.WayBillNumber equals cc.WayBillNumber into rec
                       from cp in rec.DefaultIfEmpty()
                       join p in ctx.DeliveryFeeInfos on cp.DeliveryFeeID equals p.DeliveryFeeInfoID
                           into custPurchases
                       where w.OutStorageID != null && !custPurchases.Any()
                       select new ShippingWayBillExt
                           {
                               WayBillNumber = w.WayBillNumber
                           };
            string waybillList = "";
            list.ToList().ForEach(p =>
                {
                    waybillList += p.WayBillNumber + ",";
                });
            return waybillList;
        }

        public IList<WayBillListExportModel> GetWayBillListExport(WayBillListExportParam param)
        {
            var ctx = this.UnitOfWork as LMS_DbContext;
            Check.Argument.IsNotNull(ctx, "数据库对象");

            var CustomerCode = new SqlParameter
                {
                    ParameterName = "CustomerCode",
                    Value = param.CustomerCode,
                    DbType = DbType.String
                };
            var ShippingMethodId = new SqlParameter
                {
                    ParameterName = "ShippingMethodId",
                    Value = param.ShippingMethodId,
                    DbType = DbType.Int32
                };
            var DateWhere = new SqlParameter
                {
                    ParameterName = "DateWhere",
                    Value = param.DateWhere,
                    DbType = DbType.Int32
                };
            var StartTime = new SqlParameter
                {
                    ParameterName = "StartTime",
                    Value = param.StartTime ?? new DateTime(2013, 1, 1),
                    DbType = DbType.Time
                };
            var EndTime = new SqlParameter
                {
                    ParameterName = "EndTime",
                    Value = param.EndTime ?? new DateTime(2020, 1, 1),
                    DbType = DbType.Time
                };
            var CountryCode = new SqlParameter
                {
                    ParameterName = "CountryCode",
                    Value = param.CountryCode,
                    DbType = DbType.String
                };
            var SearchWhere = new SqlParameter
                {
                    ParameterName = "SearchWhere",
                    Value = param.SearchWhere,
                    DbType = DbType.Int32
                };
            var SearchContext = new SqlParameter
                {
                    ParameterName = "SearchContext",
                    Value =
                        param.SearchContext.IsNullOrWhiteSpace()
                            ? (object) DBNull.Value
                            : string.Join(",",
                                          param.SearchContext.Split(Environment.NewLine.ToCharArray(),
                                                                    StringSplitOptions.RemoveEmptyEntries).Distinct()),
                    DbType = DbType.String
                };
            var Status = new SqlParameter
                {
                    ParameterName = "Status",
                    Value =
                        param.Status.IsNullOrWhiteSpace()
                            ? (object) DBNull.Value
                            : string.Join(",",
                                          param.Status.Split(new char[] {';'}, StringSplitOptions.RemoveEmptyEntries)
                                               .Distinct()),
                    DbType = DbType.String
                };
            var IsOutHold = new SqlParameter
                {
                    ParameterName = "IsOutHold",
                    Value = param.IsOutHold,
                    DbType = DbType.Boolean
                };
            var IsApplicationInfo = new SqlParameter
                {
                    ParameterName = "IsApplicationInfo",
                    Value = true,
                    DbType = DbType.Boolean
                };
            if (ctx != null)
            {
                var obj =
                    ctx.ExecuteStoredProcedureList<WayBillListExportModel>(
                        "P_WayBillInfos_Export @CustomerCode,@ShippingMethodId,@CountryCode,@DateWhere,@StartTime,@EndTime,@SearchWhere,@SearchContext,@Status,@IsOutHold,@IsApplicationInfo"
                        , CustomerCode, ShippingMethodId, CountryCode, DateWhere, StartTime, EndTime, SearchWhere,
                        SearchContext, Status, IsOutHold, IsApplicationInfo);
                return obj;
            }
            return null;
        }

        public IList<ApplicationInfoExportModel> GetApplicationInfoExport(WayBillListExportParam param)
        {
            var ctx = this.UnitOfWork as LMS_DbContext;
            Check.Argument.IsNotNull(ctx, "数据库对象");
            var CustomerCode = new SqlParameter
                {
                    ParameterName = "CustomerCode",
                    Value = param.CustomerCode,
                    DbType = DbType.String
                };
            var ShippingMethodId = new SqlParameter
                {
                    ParameterName = "ShippingMethodId",
                    Value = param.ShippingMethodId,
                    DbType = DbType.Int32
                };
            var DateWhere = new SqlParameter
                {
                    ParameterName = "DateWhere",
                    Value = param.DateWhere,
                    DbType = DbType.Int32
                };
            var StartTime = new SqlParameter
                {
                    ParameterName = "StartTime",
                    Value = param.StartTime ?? new DateTime(2013, 1, 1),
                    DbType = DbType.Time
                };
            var EndTime = new SqlParameter
                {
                    ParameterName = "EndTime",
                    Value = param.EndTime ?? new DateTime(2020, 1, 1),
                    DbType = DbType.Time
                };
            var CountryCode = new SqlParameter
                {
                    ParameterName = "CountryCode",
                    Value = param.CountryCode,
                    DbType = DbType.String
                };
            var SearchWhere = new SqlParameter
                {
                    ParameterName = "SearchWhere",
                    Value = param.SearchWhere,
                    DbType = DbType.Int32
                };
            var SearchContext = new SqlParameter
                {
                    ParameterName = "SearchContext",
                    Value =
                        param.SearchContext.IsNullOrWhiteSpace()
                            ? (object) DBNull.Value
                            : string.Join(",",
                                          param.SearchContext.Split(Environment.NewLine.ToCharArray(),
                                                                    StringSplitOptions.RemoveEmptyEntries).Distinct()),
                    DbType = DbType.String
                };
            var Status = new SqlParameter
                {
                    ParameterName = "Status",
                    Value =
                        param.Status.IsNullOrWhiteSpace()
                            ? (object) DBNull.Value
                            : string.Join(",",
                                          param.Status.Split(new char[] {';'}, StringSplitOptions.RemoveEmptyEntries)
                                               .Distinct()),
                    DbType = DbType.String
                };
            var IsOutHold = new SqlParameter
                {
                    ParameterName = "IsOutHold",
                    Value = param.IsOutHold,
                    DbType = DbType.Boolean
                };
            var IsApplicationInfo = new SqlParameter
                {
                    ParameterName = "IsApplicationInfo",
                    Value = false,
                    DbType = DbType.Boolean
                };
            if (ctx != null)
            {
                var obj =
                    ctx.ExecuteStoredProcedureList<ApplicationInfoExportModel>(
                        "P_WayBillInfos_Export @CustomerCode,@ShippingMethodId,@CountryCode,@DateWhere,@StartTime,@EndTime,@SearchWhere,@SearchContext,@Status,@IsOutHold,@IsApplicationInfo"
                        , CustomerCode, ShippingMethodId, CountryCode, DateWhere, StartTime, EndTime, SearchWhere,
                        SearchContext, Status, IsOutHold, IsApplicationInfo);
                return obj;
            }
            return null;
        }

        ///// <summary>
        ///// 查出福州邮政的运单
        ///// Add By zhengsong
        ///// Time:2014-11-05
        ///// </summary>
        ///// <param name="outShippingMethodId"></param>
        ///// <param name="wayBillNumbers"></param>
        ///// <returns></returns>
        //public List<FZWayBillInfoExt> GetFuZhouWayBillList(List<int> outShippingMethodId,List<string> wayBillNumbers)
        //{
        //    var ctx = this.UnitOfWork as LMS_DbContext;
        //    Check.Argument.IsNotNull(ctx, "数据库对象");
        //    var send = (int)WayBill.StatusEnum.Send;
        //    var waitOrder = (int)WayBill.StatusEnum.WaitOrder;
        //    DateTime stime = new DateTime();
        //    DateTime.TryParse(sysConfig.PostalTime, out stime);
        //    Expression<Func<WayBillInfo, bool>> filter = p => true;
        //    string testCode = "C02108,C33352,C02234,C64994,C13247,C36898,C06901,C20834,C87708,C00513,C90712,C57081";
        //    //查询出提交成功的运单
        //    //var FuzhouPostLogList = from w in ctx.FuzhouPostLogs.Where(fuzhoufilter)
        //    //                        select w;
        //    //List<string> wayBillList = new List<string>();
        //    //FuzhouPostLogList.ToList().ForEach(p => wayBillList.Add(p.WayBillNumber));

        //    //查询出未提交成功的运单
        //    filter = filter.And(p => p.Status == send || p.Status == waitOrder)
        //                   .And(p => p.OutStorageCreatedOn > stime)
        //                   .And(p=> !testCode.Contains(p.CustomerCode))
        //                   .And(p => outShippingMethodId.Contains(p.OutShippingMethodID.Value))
        //                   .AndIf(p => wayBillNumbers.Contains(p.WayBillNumber), wayBillNumbers.Count > 0);

        //    var list = from w in ctx.WayBillInfos.Where(filter)
        //               select new FZWayBillInfoExt
        //                   {
        //                       WayBillNumber = w.WayBillNumber,
        //                       CustomerOrderNumber = w.CustomerOrderNumber,
        //                       CustomerOrderID = w.CustomerOrderID,
        //                       TrackingNumber = w.TrackingNumber,
        //                       Weight = w.Weight,
        //                       SettleWeight = w.SettleWeight,
        //                       IsReturn = w.IsReturn,
        //                       IsBattery = w.IsBattery,
        //                       Status = w.Status,
        //                       OutStorageID = w.OutStorageID,
        //                       OutStorageCreatedOn = w.OutStorageCreatedOn,
        //                       CountryCode = w.CountryCode,
        //                       InsuredID = w.InsuredID,
        //                       OutShippingMethodID = w.OutShippingMethodID,
        //                       SenderInfo = w.SenderInfo,
        //                       ShippingInfo = w.ShippingInfo
        //                   };
        //    return list.ToList();
        //}

        /// <summary>
        /// 查出福州邮政的运单
        /// Add By zhengsong
        /// Time:2014-11-05
        /// </summary>
        /// <param name="outShippingMethodId"></param>
        /// <param name="number"></param>
        /// <returns></returns>
        public List<FZWayBillInfoExt> GetFuZhouWayBillNumbers(List<int> outShippingMethodId,int number)
        {
            var ctx = this.UnitOfWork as LMS_DbContext;
            Check.Argument.IsNotNull(ctx, "数据库对象");
            var send = (int) WayBill.StatusEnum.Send;
            var waitOrder = (int) WayBill.StatusEnum.WaitOrder;

            DateTime stime = DateTime.Now;
            DateTime.TryParse("2014-12-13", out stime);
            //排除测试单号
            string testCode = "C02108,C33352,C02234,C64994,C13247,C36898,C06901,C20834,C87708,C00513,C90712,C57081";
            Expression<Func<WayBillInfo, bool>> filter = p => true;
            Expression<Func<FuzhouPostLog, bool>> fuzhoufilter = p => true;
            fuzhoufilter = fuzhoufilter.And(z => z.Status == 1);
            //查询出提交成功的运单
            //var FuzhouPostLogList = from w in ctx.FuzhouPostLogs.Where(fuzhoufilter)
            //                        select w;
            //List<string> wayBillList = new List<string>();
            //FuzhouPostLogList.ToList().ForEach(p => wayBillList.Add(p.WayBillNumber));

            var success = ctx.FuzhouPostLogs.Where(fuzhoufilter).Select(w => w.WayBillNumber);

            //查询出未提交成功的运单
            filter = filter.And(p => p.Status == send || p.Status == waitOrder)
                           .And(p => p.OutStorageCreatedOn >= stime)
                           .And(p => !testCode.Contains(p.CustomerCode))
                           .And(p => outShippingMethodId.Contains(p.OutShippingMethodID.Value))
                           .And(p => !success.Contains(p.WayBillNumber));



            var list = (from w in ctx.WayBillInfos.Where(filter)
                        //join f in ctx.FuzhouPostLogs.Where(fuzhoufilter) on w.WayBillNumber equals f.WayBillNumber
                        orderby w.OutStorageCreatedOn
                       select new FZWayBillInfoExt
                           {
                               WayBillNumber = w.WayBillNumber,
                               CustomerOrderNumber = w.CustomerOrderNumber,
                               CustomerOrderID = w.CustomerOrderID,
                               TrackingNumber = w.TrackingNumber,
                               Weight = w.Weight,
                               SettleWeight = w.SettleWeight,
                               IsReturn = w.IsReturn,
                               IsBattery = w.IsBattery,
                               Status = w.Status,
                               OutStorageID = w.OutStorageID,
                               OutStorageCreatedOn = w.OutStorageCreatedOn,
                               CountryCode = w.CountryCode,
                               InsuredID = w.InsuredID,
                               OutShippingMethodID = w.OutShippingMethodID,
                                SenderInfoID = w.SenderInfoID,
                                ShippingInfoID = w.ShippingInfoID
                            }).Skip(number).Take(50).ToList();
            List<int> SenderInfoIds=new List<int>();
            List<int> ShippingInfoIds = new List<int>();
            list.ForEach(p =>
                {
                    if (p.SenderInfoID.HasValue)
                    {
                        SenderInfoIds.Add(p.SenderInfoID.Value);
        }
                    if (p.ShippingInfoID.HasValue)
                    {
                        ShippingInfoIds.Add(p.ShippingInfoID.Value);
                    }
                });
        
            var senderlist = (ctx.SenderInfos.Where(p => SenderInfoIds.Contains(p.SenderInfoID))
                                 .Select(s => new SenderInfoModelExt
        {
                                         SenderInfoID = s.SenderInfoID,
                                         CountryCode = s.CountryCode,
                                         SenderFirstName = s.SenderFirstName,
                                         SenderLastName = s.SenderLastName,
                                         SenderCompany = s.SenderCompany,
                                         SenderAddress = s.SenderAddress,
                                         SenderCity = s.SenderCity,
                                         SenderState = s.SenderState,
                                         SenderZip = s.SenderZip,
                                         SenderPhone = s.SenderPhone
                                     })).ToDictionary(p => p.SenderInfoID);

            var shippinglist = (ctx.ShippingInfos.Where(p => ShippingInfoIds.Contains(p.ShippingInfoID))
                                   .Select(s => new ShippingInfoModelExt
                                       {
                                           ShippingInfoID = s.ShippingInfoID,
                                           CountryCode = s.CountryCode,
                                           ShippingFirstName = s.ShippingFirstName,
                                           ShippingLastName = s.ShippingLastName,
                                           ShippingCompany = s.ShippingCompany,
                                           ShippingAddress = s.ShippingAddress,
                                           ShippingAddress1 = s.ShippingAddress1,
                                           ShippingAddress2 = s.ShippingAddress2,
                                           ShippingCity = s.ShippingCity,
                                           ShippingState = s.ShippingState,
                                           ShippingZip = s.ShippingZip,
                                           ShippingPhone = s.ShippingPhone,
                                           ShippingTaxId = s.ShippingTaxId
                                       })).ToDictionary(p => p.ShippingInfoID);

            foreach (var fzWayBillInfoExt in list)
                       {
                if (fzWayBillInfoExt.SenderInfoID.HasValue)
                {
                    fzWayBillInfoExt.SenderInfo = senderlist[fzWayBillInfoExt.SenderInfoID.Value];
        }
                if (fzWayBillInfoExt.ShippingInfoID.HasValue)
                {
                    fzWayBillInfoExt.ShippingInfo = shippinglist[fzWayBillInfoExt.ShippingInfoID.Value];
                }
            }
            return list;
        }

        /// <summary>
        /// 获取需要预报信息的DHL和EUB运单
        /// </summary>
        /// <param name="intShippingMethodIds"></param>
        /// <param name="number">每次取number之后的运单</param>
        /// <returns></returns>
        public List<WayBillInfo> GetDHLandEUBWayBillInfos(List<int> intShippingMethodIds, int number)
        {
            var ctx = this.UnitOfWork as LMS_DbContext;
            Check.Argument.IsNotNull(ctx, "数据库对象");
            var send = (int)WayBill.StatusEnum.Submitted;
            DateTime stime = DateTime.Now;
            DateTime.TryParse(sysConfig.EUBDHLPostal_Time, out stime);
            //排除测试单号
            string testCode = "C02108,C33352,C02234,C64994,C13247,C36898,C06901,C20834,C87708,C00513,C90712,C57081";
            Expression<Func<WayBillInfo, bool>> filter = p => true;
            filter = filter.And(p => p.Status == send)
                           .And(p => p.CreatedOn >= stime)
                           .And(p => !testCode.Contains(p.CustomerCode))
                           .And(p => p.IsHold==false)
                           .And(p => intShippingMethodIds.Contains(p.InShippingMethodID.Value))
                           .And(p => p.TrackingNumber==null || p.TrackingNumber=="");

            var list = (from w in ctx.WayBillInfos.Where(filter)
                        orderby w.CreatedOn
                        select w).Skip(number).Take(50).ToList();
            return list;
        }


        public List<InStorageProcess> GetInStorageProcess(List<string> inStorageIDs)
        {
            if (inStorageIDs.Count == 0)
            {
                return new List<InStorageProcess>();
            }

            var ctx = this.UnitOfWork as LMS_DbContext;
            Check.Argument.IsNotNull(ctx, "数据库对象");

            string query = @"
SELECT  InStorageID,(SELECT count(0) FROM dbo.WayBillInfos WHERE  [Status] <> 350 And InStorageID = w.InStorageID) Completed ,COUNT(0) Total
FROM   dbo.WayBillInfos w WHERE InStorageID IN ({0}) GROUP BY InStorageID";

            StringBuilder sbIds = new StringBuilder();

            inStorageIDs.ForEach(t => sbIds.AppendFormat("'{0}',", t));
            query = string.Format(query, sbIds.ToString().TrimEnd(','));

            var process = ctx.Database.SqlQuery<InStorageProcess>(query);

            return process.ToList();
        }
    }
}

