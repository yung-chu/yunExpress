using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DbHelper;
using LMS.Client.UpdateReceivingPrice.Model;
using LighTake.Infrastructure.Common;
using LighTake.Infrastructure.Common.Logging;

namespace LMS.Client.UpdateReceivingPrice.Controller
{
    public class WayBillController
    {
        private static readonly string _lmsCon = System.Configuration.ConfigurationManager.AppSettings["LMSCon"].ToString();
        /// <summary>
        /// 获取需要更新收货运费的运单
        /// </summary>
        /// <returns></returns>
        public static List<WayBillPriceModel> GetUpdatePriceWayBillList()
        {
            DbUtility dbUtility = new SqlDbUtility(_lmsCon);
            DataTable dt = dbUtility.ExecuteData(@"SELECT r.WayBillNumber,r.ReceivingExpenseID,w.EnableTariffPrepay,c.CustomerID,c.CustomerTypeID,w.GoodsTypeID,
	            w.CountryCode,w.InShippingMethodID,ISNULL(s.ShippingCity,'') ShippingCity,ISNULL(s.ShippingState,'') ShippingState,
				ISNULL(s.ShippingZip,'') ShippingZip,CONVERT(XML, (SELECT Length, Width,Height,Weight
                FROM WaybillPackageDetails PackageRequest
                WHERE PackageRequest.WayBillNumber=w.WayBillNumber
                FOR XML Path('PackageRequest'),ROOT('ArrayOfPackageRequest')))AS PackageRequest
                FROM dbo.ReceivingExpenses r LEFT JOIN dbo.WayBillInfos w ON r.WayBillNumber=w.WayBillNumber
	            LEFT JOIN dbo.Customers c ON w.CustomerCode=c.CustomerCode
				LEFT JOIN dbo.ShippingInfos s ON w.ShippingInfoID=s.ShippingInfoID
                WHERE IsNoGet=0 AND DATEDIFF(DAY,w.CreatedOn,GETDATE())>=1
                AND EXISTS(SELECT 1 FROM WaybillPackageDetails wp WHERE w.WayBillNumber=wp.WayBillNumber)");
            var list = new List<WayBillPriceModel>();
            if (dt != null && dt.Rows.Count > 0)
            {
                Log.Info("LMS收货需要更新运费运单总数是" + dt.Rows.Count);
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    var v = new WayBillPriceModel();
                    for (int j = 0; j < dt.Columns.Count; j++)
                    {
                        switch (dt.Columns[j].ColumnName)
                        {
                            case "EnableTariffPrepay":
                                v.EnableTariffPrepay = bool.Parse(dt.Rows[i][j].ToString());
                                break;
                            case "CountryCode":
                                v.CountryCode = dt.Rows[i][j].ToString();
                                break;
                            case "WayBillNumber":
                                v.WayBillNumber = dt.Rows[i][j].ToString();
                                break;
                            case "CustomerID":
                                v.CustomerId = Guid.Parse(dt.Rows[i][j].ToString());
                                break;
                            //case "CustomerTypeID":
                            //    v.CustomerTypeID = Int32.Parse(dt.Rows[i][j].ToString());
                            //    break;
                            case "PackageRequest":
                                v.Packages = SerializeUtil.DeserializeFromXml<List<PackageRequest>>(dt.Rows[i][j].ToString());
                                break;
                            case "InShippingMethodID":
                                v.ShippingMethodId = Int32.Parse(dt.Rows[i][j].ToString());
                                break;
                            case "GoodsTypeID":
                                v.ShippingTypeId = Int32.Parse(dt.Rows[i][j].ToString());
                                break;
                            case "ReceivingExpenseID":
                                v.ReceivingExpenseID = Int32.Parse(dt.Rows[i][j].ToString());
                                break;
                            case "ShippingCity":
                                v.ShippingInfo.ShippingCity = dt.Rows[i][j].ToString();
                                break;
                            case "ShippingState":
                                v.ShippingInfo.ShippingState = dt.Rows[i][j].ToString();
                                break;
                            case "ShippingZip":
                                v.ShippingInfo.ShippingZip = dt.Rows[i][j].ToString();
                                break;
                        }
                    }
                    list.Add(v);
                }
            }
            Log.Info("LMS收货需要更新运费运单获取完毕！");
            return list;
        }
        /// <summary>
        /// 更新费用
        /// </summary>
        /// <param name="priceResult"></param>
        /// <param name="wayBillNumber"></param>
        /// <returns></returns>
        public static bool UpdatePriceWayBill(PriceProviderResult priceResult, string wayBillNumber, int receivingExpenseId)
        {
            Log.Info(string.Format("开始更新运单号为：{0}的运费！", wayBillNumber));
            bool result = false;
            try
            {
                if (priceResult.CanShipping)
                {
                    if (priceResult.Weight <= 0)
                    {
                        UpdateJobErrorLog(wayBillNumber, string.Format("运单号：{0},运费计算中心接口计算结算重量为0", wayBillNumber) + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + Environment.NewLine);
                        Log.Error("运单号：{0},运费计算中心接口计算结算重量为0".FormatWith(wayBillNumber));
                        return false;
                    }

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
                                UpdateJobErrorLog(wayBillNumber, string.Format("运单号：{0},运费计算中心接口计算结算重量和是{1}小于包裹明细净重量和为{2}", wayBillNumber,priceResult.Weight,suttle) + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + Environment.NewLine);
                                Log.Error("运单号：{0},运费计算中心接口计算结算重量和是{1}小于包裹明细净重量和为{2}".FormatWith(wayBillNumber, priceResult.Weight, suttle));
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
                            sb.Append("<LengthFee>{0}</LengthFee>".FormatWith(p.OverGirthFee));
                            sb.Append("<WeightFee>{0}</WeightFee>".FormatWith(p.OverWeightOrLengthFee));
                            sb.Append("</PackageDetails>");
                        });
                    string obj = dbUtility.ExecuteScalar(
                        "exec P_UpdateReceivingFreight {0},{1},{2},{3},{4},{5},{6},{7},{8},{9}",
                        wayBillNumber, receivingExpenseId, priceResult.Weight, priceResult.ShippingFee,
                        priceResult.RemoteAreaFee, priceResult.RegistrationFee, priceResult.FuelFee,
                        priceResult.Value - priceResult.ShippingFee - priceResult.RemoteAreaFee - priceResult.RegistrationFee - priceResult.FuelFee - priceResult.TariffPrepayFee,//其他费用把总费用减去已知费用
                        priceResult.TariffPrepayFee, sb.ToString()).ToString();
                    result = obj == "1";
                    if (obj == "2")
                    {
                        Log.Error("运单号：{0}运费已经更新！".FormatWith(wayBillNumber));
                    }
                    if (result)
                    {
                        UpdateJobErrorLog(wayBillNumber,null);
                    }
                    Log.Info(string.Format("完成更新运单号为：{0}的运费！", wayBillNumber));
                }
                else
                {
                    UpdateJobErrorLog(wayBillNumber, priceResult.Message + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + Environment.NewLine);
                    Log.Error("运单号：{0}获取运费失败，错误：{1}".FormatWith(wayBillNumber, priceResult.Message));
                }
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
            return result;
        }
        /// <summary>
        /// 更新错误日记
        /// </summary>
        /// <param name="waybillnumber"></param>
        /// <param name="errorbody"></param>
        public static void UpdateJobErrorLog(string waybillnumber, string errorbody)
        {
            const int jobtype = 1; //获取收货运费Job
            const int errortype = 2; //-运费接口错误
            try
            {
                var iscorrect = 0;
                if (string.IsNullOrWhiteSpace(errorbody))
                {
                    iscorrect = 1;
                }
                DbUtility dbUtility = new SqlDbUtility(_lmsCon);
                dbUtility.ExecuteNonQuery("exec P_JobErrorLog {0},{1},{2},{3},{4},{5}", waybillnumber, jobtype,
                                          errortype, errorbody??"", "system", iscorrect);
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }

        }
    }
}
