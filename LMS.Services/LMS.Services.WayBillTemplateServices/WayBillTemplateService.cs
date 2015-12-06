using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using LMS.Core;
using LMS.Data.Entity;
using LMS.Data.Repository;
using LighTake.Infrastructure.Common;
using LighTake.Infrastructure.Common.Logging;

namespace LMS.Services.WayBillTemplateServices
{
    public class WayBillTemplateService:IWayBillTemplateService
    {

        private IWayBillTemplateRepository _wayBillTemplateRepository;
        private readonly IWayBillTemplateInfoRepository _wayBillTemplateInfoRepository;
        private readonly IGZPacketAddressInfoRepository _gzPacketAddressInfoRepository;
        private readonly ICustomerOrderInfoRepository _customerOrderInfoRepository;
        private IWorkContext _workContext;
        public WayBillTemplateService(IWayBillTemplateRepository wayBillTemplateRepository,
                                      IWorkContext workContext,
                                      IWayBillTemplateInfoRepository wayBillTemplateInfoRepository,
                                      IGZPacketAddressInfoRepository gzPacketAddressInfoRepository,
                                      ICustomerOrderInfoRepository customerOrderInfoRepository)
        {
            _wayBillTemplateRepository = wayBillTemplateRepository;
            _workContext = workContext;
            _wayBillTemplateInfoRepository = wayBillTemplateInfoRepository;
            _gzPacketAddressInfoRepository = gzPacketAddressInfoRepository;
            _customerOrderInfoRepository = customerOrderInfoRepository;
        }

        public List<WayBillTemplate> GetList()
        {
            int disable = WayBillTemplateInfo.StatusToValue(WayBillTemplateInfo.StatusEnum.Disable);
            return _wayBillTemplateRepository.GetList(p=>p.Status!=disable);
        }

        public WayBillTemplate GetWayBillTemplate(int wayBillTempLateId)
        {
            return _wayBillTemplateRepository.Get(wayBillTempLateId);
        }

        public IEnumerable<WayBillTemplateExt> GetGetWayBillTemplateExtByName(string templateName)
        {
            return _wayBillTemplateRepository.GetGetWayBillTemplateExtByName(templateName);
        }

        public List<WayBillTemplate> GetWayBillTemplateByShippingMethod(int shippingMethodId)
        {
            int disable = WayBillTemplateInfo.StatusToValue(WayBillTemplateInfo.StatusEnum.Disable);
            return _wayBillTemplateRepository.GetList(p => p.ShippingMethodId == shippingMethodId&&p.Status!=disable);
        }

        public List<WayBillTemplate> GetWayBillTemplateByNameAndShippingMethod(string templateName, int shippingMethodId)
        {
            int disable = WayBillTemplateInfo.StatusToValue(WayBillTemplateInfo.StatusEnum.Disable);
            return _wayBillTemplateRepository.GetList(p => p.TemplateName == templateName && p.ShippingMethodId == shippingMethodId && p.Status != disable);
        }
        //public List<WayBillTemplate> GetWayBillTemplateList(List<int> shippingMethodIds, string templateTypeId)
        //{
        //    int disable = WayBillTemplateInfo.StatusToValue(WayBillTemplateInfo.StatusEnum.Disable);
        //    Expression<Func<WayBillTemplate, bool>> filter = p => true;
        //    filter = filter.AndIf(p => shippingMethodIds.Contains(p.ShippingMethodId), shippingMethodIds.Count > 0)
        //                        .AndIf(p => p.TemplateTypeId == templateTypeId, true)
        //                        .AndIf(p=>p.Status!=disable,true)
        //                        ;
        //    return _wayBillTemplateRepository.GetList(filter);
        //}

        public IEnumerable<WayBillTemplateExt> GetWayBillTemplateList(IEnumerable<int> shippingMethodIds, string templateTypeId)
        {
            return _wayBillTemplateRepository.GetWayBillTemplateList(shippingMethodIds,templateTypeId);
        }

        public WayBillTemplateExt GetWayBillTemplate(int shippingMethodId, string templateName)
        {
            return _wayBillTemplateRepository.GetWayBillTemplate(shippingMethodId, templateName);
        }

        public bool GetCanPrint(string templateName, string number)
        {
            return _wayBillTemplateRepository.GetCanPrint(templateName, number);
        }

