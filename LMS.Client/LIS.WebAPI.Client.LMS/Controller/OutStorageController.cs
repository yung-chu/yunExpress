using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LIS.WebAPI.Client.LMS.Model;
using LMS.Core;
using LMS.Data.Context;
using LMS.Data.Entity;
using LMS.Data.Repository;
using LMS.Services.FreightServices;
using LMS.Services.OutStorageServices;
using LighTake.Infrastructure.Common.Logging;
using LighTake.Infrastructure.Http;
using LighTake.Infrastructure.Http.Infrastructure;

namespace LIS.WebAPI.Client.LMS.Controller
{
    public class OutStorageController
    {
        private readonly IOutStorageService _outStorageService;
        const int Batchqty = 100;//批量更新服务商价格运单号的数量
        public OutStorageController()
        {
            var dbContext = new LMS_DbContext();

            _outStorageService = new OutStorageService(new WayBillInfoRepository(dbContext),
                                                       new VenderFeeLogRepository(dbContext),
                                                       new OutStorageInfoRepository(dbContext),
                                                       new FreightService(new CustomerRepository(dbContext),
                                                       new CountryRepository(dbContext)));
        }

        private void UpdatePrice()
        {
            #region 获取需要更新价格的运单号
            Log.Info("开始获取服务商价格");
            var list = _outStorageService.GetNoPriceOutStorageWayBillList();
            if (list == null && list.Count == 0)
            {
                Log.Info(string.Format("需要获取服务商价格的运单数量:0"));
                return;
            }
            Log.Info(string.Format("需要获取服务商价格的运单数量:{0}", list.Count));
            #endregion

            #region 获取服务商成本价并更新
            foreach (var p in list)
            {
                var wayBillInfo = _outStorageService.GetWayBillInfoByWayBillNumber(p.WayBillNumber);
                if (wayBillInfo == null)
                {
                    Log.Error(string.Format("获取运单号为：{0}信息失败", p.WayBillNumber));
                    continue;
                }
                var result = PostVenderPrice(new VenderPackageModel()
                    {
                        Code = wayBillInfo.OutStorageInfo.VenderCode,
                        CountryCode = wayBillInfo.CountryCode,
                        Height = wayBillInfo.Height ?? 0,
                        Weight = wayBillInfo.Weight ?? 0,
                        Length = wayBillInfo.Length ?? 0,
                        Width = wayBillInfo.Width ?? 0,
                        ShippingMethodId = wayBillInfo.OutShippingMethodID ?? 0,
                        ShippingTypeId = p.GoodsTypeID
                    });
                if (result.CanShipping)
                {
                    try
                    {
                        _outStorageService.UpdateVenderPrice(p.WayBillNumber, result);
                        Log.Info(string.Format("运单号为：{0}更新服务商价格成功", p.WayBillNumber));
                    }
                    catch (Exception ex)
                    {
                        Log.Exception(ex);
                    }
                }
                else
                {
                    _outStorageService.UpdateErrorRemark(p.WayBillNumber, result.Message);
                    Log.Error(string.Format("获取运单号为：{0}的服务商成本价失败，原因:{1}", p.WayBillNumber, result.Message));
                }
            }
            #endregion

            #region 更新汇总服务商价格
            Log.Info("开始更新汇总服务商价格");
            var wayBillNumbers = list.Select(p => p.WayBillNumber).ToList();
            if (wayBillNumbers != null && wayBillNumbers.Count > 0)
            {
                int max = wayBillNumbers.Count / Batchqty;
                for (int n = 0; n < max; n++)
                {
                    if (!_outStorageService.UpdateOutStoragePrice(wayBillNumbers.GetRange(n * Batchqty, Batchqty)))
                    {
                        Log.Error(string.Format("更新失败，运单号为：{0}", string.Join(",", wayBillNumbers.GetRange(n * Batchqty, Batchqty))));
                    }
                }
                if (wayBillNumbers.Count - max * Batchqty != 0)
                {
                    if (!_outStorageService.UpdateOutStoragePrice(wayBillNumbers.GetRange(max * Batchqty,
                                                                                         (wayBillNumbers.Count -
                                                                                          max * Batchqty))))
                    {
                        Log.Error(string.Format("更新失败，运单号为：{0}", string.Join(",", wayBillNumbers.GetRange(max * Batchqty,
                                                                                         (wayBillNumbers.Count -
                                                                                          max * Batchqty)))));
                    }
                }
            }
            Log.Info("结束更新汇总服务商价格");
            #endregion
            Log.Info("本次更新完毕");
        }

        /// <summary>
        /// 获取出服务商价格（成本价）
        /// </summary>
        /// <param name="package"></param>
        /// <returns></returns>
        private PriceProviderResult PostVenderPrice(VenderPackageModel package)
        {
            var result = new PriceProviderResult();
            try
            {
                var list = HttpHelper.DoRequest<PriceProviderResult>(MyConfig.PostVenderPriceUrl, EnumHttpMethod.POST, EnumContentType.Json, package);
                result = list.Value;
                Log.Info(list.RawValue);
            }
            catch (Exception ex)
            {
                result.CanShipping = false;
                result.Message = ex.Message;
            }
            return result;
        }

        public static void Start()
        {
            try
            {
                var outStorageController = new OutStorageController();
                outStorageController.UpdatePrice();
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
        }
    }
}
