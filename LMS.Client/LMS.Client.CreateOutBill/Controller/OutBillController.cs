using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DbHelper;
using LMS.Client.CreateOutBill.Model;
using LighTake.Infrastructure.Common;
using LighTake.Infrastructure.Common.Logging;
using System.Transactions;

namespace LMS.Client.CreateOutBill.Controller
{
    public class OutBillController
    {
        private static readonly string _lmsCon = System.Configuration.ConfigurationManager.AppSettings["LMSCon"].ToString();
        private static readonly string _lmsfinCon = System.Configuration.ConfigurationManager.AppSettings["LMSFinancialCon"].ToString();
        /// <summary>
        /// 获取需要生成出账单的账单号
        /// </summary>
        /// <returns></returns>
        public static List<ReceivingBill> GetCreateOutBillList()
        {
            DbUtility dbUtility = new SqlDbUtility(_lmsCon);
            DataTable dt = dbUtility.ExecuteData("select ReceivingBillID,CustomerCode,CustomerName,ReceivingBillDate,ReceivingBillAuditor,BillStartTime,BillEndTime,ISNULL(Search,0) Search from ReceivingBills where Status=1");
            var list = new List<ReceivingBill>();
            if (dt != null && dt.Rows.Count > 0)
            {
                Log.Info("LMS需要生成账单的账单号总数是" + dt.Rows.Count);
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    var v = new ReceivingBill();
                    for (int j = 0; j < dt.Columns.Count; j++)
                    {
                        switch (dt.Columns[j].ColumnName)
                        {
                            case "ReceivingBillID":
                                v.ReceivingBillID = dt.Rows[i][j].ToString();
                                break;
                            case "CustomerCode":
                                v.CustomerCode = dt.Rows[i][j].ToString();
                                break;
                            case "CustomerName":
                                v.CustomerName = dt.Rows[i][j].ToString();
                                break;
                            case "ReceivingBillDate":
                                v.ReceivingBillDate = dt.Rows[i][j].ToString();
                                break;
                            case "ReceivingBillAuditor":
                                v.ReceivingBillAuditor = dt.Rows[i][j].ToString();
                                break;
                            case "BillStartTime":
                                v.BillStartTime = dt.Rows[i][j].ToString();
                                break;
                            case "BillEndTime":
                                v.BillEndTime = dt.Rows[i][j].ToString();
                                break;
                            case "Search":
                                v.Search = Int32.Parse(dt.Rows[i][j].ToString());
                                break;
                        }
                    }
                    list.Add(v);
                }
            }
            Log.Info("LMS需要生成账单的账单号获取完毕！");
            return list;
        }
        /// <summary>
        /// 根据出账单号获取账单信息
        /// </summary>
        /// <param name="receivingBillId"></param>
        /// <returns></returns>
        public static List<BillModel> GetExportReceivingBill(string receivingBillId)
        {
            Log.Info("LMS开始获取账单号为:{0}的账单信息".FormatWith(receivingBillId));
            DbUtility dbUtility = new SqlDbUtility(_lmsCon);
            DataTable dt = dbUtility.ExecuteData("exec P_ExportReceivingBill N'" + receivingBillId + "'");
            var list = new List<BillModel>();
            if (dt != null && dt.Rows.Count > 0)
            {
                Log.Info("LMS开始获取账单号为:{0}的账单信息总数是{1}".FormatWith(receivingBillId, dt.Rows.Count));
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    var v = new BillModel();
                    for (int j = 0; j < dt.Columns.Count; j++)
                    {
                        switch (dt.Columns[j].ColumnName)
                        {
                            case "ReceivingExpenseId":
                                v.ReceivingExpenseId = Int32.Parse(dt.Rows[i][j].ToString());
                                break;
                            case "CustomerOrderNumber":
                                v.CustomerOrderNumber = dt.Rows[i][j].ToString();
                                break;
                            case "WayBillNumber":
                                v.WayBillNumber = dt.Rows[i][j].ToString();
                                break;
                            case "CreatedOn":
                                v.CreatedOn = DateTime.Parse(dt.Rows[i][j].ToString());
                                break;
                            case "InStorageCreatedOn":
                                v.InStorageCreatedOn = DateTime.Parse(dt.Rows[i][j].ToString());
                                break;
                            case "TrackingNumber":
                                v.TrackingNumber = dt.Rows[i][j].ToString();
                                break;
                            case "ChineseName":
                                v.ChineseName = dt.Rows[i][j].ToString();
                                break;
                            case "InShippingMethodName":
                                v.InShippingMethodName = dt.Rows[i][j].ToString();
                                break;
                            case "InShippingMethodId":
                                v.InShippingMethodId = Int32.Parse(dt.Rows[i][j].ToString());
                                break;
                            case "InStorageID":
                                v.InStorageID = dt.Rows[i][j].ToString();
                                break;
                            case "SettleWeight":
                                v.SettleWeight = decimal.Parse(dt.Rows[i][j].ToString());
                                break;
                            case "Weight":
                                v.Weight = decimal.Parse(dt.Rows[i][j].ToString());
                                break;
                            case "CountNumber":
                                v.CountNumber = Int32.Parse(dt.Rows[i][j].ToString());
                                break;
                            case "Freight":
                                v.Freight = decimal.Parse(dt.Rows[i][j].ToString());
                                break;
                            case "FuelCharge":
                                v.FuelCharge = decimal.Parse(dt.Rows[i][j].ToString());
                                break;
                            case "Register":
                                v.Register = decimal.Parse(dt.Rows[i][j].ToString());
                                break;
                            case "Surcharge":
                                v.Surcharge = decimal.Parse(dt.Rows[i][j].ToString());
                                break;
                            case "TariffPrepayFee":
                                v.TariffPrepayFee = decimal.Parse(dt.Rows[i][j].ToString());
                                break;
                            case "SpecialFee":
                                v.SpecialFee = decimal.Parse(dt.Rows[i][j].ToString());
                                break;
                            case "RemoteAreaFee":
                                v.RemoteAreaFee = decimal.Parse(dt.Rows[i][j].ToString());
                                break;
                        }
                    }
                    list.Add(v);
                }
            }
            Log.Info("LMS完成获取账单号为:{0}的账单信息".FormatWith(receivingBillId));
            return list;
        }
        /// <summary>
        /// 插入备份数据库
        /// </summary>
        /// <param name="bill"></param>
        /// <param name="list"></param>
        /// <returns></returns>
        public static bool InsertBackUp(ReceivingBill bill, List<BillModel> list)
        {
            var result = false;
            Log.Info("LMS开始插入备份数据库账单号为:{0}".FormatWith(bill.ReceivingBillID));
            const string insertReceivingBills = @"INSERT INTO [LMS_DB_Financial].[dbo].[ReceivingBills]
           ([ReceivingBillID],[CustomerCode],[CustomerName]
           ,[ReceivingBillDate],[ReceivingBillAuditor],[BillStartTime],[BillEndTime]) VALUES({0},{1},{2},{3},{4},{5},{6})";
            const string insertReceivingBillInfos = @"INSERT INTO [ReceivingBillInfos]
           ([WayBillNumber],[ReceivingBillID],[CustomerOrderNumber],[CustomerName]
           ,[CustomerCode],[ReceivingDate],[TrackingNumber],[CountryName]
           ,[ShippingMethodName],[SettleWeight],[Weight],[PackageNumber]
           ,[FeeDetail],[TotalFee])
           VALUES({0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13})";
            const string xml = "<FeeDetails><Freight>{0}</Freight><FuelCharge>{1}</FuelCharge><Register>{2}</Register><Surcharge>{3}</Surcharge><TariffPrepayFee>{4}</TariffPrepayFee><SpecialFee>{5}</SpecialFee><RemoteAreaFee>{6}</RemoteAreaFee></FeeDetails>";
            try
            {
                DbUtility dbUtility = new SqlDbUtility(_lmsfinCon);
                using (var transaction = new TransactionScope())
                {
                    dbUtility.ExecuteNonQuery(insertReceivingBills, bill.ReceivingBillID, bill.CustomerCode,
                                              bill.CustomerName, bill.ReceivingBillDate, bill.ReceivingBillAuditor,
                                              bill.BillStartTime, bill.BillEndTime);
                    list.ForEach(
                        p => dbUtility.ExecuteNonQuery(insertReceivingBillInfos, p.WayBillNumber,
                                                       bill.ReceivingBillID,
                                                       p.CustomerOrderNumber, bill.CustomerName, bill.CustomerCode,
                                                       p.InStorageCreatedOn, p.TrackingNumber, p.ChineseName,
                                                       p.InShippingMethodName, p.SettleWeight, p.Weight,
                                                       p.CountNumber,
                                                       xml.FormatWith(p.Freight, p.FuelCharge, p.Register,
                                                                      p.Surcharge, p.TariffPrepayFee, p.SpecialFee, p.RemoteAreaFee),
                                                       p.Freight + p.FuelCharge + p.Register + p.Surcharge +
                                                       p.TariffPrepayFee + p.SpecialFee));
                    transaction.Complete();
                }
                result = true;
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
            Log.Info("LMS完成插入备份数据库账单号为:{0}".FormatWith(bill.ReceivingBillID));
            return result;
        }
        /// <summary>
        /// 插入客户资金记录表
        /// </summary>
        /// <param name="bill"></param>
        /// <param name="list"></param>
        /// <returns></returns>
        public static bool CreateCustomerAmountRecords(ReceivingBill bill, List<BillModel> list)
        {
            var result = false;
            Log.Info("LMS开始插入客户资金记录客户为：{1}，账单号为:{0}".FormatWith(bill.ReceivingBillID, bill.CustomerCode));
            try
            {
                DbUtility dbUtility = new SqlDbUtility(_lmsCon);
                using (var transaction = new TransactionScope())
                {
                    list.ForEach(p =>
                        {
                            object obj;
                            if (p.Freight > 0)
                            {
                                obj =
                                    dbUtility.ExecuteScalar(
                                        "Exec P_JobCustomerAmountRecord {0},{1},{2},{3},{4},{5},{6},{7}",
                                        bill.CustomerCode,
                                        p.WayBillNumber, p.InStorageID, 2, 3, p.Freight,
                                        "运单号：{0}扣运费".FormatWith(p.WayBillNumber), "system");
                                if (obj.ToString() != "1")
                                {
                                    throw new ArgumentException("该运单号\"{0}\"生成运费扣费记录失败！".FormatWith(p.WayBillNumber));
                                }
                            }
                            else if (p.Freight < 0)
                            {
                                obj =
                                    dbUtility.ExecuteScalar(
                                        "Exec P_JobCustomerAmountRecord {0},{1},{2},{3},{4},{5},{6},{7}",
                                        bill.CustomerCode,
                                        p.WayBillNumber, p.InStorageID, 3, 3, p.Freight * -1,
                                        "运单号：{0}退运费".FormatWith(p.WayBillNumber), "system");
                                if (obj.ToString() != "1")
                                {
                                    throw new ArgumentException("该运单号\"{0}\"生成运费退费记录失败！".FormatWith(p.WayBillNumber));
                                }
                            }
                            if (p.Register > 0)
                            {
                                obj =
                                    dbUtility.ExecuteScalar(
                                        "Exec P_JobCustomerAmountRecord {0},{1},{2},{3},{4},{5},{6},{7}",
                                        bill.CustomerCode,
                                        p.WayBillNumber, p.InStorageID, 2, 4, p.Register,
                                        "运单号：{0}扣挂号费".FormatWith(p.WayBillNumber), "system");
                                if (obj.ToString() != "1")
                                {
                                    throw new ArgumentException("该运单号\"{0}\"生成挂号费扣费记录失败！".FormatWith(p.WayBillNumber));
                                }
                            }
                            else if (p.Register < 0)
                            {
                                obj =
                                    dbUtility.ExecuteScalar(
                                        "Exec P_JobCustomerAmountRecord {0},{1},{2},{3},{4},{5},{6},{7}",
                                        bill.CustomerCode,
                                        p.WayBillNumber, p.InStorageID, 3, 4, p.Register * -1,
                                        "运单号：{0}退挂号费".FormatWith(p.WayBillNumber), "system");
                                if (obj.ToString() != "1")
                                {
                                    throw new ArgumentException("该运单号\"{0}\"生成挂号费退费记录失败！".FormatWith(p.WayBillNumber));
                                }
                            }
                            if (p.FuelCharge > 0)
                            {
                                obj =
                                    dbUtility.ExecuteScalar(
                                        "Exec P_JobCustomerAmountRecord {0},{1},{2},{3},{4},{5},{6},{7}",
                                        bill.CustomerCode,
                                        p.WayBillNumber, p.InStorageID, 2, 5, p.FuelCharge,
                                        "运单号：{0}扣燃油费".FormatWith(p.WayBillNumber), "system");
                                if (obj.ToString() != "1")
                                {
                                    throw new ArgumentException("该运单号\"{0}\"生成燃油费扣费记录失败！".FormatWith(p.WayBillNumber));
                                }
                            }
                            else if (p.FuelCharge < 0)
                            {
                                obj =
                                    dbUtility.ExecuteScalar(
                                        "Exec P_JobCustomerAmountRecord {0},{1},{2},{3},{4},{5},{6},{7}",
                                        bill.CustomerCode,
                                        p.WayBillNumber, p.InStorageID, 3, 5, p.FuelCharge * -1,
                                        "运单号：{0}退燃油费".FormatWith(p.WayBillNumber), "system");
                                if (obj.ToString() != "1")
                                {
                                    throw new ArgumentException("该运单号\"{0}\"生成燃油费退费记录失败！".FormatWith(p.WayBillNumber));
                                }
                            }
                            if (p.Surcharge + p.SpecialFee > 0)
                            {
                                obj =
                                    dbUtility.ExecuteScalar(
                                        "Exec P_JobCustomerAmountRecord {0},{1},{2},{3},{4},{5},{6},{7}",
                                        bill.CustomerCode,
                                        p.WayBillNumber, p.InStorageID, 2, 2, p.Surcharge + p.SpecialFee,
                                        "运单号：{0}扣附加费".FormatWith(p.WayBillNumber), "system");
                                if (obj.ToString() != "1")
                                {
                                    throw new ArgumentException("该运单号\"{0}\"生成附加费扣费记录失败！".FormatWith(p.WayBillNumber));
                                }
                            }
                            else if (p.Surcharge + p.SpecialFee < 0)
                            {
                                obj =
                                    dbUtility.ExecuteScalar(
                                        "Exec P_JobCustomerAmountRecord {0},{1},{2},{3},{4},{5},{6},{7}",
                                        bill.CustomerCode,
                                        p.WayBillNumber, p.InStorageID, 3, 2, (p.Surcharge + p.SpecialFee) * -1,
                                        "运单号：{0}退附加费".FormatWith(p.WayBillNumber), "system");
                                if (obj.ToString() != "1")
                                {
                                    throw new ArgumentException("该运单号\"{0}\"生成附加费退费记录失败！".FormatWith(p.WayBillNumber));
                                }
                            }
                            if (p.TariffPrepayFee > 0)
                            {
                                obj =
                                    dbUtility.ExecuteScalar(
                                        "Exec P_JobCustomerAmountRecord {0},{1},{2},{3},{4},{5},{6},{7}",
                                        bill.CustomerCode,
                                        p.WayBillNumber, p.InStorageID, 2, 6, p.TariffPrepayFee,
                                        "运单号：{0}扣关税预付服务费".FormatWith(p.WayBillNumber), "system");
                                if (obj.ToString() != "1")
                                {
                                    throw new ArgumentException("该运单号\"{0}\"生成关税预付服务费扣费记录失败！".FormatWith(p.WayBillNumber));
                                }
                            }
                            else if (p.TariffPrepayFee < 0)
                            {
                                obj =
                                    dbUtility.ExecuteScalar(
                                        "Exec P_JobCustomerAmountRecord {0},{1},{2},{3},{4},{5},{6},{7}",
                                        bill.CustomerCode,
                                        p.WayBillNumber, p.InStorageID, 3, 6, p.TariffPrepayFee * -1,
                                        "运单号：{0}退关税预付服务费".FormatWith(p.WayBillNumber), "system");
                                if (obj.ToString() != "1")
                                {
                                    throw new ArgumentException("该运单号\"{0}\"生成关税预付服务费退费记录失败！".FormatWith(p.WayBillNumber));
                                }
                            }
                            if (p.RemoteAreaFee > 0)
                            {
                                obj =
                                    dbUtility.ExecuteScalar(
                                        "Exec P_JobCustomerAmountRecord {0},{1},{2},{3},{4},{5},{6},{7}",
                                        bill.CustomerCode,
                                        p.WayBillNumber, p.InStorageID, 2, 12, p.RemoteAreaFee,
                                        "运单号：{0}扣偏远附加服务费".FormatWith(p.WayBillNumber), "system");
                                if (obj.ToString() != "1")
                                {
                                    throw new ArgumentException("该运单号\"{0}\"生成偏远附加服务费扣费记录失败！".FormatWith(p.WayBillNumber));
                                }
                            }
                            else if (p.RemoteAreaFee < 0)
                            {
                                obj =
                                    dbUtility.ExecuteScalar(
                                        "Exec P_JobCustomerAmountRecord {0},{1},{2},{3},{4},{5},{6},{7}",
                                        bill.CustomerCode,
                                        p.WayBillNumber, p.InStorageID, 3, 12, p.RemoteAreaFee * -1,
                                        "运单号：{0}退偏远附加服务费".FormatWith(p.WayBillNumber), "system");
                                if (obj.ToString() != "1")
                                {
                                    throw new ArgumentException("该运单号\"{0}\"生成偏远附加服务费退费记录失败！".FormatWith(p.WayBillNumber));
                                }
                            }
                            //if (p.SpecialFee <= 0) return;
                            //obj =
                            //    dbUtility.ExecuteScalar(
                            //        "Exec P_JobCustomerAmountRecord {0},{1},{2},{3},{4},{5},{6},{7}",
                            //        bill.CustomerCode,
                            //        p.WayBillNumber, p.InStorageID, 2, 7, p.SpecialFee,
                            //        "运单号：{0}扣特殊费".FormatWith(p.WayBillNumber), "system");
                            //if (obj.ToString() != "1")
                            //{
                            //    throw new ArgumentException("该运单号\"{0}\"生成特殊费扣费记录失败！".FormatWith(p.WayBillNumber));
                            //}
                        });
                    dbUtility.ExecuteNonQuery("update ReceivingBills set Search=100  where ReceivingBillID='" +
                                              bill.ReceivingBillID + "' and Status=1");//限制重复扣费
                    transaction.Complete();
                }
                result = true;
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
            Log.Info("LMS完成插入客户资金记录客户为：{1}，账单号为:{0}".FormatWith(bill.ReceivingBillID, bill.CustomerCode));
            return result;
        }
        /// <summary>
        /// 修改已生成订单状态
        /// </summary>
        /// <param name="receivingBillId"></param>
        /// <returns></returns>
        public static bool UpdateReceivingBillStatus(string receivingBillId)
        {
            Log.Info("LMS开始修改账单号为：{0}出账单状态".FormatWith(receivingBillId));
            var result = false;
            try
            {
                DbUtility dbUtility = new SqlDbUtility(_lmsCon);
                int obj =
                    dbUtility.ExecuteNonQuery("update ReceivingBills set Status=2  where ReceivingBillID='" +
                                              receivingBillId + "' and Status=1");
                result = obj > 0;
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
            Log.Info("LMS完成修改账单号为：{0}出账单状态".FormatWith(receivingBillId));
            return result;
        }
        ///// <summary>
        ///// 修改收货费用主表最后出账单时间
        ///// </summary>
        ///// <param name="receivingBillId"></param>
        ///// <param name="dt"></param>
        ///// <returns></returns>
        //public static bool UpdateLastOutBillTime(string receivingBillId, DateTime dt)
        //{
        //    Log.Info("LMS开始修改账单号为：{0}最后出账单时间".FormatWith(receivingBillId));
        //    var result = false;
        //    DbUtility dbUtility = new SqlDbUtility(_lmsCon);
        //    int obj =
        //        dbUtility.ExecuteNonQuery("update ReceivingExpenses set LastOutBillTime='" + dt.ToString("yyyy-MM-dd HH:mm:ss") + "'  where ReceivingBillID='" +
        //                                  receivingBillId + "'");
        //    result = obj > 0;
        //    Log.Info("LMS完成修改账单号为：{0}最后出账单时间".FormatWith(receivingBillId));
        //    return result;
        //}
    }
}