        public void AddWayBillTemplate(WayBillTemplate wayBillTemplate)
        {
            //状态 1-代表启用,2-代表禁用
            var list =
                _wayBillTemplateRepository.GetList(
                    p =>
                    p.TemplateName == wayBillTemplate.TemplateName.Trim() &&
                    p.ShippingMethodId == wayBillTemplate.ShippingMethodId &&
                    p.TemplateTypeId == wayBillTemplate.TemplateTypeId &&
                    p.Status != 2);
            if (null != list)
            {
                string countryNames = (from itemValue in list
                                       from str in
                                           wayBillTemplate.Countries.Split(new char[] { ',' },
                                                                  StringSplitOptions.RemoveEmptyEntries)
                                       where itemValue.Countries.Contains(str)
                                       select str).Aggregate(string.Empty, (current, str) => current.Contains(str) ? current : current + (str + ","));
                bool isExistCountryName = !string.IsNullOrWhiteSpace(countryNames);
                //判断国家是否重复
                if (isExistCountryName)
                {
                    throw new ArgumentException("该类型的模板存在重复国家有：" + countryNames);
                }
            }
            Check.Argument.IsNotNull(wayBillTemplate, "运单模板");
            wayBillTemplate.TemplateName = wayBillTemplate.TemplateName.Trim();
            wayBillTemplate.CreatedBy = _workContext.User==null?"" :_workContext.User.UserUame;
            wayBillTemplate.CreatedOn = DateTime.Now;
            wayBillTemplate.LastUpdatedBy = _workContext.User == null ? "" : _workContext.User.UserUame;
            wayBillTemplate.LastUpdatedOn = DateTime.Now;
            _wayBillTemplateRepository.Add(wayBillTemplate);
            _wayBillTemplateRepository.UnitOfWork.Commit();
        }

        public void UpdateWayBillTemplate(WayBillTemplate wayBillTemplate)
        {
            Check.Argument.IsNotNull(wayBillTemplate,"运单模板");
            Check.Argument.IsNullOrWhiteSpace(wayBillTemplate.WayBillTemplateId.ToString(), "运单模板信息ID");
            WayBillTemplate wayBill = _wayBillTemplateRepository.Get(wayBillTemplate.WayBillTemplateId);
            if (wayBill==null)
            {
                throw new ArgumentException("该运单模板ID:{0}不存在".FormatWith(wayBillTemplate.WayBillTemplateId));
            }
            //var isExist= _wayBillTemplateRepository.Exists(
            //    p =>p.WayBillTemplateId!=wayBillTemplate.WayBillTemplateId&&
            //    p.TemplateName == wayBillTemplate.TemplateName && p.ShippingMethodId == wayBillTemplate.ShippingMethodId);
            //if (isExist)
            //{
            //    throw new ArgumentException("运单打印模版，相同名称,相同运输方式，只能有一个模版");
            //}
            var list =
                _wayBillTemplateRepository.GetList(
                    p =>
                    p.WayBillTemplateId !=wayBillTemplate.WayBillTemplateId &&
                    p.TemplateName == wayBillTemplate.TemplateName.Trim() &&
                    p.ShippingMethodId == wayBillTemplate.ShippingMethodId &&
                    p.TemplateTypeId == wayBillTemplate.TemplateTypeId);
            if (null != list)
            {
                string countryNames = (from itemValue in list
                                       from str in
                                           wayBillTemplate.Countries.Split(new char[] { ',' },
                                                                  StringSplitOptions.RemoveEmptyEntries)
                                       where itemValue.Countries.Contains(str)
                                       select str).Aggregate(string.Empty, (current, str) => current.Contains(str) ? current : current + (str + ","));
                bool isExistCountryName = !string.IsNullOrWhiteSpace(countryNames);
                //判断国家是否重复
                if (isExistCountryName)
                {
                    throw new ArgumentException("该类型的模板存在重复国家有：" + countryNames);
                }
            }
            wayBill.TemplateName = wayBillTemplate.TemplateName.Trim();
            wayBill.TemplateTypeId = wayBillTemplate.TemplateTypeId;
            wayBill.ShippingMethodId = wayBillTemplate.ShippingMethodId;
            wayBill.RowNumber = wayBillTemplate.RowNumber;
            wayBill.ColumnNumber = wayBillTemplate.ColumnNumber;
            wayBill.Status = wayBillTemplate.Status;
            wayBill.TemplateContent = wayBillTemplate.TemplateContent;
            wayBill.Remark = wayBillTemplate.Remark;
            wayBill.TemplateHeadId = wayBillTemplate.TemplateHeadId;
            wayBill.TemplateContentId = wayBillTemplate.TemplateContentId;
            wayBill.Countries = wayBillTemplate.Countries;
            wayBill.LinkMode = wayBillTemplate.LinkMode;
            wayBill.LastUpdatedBy = _workContext.User.UserUame;
            wayBill.LastUpdatedOn = DateTime.Now;
            _wayBillTemplateRepository.Modify(wayBill);
            _wayBillTemplateRepository.UnitOfWork.Commit();
        }


        public IPagedList<WayBillTemplate> GetWayBillTemplatePagedList(WayBillTemplateListParam param)
        {
            Expression<Func<WayBillTemplate, bool>> filter = p => true;
            filter = filter.AndIf(p => p.ShippingMethodId == param.ShippingMethodId,
                                  param.ShippingMethodId!=0);
            Func<IQueryable<WayBillTemplate>, IOrderedQueryable<WayBillTemplate>>
              orderBy = o => o.OrderByDescending(p => p.CreatedOn);
            return _wayBillTemplateRepository.FindPagedList(param.Page, param.PageSize, filter, orderBy);
        }

