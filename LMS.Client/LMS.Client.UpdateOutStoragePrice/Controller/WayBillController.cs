using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using DbHelper;
using LMS.Client.UpdateOutStoragePrice.Model;
using LighTake.Infrastructure.Common;
using LighTake.Infrastructure.Common.Logging;

namespace LMS.Client.UpdateOutStoragePrice.Controller
{
    public class WayBillController
    {
        private static readonly string _lmsCon = System.Configuration.ConfigurationManager.AppSettings["LMSCon"].ToString();

        private static readonly string _status = System.Configuration.ConfigurationManager.AppSettings["WayBillStatus"].ToString();
        const int jobtype = 3; //获取发货运费Job
        const int errortype = 2; //-运费接口错误
        /// <summary>
        /// 获取需要更新的运单
        /// </summary>
        /// <returns></returns>
        public static List<VenderInfoPackageRequest> GetUpdatePriceWayBillList(string lastUpdateTime)
        {
            DbUtility dbUtility = new SqlDbUtility(_lmsCon);
            DataTable dt = dbUtility.ExecuteData("exec P_GetBatchFreightCostWayBillNumberList N'" + _status + "',N'" + lastUpdateTime+"'");
            var list = new List<VenderInfoPackageRequest>();
            if (dt != null && dt.Rows.Count > 0)
            {
                Log.Info("LMS需要更新成本价运单总数是" + dt.Rows.Count);
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    var v = new VenderInfoPackageRequest();
                    for (int j = 0; j < dt.Columns.Count; j++)
                    {
                        switch (dt.Columns[j].ColumnName)
                        {
                            case "VenderCode":
                                v.VenderCode = dt.Rows[i][j].ToString();
                                break;
                            case "CountryCode":
                                v.CountryCode = dt.Rows[i][j].ToString();
                                break;
                            case "WayBillNumber":
                                v.WayBillNumber = dt.Rows[i][j].ToString();
                                break;
                            case "PackageRequest":
                                if (string.IsNullOrWhiteSpace(dt.Rows[i][j].ToString()))
                                    continue;
                                v.Packages = SerializeUtil.DeserializeFromXml<List<VenderPackageRequest>>(dt.Rows[i][j].ToString());
                                break;
                            case "OutShippingMethodID":
                                v.ShippingMethodId = Int32.Parse(dt.Rows[i][j].ToString());
                                break;
                            case "GoodsTypeID":
                                v.ShippingTypeId = Int32.Parse(dt.Rows[i][j].ToString());
                                break;
                            case "CustomerID":
                                v.CustomerId = Guid.Parse(dt.Rows[i][j].ToString());
                                break;
                            case "EnableTariffPrepay":
                                v.EnableTariffPrepay = bool.Parse(dt.Rows[i][j].ToString());
                                break;
                        }
                    }
                    list.Add(v);
                }
            }
            Log.Info("LMS需要更新成本价运单获取完毕！");
            return list;
        }
        /// <summary>
        /// 更新运单操作
        /// </summary>
        /// <param name="priceResult"></param>
        /// <param name="wayBillNumber"></param>
        /// <returns></returns>
        public static bool UpdatePriceWayBill(PriceProviderResult priceResult, string wayBillNumber)
        {
            Log.Info(string.Format("开始更新运单号为：{0}的运费成本价！",wayBillNumber));
            bool result = false;
            if (priceResult.CanShipping)
            {
                try
                {
                    DbUtility dbUtility = new SqlDbUtility(_lmsCon);
                    decimal suttle = 0;
                    if (decimal.TryParse(
                        dbUtility.ExecuteScalar(
                            @"SELECT SUM(Weight) FROM WaybillPackageDetails WHERE WayBillNumber={0}", wayBillNumber)
                                 .ToString(), out suttle))
                    {
                        if (suttle == 0)
                        {
                            Log.Error("运单号：{0},包裹明细净重量和为0".FormatWith(wayBillNumber));
                            return false;
                        }
                        else
                        {
                            if (suttle > priceResult.Weight)
                            {
                                UpdateJobErrorLog(wayBillNumber, string.Format("运单号：{0},运费计算中心接口计算发货结算重量和是{1}小于包裹明细净重量和为{2}", wayBillNumber, priceResult.Weight, suttle) + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + Environment.NewLine);
                                Log.Error("运单号：{0},运费计算中心接口计算发货结算重量和是{1}小于包裹明细净重量和为{2}".FormatWith(wayBillNumber, priceResult.Weight, suttle));
                                return false;
                            }
                        }
                    }
                    else
                    {
                        Log.Error("运单号：{0},获取包裹明细净重量和失败！".FormatWith(wayBillNumber));
                        return false;
                    }
                    var sb = new StringBuilder();
                    priceResult.PackageDetails.ForEach(p =>
                    {
                        sb.Append("<PackageDetails>");
                        sb.Append("<Length>{0}</Length>".FormatWith(p.Length));
                        sb.Append("<SettleWeight>{0}</SettleWeight>".FormatWith(p.SettleWeight));
                        sb.Append("<Width>{0}</Width>".FormatWith(p.Width));
                        sb.Append("<Height>{0}</Height>".FormatWith(p.Height));
                        sb.Append("<Weight>{0}</Weight>".FormatWith(p.Weight));
                        //sb.Append("<LengthFee>{0}</LengthFee>".FormatWith(p.OverGirthFee));
                        //sb.Append("<WeightFee>{0}</WeightFee>".FormatWith(p.OverWeightOrLengthFee));
                        sb.Append("</PackageDetails>");
                    });
                    string obj = dbUtility.ExecuteScalar(
                        "exec P_UpdateOutStoragePriceAll {0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15},{16},{17}",
                        wayBillNumber,
                        priceResult.VenderId ?? 0, priceResult.VenderCode ?? "", priceResult.VenderName ?? "",
                        priceResult.ShippingMethodId ?? 0,
                        priceResult.ShippingMethodName ?? "", priceResult.Weight, priceResult.ShippingFee,
                        priceResult.RemoteAreaFee,
                        priceResult.RegistrationFee, priceResult.FuelFee, priceResult.OtherFee+priceResult.HandlingFee,
                        priceResult.Message ?? "", priceResult.TariffPrepayFee,priceResult.SecurityAppendFee,
                        priceResult.OverWeightOrLengthFee + priceResult.OverGirthFee, priceResult.AddedTaxFee, sb.ToString())
                                          .ToString();
                    result = obj == "1";
                    if (result)
                    {
                        UpdateJobErrorLog(wayBillNumber, null);
                    }
                    Log.Info(string.Format("完成更新运单号为：{0}的运费成本价！", wayBillNumber));
                }
                catch (Exception ex)
                {
                    Log.Exception(ex);
                }
            }
            else
            {
                UpdateJobErrorLog(wayBillNumber, priceResult.Message + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + Environment.NewLine);
                Log.Error("运单号:{0},错误：{1}".FormatWith(wayBillNumber,priceResult.Message));
            }
            return result;
        }
        /// <summary>
        /// 获取最后更新时间
        /// </summary>
        /// <returns></returns>
        public static string GetLastUpdateTime(string path)
        {
            string lastdatetime = LogTime.ReadFile(path);
            lastdatetime = lastdatetime.Replace("\r\n", "").Replace("\n", "").Replace("\r", "");
            if (!string.IsNullOrWhiteSpace(lastdatetime))
            {
                Log.Info(lastdatetime);
            }
            else
            {
                lastdatetime = DateTime.Now.AddYears(-10).ToString("yyyy-MM-dd HH:mm:ss");
            }
            return lastdatetime;
        }
        /// <summary>
        /// 更新错误日记
        /// </summary>
        /// <param name="waybillnumber"></param>
        /// <param name="errorbody"></param>
        public static void UpdateJobErrorLog(string waybillnumber, string errorbody)
        {
            try
            {
                var iscorrect = 0;
                if (string.IsNullOrWhiteSpace(errorbody))
                {
                    iscorrect = 1;
                }
                DbUtility dbUtility = new SqlDbUtility(_lmsCon);
                dbUtility.ExecuteNonQuery("exec P_JobErrorLog {0},{1},{2},{3},{4},{5}", waybillnumber, jobtype,
                                          errortype, errorbody ?? "", "system", iscorrect);
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }

        }
        /// <summary>
        /// 同步运输方式
        /// </summary>
        /// <param name="shippingMethodId"></param>
        /// <param name="shippingMethodTypeId"></param>
        /// <param name="enabled"></param>
        public static void SynchronousShippingMethod(int shippingMethodId, int shippingMethodTypeId, bool enabled)
        {
            Log.Info(string.Format("开始同步运输方式ID：{0}",shippingMethodId));
            try
            {
                DbUtility dbUtility = new SqlDbUtility(_lmsCon);
                dbUtility.ExecuteNonQuery("exec P_SynchronousShippingMethod {0},{1},{2}", shippingMethodId, shippingMethodTypeId,enabled?1:0);
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
            Log.Info(string.Format("完成同步运输方式ID：{0}",shippingMethodId));
        }
    }
}
