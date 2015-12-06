using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using LMS.Data.Context;
using LMS.Data.Entity;
using LMS.Data.Repository;
using LMS.Services.FinancialServices;

namespace LMS.Client.GetWayBillSettleWeight
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            string[] wayBillNumbers = File.ReadAllLines("wayBillNumbers.txt");

            LMS_DbContext lmsDbContext = new LMS_DbContext();
            FinancialService financialService = new FinancialService(null, null, null, null, null, new CustomerRepository(lmsDbContext), new WayBillInfoRepository(lmsDbContext), new WaybillPackageDetailRepository(lmsDbContext), null, null, null);

            File.AppendAllText("Success.csv", string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8}\r\n",
                                                            "运单号", "实际重量", "结算重量", "总费用","运费", "挂号费", "燃油费", "附加费", "关税预付服务费"));

            wayBillNumbers.ToList().ForEach(p =>
                {

                    if (string.IsNullOrWhiteSpace(p))
                        return;

                    Console.WriteLine("开始计算运单{0}结算重量", p);

                    var priceProviderResult = financialService.GetPriceProviderResult(p);

                    if (priceProviderResult == null)
                    {
                        Console.WriteLine("运单{0}无法计算，检查配置", p);

                        File.AppendAllText("Fail.txt", string.Format("{0}\t{1}\r\n", p, "检查配置"));

                        return;
                    }


                    if (priceProviderResult.CanShipping)
                    {
                        Console.WriteLine("运单{0}结算重量：{1}", p, priceProviderResult.Weight);

                        decimal Weight = 0;

                        priceProviderResult.PackageDetails.ForEach(pp =>
                            {
                                Weight += pp.Weight + pp.SettleWeight;
                            });

                        string sql = string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8}\r\n",
                                                   p, Weight, priceProviderResult.Weight, priceProviderResult.Value, priceProviderResult.ShippingFee, priceProviderResult.RegistrationFee,
                                                   priceProviderResult.FuelFee,
                                                   priceProviderResult.Value - (priceProviderResult.ShippingFee + priceProviderResult.FuelFee + priceProviderResult.RegistrationFee + priceProviderResult.TariffPrepayFee),
                                                   priceProviderResult.TariffPrepayFee);
                        //string sql = string.Format("update dbo.WayBillInfos set SettleWeight={0} where WayBillNumber='{1}'\r\n", priceProviderResult.Weight, p);
                        File.AppendAllText("Success.csv", sql);
                    }
                    else
                    {
                        Console.WriteLine("运单{0}无法运送：{1}", p, priceProviderResult.Message);

                        File.AppendAllText("Fail.txt", string.Format("{0}\t{1}\r\n", p, priceProviderResult.Message));
                    }
                });

            Console.WriteLine("处理完成,请查看文件");

            Console.ReadKey();
        }
    }
}