        /// <summary> 获取新建模板列表
        /// 获取新建模板列表
        /// Add zhengsong
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public IPagedList<WayBillTemplateInfo> GetWayBillTemplateInfoList(WayBillTemplateInfoParam param)
        {
            Expression<Func<WayBillTemplateInfo, bool>> filter = p => true;
            filter = filter.AndIf(p => p.TemplateName.Contains(param.TemplateName), !string.IsNullOrWhiteSpace(param.TemplateName))
                           .AndIf(p => p.TemplateType == param.TemplateType,param.TemplateType != 0)
                           .AndIf(p => p.Status == param.Status, param.Status != 0);
            Func<IQueryable<WayBillTemplateInfo>, IOrderedQueryable<WayBillTemplateInfo>>
              orderBy = o => o.OrderByDescending(p => p.CreatedOn);
            return _wayBillTemplateInfoRepository.FindPagedList(param.Page,param.PageSize,filter,orderBy);
        }

        /// <summary> 添加和编辑模板
        /// Add by zhengsong
        /// </summary>
        /// <param name="wayBillTemplateInfo"></param>
        /// <returns></returns>
        public bool AddWayBillTemplateInfo(WayBillTemplateInfo wayBillTemplateInfo)
        {
            bool result = false;
            if (wayBillTemplateInfo != null)
            {
                if (wayBillTemplateInfo.TemplateModelId != 0)
                {
                  var wayBillTemplate = _wayBillTemplateInfoRepository.Get(wayBillTemplateInfo.TemplateModelId);
                    if (wayBillTemplate != null)
                    {
                        wayBillTemplate.TemplateType = wayBillTemplateInfo.TemplateType;
                        wayBillTemplate.TemplateName = wayBillTemplateInfo.TemplateName;
                        wayBillTemplate.TemplateContent = wayBillTemplateInfo.TemplateContent;
                        wayBillTemplate.Status = wayBillTemplateInfo.Status;
                        wayBillTemplate.Remarks = wayBillTemplateInfo.Remarks;
                        wayBillTemplate.LastUpdatedBy = _workContext.User.UserUame;
                        wayBillTemplate.LastUpdatedOn = DateTime.Now;
                        _wayBillTemplateInfoRepository.Modify(wayBillTemplate);
                        _wayBillTemplateInfoRepository.UnitOfWork.Commit();
                        result = true;
                    }
                }
                else
                {
                    _wayBillTemplateInfoRepository.Add(wayBillTemplateInfo);
                    _wayBillTemplateInfoRepository.UnitOfWork.Commit();
                    result = true;
                }
                
            }
            return result;
        }

        public WayBillTemplateInfo GetWayBillTemplateInfoByID(int templateModelId)
        {
            return _wayBillTemplateInfoRepository.Get(templateModelId);
        }

        //查询广州小包地址信息
        /// <summary>
        /// Add By zhengsong
        /// Time:2014-11-11
        /// </summary>
        /// <returns></returns>
        public List<GZPacketAddressInfo> GetGZPacketAddressInfo()
        {
            return _gzPacketAddressInfoRepository.GetFiltered(p => (p.Status == 1 && p.Number < 50)).ToList();
        }

        //更改广州小包地址信息
        /// <summary>
        /// Add By zhengsong
        /// Time:2014-11-11
        /// </summary>
        /// <param name="gzPacketAddressInfo"></param>
        /// <returns></returns>
        public void UpdateGZPacketAddressInfo(GZPacketAddressInfo gzPacketAddressInfo)
        {
            _gzPacketAddressInfoRepository.Modify(gzPacketAddressInfo);
            _gzPacketAddressInfoRepository.UnitOfWork.Commit();
        }
        /// <summary>
        /// 清空广州小包地址信息使用次数
        /// </summary>
        public void UpdateGZPacketAddressNumber()
        {
            try
            {
                var gzPacketAddressList = _gzPacketAddressInfoRepository.GetFiltered(p => p.Status == 1);
                if (gzPacketAddressList != null)
                {
                    gzPacketAddressList.ToList().ForEach(p =>
                    {
                        p.Number = 0;
                        _gzPacketAddressInfoRepository.Modify(p);
                    });
                    _gzPacketAddressInfoRepository.UnitOfWork.Commit();
                    Log.Info("清空数量" + gzPacketAddressList.Count());
                }
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
        }
        /// <summary>
        /// 根据客户订单号获取客户订单信息（对外打印标签接口）
        /// </summary>
        /// <param name="orderNumbers"></param>
        /// <returns></returns>
        public List<CustomerOrderInfo> PrintByCustomerOrderNumbers(List<string> orderNumbers,string customerCode)
        {
            int deleteStatus = CustomerOrder.StatusToValue(CustomerOrder.StatusEnum.Delete);
            var orderInfoList = _customerOrderInfoRepository.GetList(p => orderNumbers.Contains(p.CustomerOrderNumber)&& p.CustomerCode==customerCode && p.Status != deleteStatus);
            orderInfoList.ForEach(p =>
            {
                if (p.IsPrinted) return;
                p.IsPrinted = true;
                p.LastUpdatedBy = customerCode;
                p.LastUpdatedOn = DateTime.Now;
                _customerOrderInfoRepository.Modify(p);
            });

            _customerOrderInfoRepository.UnitOfWork.Commit();

            return orderInfoList;
        }
    }
}
