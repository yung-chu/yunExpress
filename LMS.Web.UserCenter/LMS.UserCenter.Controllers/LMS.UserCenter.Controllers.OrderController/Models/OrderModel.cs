using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Caching;
using System.Web.Hosting;
using System.Web.Mvc;
using FluentValidation;
using FluentValidation.Attributes;
using LMS.Core;
using LMS.Data.Entity;
using LMS.Services.CommonServices;
using LMS.Services.CustomerOrderServices;
using LMS.Services.FreightServices;
using LighTake.Infrastructure.Common.InversionOfControl;
using LighTake.Infrastructure.Web.Models;

namespace LMS.UserCenter.Controllers.OrderController.Models
{

    public class BatchAddViewModels
    {
        public BatchAddViewModels()
        {
            OrderList = new List<OrderModel>();
            ShippingMethodModels = new List<SelectListItem>();
            ShippingMethodModel = new ShippingMethodModel();
            CountryModels = new List<SelectListItem>();
            ShippingMethodCountryModel = new ShippingMethodCountryModel();
        }

        public List<OrderModel> OrderList { get; set; }

        public int GoodsTypeID { get; set; }

        public string FilePath { get; set; }

        public string FilePathDate { get; set; }

        public List<SelectListItem> ShippingMethodModels { get; set; }

        public List<SelectListItem> CountryModels { get; set; }

        public ShippingMethodCountryModel ShippingMethodCountryModel { get; set; }

        public ShippingMethodModel ShippingMethodModel { get; set; }


        //public List<SelectListItem> 



    }

    [Validator(typeof(OrderModelValidator))]
    public class OrderModel : SearchFilter
    {
        public OrderModel()
        {
            ApplicationInfos = new List<ProductModel>();
            ErrorMessage = new StringBuilder();
            IsValid = true;
            ErrorType = 0;
        }

        public int GoodsTypeID { get; set; }
        public string CustomerOrderNumber { get; set; }
        public string ShippingMethodCode { get; set; }
        public string TrackingNumber { get; set; }
        public string WayBillNumber { get; set; }
        public string CountryCode { get; set; }
        public string ShippingFirstName { get; set; }
        public string ShippingLastName { get; set; }
        public string ShippingAddress { get; set; }
        public string ShippingAddress1 { get; set; }
        public string ShippingAddress2 { get; set; }
        public string ShippingCity { get; set; }
        public string ShippingState { get; set; }
        public string ShippingZip { get; set; }
        public string ShippingPhone { get; set; }
        public string ShippingCompany { get; set; }
        public string ShippingTaxId { get; set; }
        public string SenderFirstName { get; set; }
        public string SenderLastName { get; set; }
        public string SenderCompany { get; set; }
        public string SenderAddress { get; set; }
        public string SenderCity { get; set; }
        public string SenderState { get; set; }
        public string SenderZip { get; set; }
        public string SenderPhone { get; set; }
        public decimal? Weight { get; set; }
        public Nullable<decimal> Length { get; set; }
        public Nullable<decimal> Width { get; set; }
        public Nullable<decimal> Height { get; set; }
        public decimal? InsureAmount { get; set; }
        public int? AppLicationTypeId { get; set; }
        public int? PackageNumber { get; set; }

        public string InsureAmountValue { get; set; }

        public string ProductUrl { get; set; }

        public bool IsReturn
        {
            get
            {
                return ReturnString.ToUpperInvariant() == "Y";
            }
        }
        public string ReturnString { get; set; }
        public bool IsInsured { get; set; }
        public bool IsBattery { get { return !string.IsNullOrWhiteSpace(SensitiveTypeID); } }
        public string InsuredID { get; set; }
        public string InsuredValue { get; set; }
        public string SensitiveTypeID { get; set; }
        public List<ProductModel> ApplicationInfos { get; set; }

        public StringBuilder ErrorMessage { get; set; }

        public bool IsValid { get; set; }
        /// <summary>
        /// 错误类型跟排序
        /// 0=>没有错误
        /// 1=>运输方式错误
        /// 2=>国家简码错误
        /// 3=>运输方式跟国家代码都出现错误
        /// 4=>其他错误
        /// </summary>
        public int ErrorType { get; set; }

        /// <summary>
        /// Execl  行号
        /// </summary>
        public int ExeclRow { get; set; }

        public DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public DateTime LastUpdatedOn { get; set; }
        public string LastUpdatedBy { get; set; }

        public string EnableTariffPrepayString { get; set; }
    }



    public class OrderModelValidator : AbstractValidator<OrderModel>
    {
        public OrderModelValidator()
        {

            RuleFor(s => s.CustomerOrderNumber).Cascade(CascadeMode.StopOnFirstFailure)
                .NotEmpty().WithMessage("[{PropertyName}]不能为空")
                 .Length(1, 50).WithMessage("[{PropertyName}]长度必须为[{MinLength}-{MaxLength}]")
                //.Must(IsExistsOrderID).When(o => !o.IsReturn).WithMessage("[{PropertyName}-{PropertyValue}]已存在")
                 .WithName("客户订单号");
            RuleFor(s => s.TrackingNumber)
                .Length(1, 50).WithMessage("[{PropertyName}]长度必须为[{MinLength}-{MaxLength}]")
               .When(p => !string.IsNullOrWhiteSpace(p.TrackingNumber))
                .WithName("跟踪号");
            RuleFor(s => s.ShippingMethodCode)
                .NotEmpty().WithMessage("[{PropertyName}]不能为空")
                .Must(ShippingMethodIsEnable).WithMessage("[{PropertyName}]不存在或不可用")
                .WithName("运输方式代号"); 
            RuleFor(s => s.ShippingFirstName)
                .NotEmpty().WithMessage("[{PropertyName}]不能为空")
                .Length(1, 100).WithMessage("[{PropertyName}]长度必须为[{MinLength}-{MaxLength}]")
                .WithName("收件人姓");
            RuleFor(s => s.ShippingLastName)

                //.NotEmpty().WithMessage("[{PropertyName}]不能为空")
                .Length(1, 100).When(s => !string.IsNullOrWhiteSpace(s.ShippingLastName)).WithMessage("[{PropertyName}]长度必须为[{MinLength}-{MaxLength}]")
                .WithName("收件人名");
            RuleFor(s => s.ShippingAddress)
                .NotEmpty().WithMessage("[{PropertyName}]不能为空")
                .Length(1, 200).WithMessage("[{PropertyName}]长度必须为[{MinLength}-{MaxLength}]")
                .WithName("收件人地址");
            RuleFor(s => s.ShippingCity)
                .NotEmpty().WithMessage("[{PropertyName}]不能为空")
                .Length(1, 100).WithMessage("[{PropertyName}]长度必须为[{MinLength}-{MaxLength}]")
                .WithName("城市");
            RuleFor(s => s.ShippingState)
                .NotEmpty().WithMessage("[{PropertyName}]不能为空")
                .Length(1, 100).When(s => !string.IsNullOrWhiteSpace(s.ShippingState)).WithMessage("[{PropertyName}]长度必须为[{MinLength}-{MaxLength}]")
                .WithName("省/州");
            RuleFor(s => s.CountryCode)
                .NotEmpty().WithMessage("[{PropertyName}]不能为空")
                .Length(2).WithMessage("[{PropertyName}]长度必须为2个字符")
                .Must(CountryCodeIsEnable).WithMessage("[{PropertyName}]不存在")
                .WithName("国家代码");
            //RuleFor(s => s.ShippingZip)
            //    .NotEmpty().WithMessage("[{PropertyName}]不能为空")
            //    .Length(1, 50).WithMessage("[{PropertyName}]长度必须为[{MinLength}-{MaxLength}]")
            //    .WithName("邮编");
            RuleFor(s => s.ShippingPhone)
                .Length(1, 50).WithMessage("[{PropertyName}]长度必须为[{MinLength}-{MaxLength}]")
                .When(s => !string.IsNullOrWhiteSpace(s.ShippingPhone))
                .WithName("电话");
            RuleFor(s => s.InsuredID)
                .Must(IsExistsInsuredID).WithMessage("[{PropertyName}-{PropertyValue}]值不存在或不可用")
                .WithName("保险类型");

            //            RuleFor(s => s.GoodsTypeID)
            //                .NotEmpty().WithMessage("[{PropertyName}]不能为空")
            //                .Must(IsPackageType).WithMessage("[{PropertyName}]必须为大于零的整数")
            //                .WithName("包裹类型");
            RuleFor(s => s.SensitiveTypeID)
                .Must(IsGoodsType).WithMessage("[{PropertyName}-{PropertyValue}]值不存在或不可用")
                .WithName("货品类型");

        }
        private bool IsExistsOrderID(string value)
        {
            var service = EngineContext.Current.Resolve<ICustomerOrderService>();
            var workContext = EngineContext.Current.Resolve<IWorkContext>();
            return !service.IsExists(workContext.User.UserUame, value.Trim());
        }

        private bool IsExistsInsuredID(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return true;
            int id;

            if (int.TryParse(value, out id))
            {
                if (id < 1) return false;
            }
            var list = CacheHelper.Get("cache_InsuredList");
            List<InsuredCalculation> listInsuredCalculation;
            if (list == null)
            {
                var service = EngineContext.Current.Resolve<IInsuredCalculationService>();
                listInsuredCalculation = service.GetList();
                CacheHelper.Insert("cache_InsuredList", listInsuredCalculation);
            }
            else
            {
                listInsuredCalculation = list as List<InsuredCalculation>;
            }

            if (listInsuredCalculation == null) return false;

            if (listInsuredCalculation.Count == 0) return false;


            return (listInsuredCalculation.Any(a => a.InsuredID == id));

        }

        private bool IsGoodsType(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return true;

            int id;

            if (int.TryParse(value, out id))
            {
                if (id < 1) return false;
            }

            var list = CacheHelper.Get("cache_GoodsTypeList");
            List<SensitiveTypeInfo> listInsuredCalculation;
            if (list == null)
            {
                var service = EngineContext.Current.Resolve<ISensitiveTypeInfoService>();
                listInsuredCalculation = service.GetList();
                CacheHelper.Insert("cache_GoodsTypeList", listInsuredCalculation);
            }
            else
            {
                listInsuredCalculation = list as List<SensitiveTypeInfo>;
            }

            if (listInsuredCalculation == null) return false;

            if (listInsuredCalculation.Count == 0) return false;


            return (listInsuredCalculation.Any(a => a.SensitiveTypeID == id));



        }

        private bool IsPackageType(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return true;

            int id;

            if (int.TryParse(value, out id))
            {
                if (id < 1) return false;
            }

            var list = CacheHelper.Get("cache_PackageType");
            List<GoodsTypeInfo> listInsuredCalculation;
            if (list == null)
            {
                var service = EngineContext.Current.Resolve<IGoodsTypeService>();
                listInsuredCalculation = service.GetList();
                CacheHelper.Insert("cache_PackageType", listInsuredCalculation);
            }
            else
            {
                listInsuredCalculation = list as List<GoodsTypeInfo>;
            }

            if (listInsuredCalculation == null) return false;

            if (listInsuredCalculation.Count == 0) return false;



            return (listInsuredCalculation.Any(a => a.GoodsTypeID == id));



        }

        private bool IsYesOrNo(string value)
        {
            value = value.ToUpperInvariant();
            if (value == "Y" || value == "N")
            {
                return true;
            }
            return false;
        }

        private bool ShippingMethodIsEnable(string value)
        {

            var list = GetShippingMethodList();
            if (list == null) return false;

            if (list.Count == 0) return false;
            var valueToUpper = value.Trim().ToUpperInvariant();

            var result = list.Any(m => m.Code.ToUpperInvariant() == valueToUpper);
            return result;
        }

        public bool CountryCodeIsEnable(string value)
        {
            var list = GetCountryList();
            if (list == null) return false;
            if (list.Count == 0) return false;
            var valueToUpper = value.Trim().ToUpperInvariant();
            return list.Any(m => m.CountryCode == valueToUpper);
        }

        private List<ShippingMethodModel> GetShippingMethodList()
        {
            //注释缓存，运输方式跟新后不能及时取到最新状态
            //var list = CacheHelper.Get("cache_ShippingMethods");

            //if (list == null)
            //{
            var freightService = EngineContext.Current.Resolve<IFreightService>();
            // var workContext = EngineContext.Current.Resolve<IWorkContext>();
            var methods = freightService.GetShippingMethods(null, false);
            if (methods != null && methods.Count > 0)
            {
                //CacheHelper.Insert("cache_ShippingMethods", methods);
                return methods;
            }
            else
            {
                return null;
            }
            //}

            //return list as List<ShippingMethodModel>;
        }

        private List<ShippingMethodCountryModel> GetCountryList()
        {
            var list = CacheHelper.Get("cache_CountryMethods");
            if (list == null)
            {
                var service = EngineContext.Current.Resolve<IFreightService>();
                var countrys = service.GetCountrys();
                if (countrys != null && countrys.Count > 0)
                {
                    CacheHelper.Insert("cache_CountryMethods", countrys);
                    return countrys;
                }
                return null;
            }
            return list as List<ShippingMethodCountryModel>;
        }

    }


    public class OrderNlPostModelValidator : AbstractValidator<OrderModel>
    {
        public OrderNlPostModelValidator()
        {
            RuleFor(s => s.CustomerOrderNumber).Cascade(CascadeMode.StopOnFirstFailure)
                .NotEmpty().WithMessage("[{PropertyName}]不能为空")
                 .Length(1, 50).WithMessage("[{PropertyName}]长度必须为[{MinLength}-{MaxLength}]")
                 .WithName("客户订单号");
            RuleFor(s => s.TrackingNumber)
                .Length(1, 50).WithMessage("[{PropertyName}]长度必须为[{MinLength}-{MaxLength}]")
               .When(p => !string.IsNullOrWhiteSpace(p.TrackingNumber))
                .WithName("跟踪号");
            RuleFor(s => s.ShippingMethodCode)
                .NotEmpty().WithMessage("[{PropertyName}]不能为空")
                .Must(ShippingMethodIsEnable).WithMessage("[{PropertyName}]不存在或不可用")
                .WithName("运输方式代号");
            RuleFor(s => s.ShippingFirstName)
                .NotEmpty().WithMessage("{PropertyName}]不能为空")
                .Length(1, 60).WithMessage("[{PropertyName}]长度必须为[{MinLength}-{MaxLength}]")
                .WithName("收件人姓");
            RuleFor(s => s.ShippingFirstName + " " + s.ShippingLastName)
                .Length(1, 60).When(s => !string.IsNullOrWhiteSpace(s.ShippingFirstName))
                .WithMessage("[{PropertyName}]姓名总和长度必须为[{MinLength}-{MaxLength}]")
                .WithName("收件人名");
            RuleFor(s => s.ShippingAddress)
                .NotEmpty().WithMessage("[{PropertyName}]不能为空")
                .Length(1, 200).WithMessage("[{PropertyName}]长度必须为[{MinLength}-{MaxLength}]")
                .Must(IsNoChinese).WithMessage("[{PropertyName}]不能包含中文")
                .WithName("收件人地址");
            RuleFor(s => s.ShippingCity)
                .NotEmpty().WithMessage("[{PropertyName}]不能为空")
                .Length(1, 30).WithMessage("[{PropertyName}]长度必须为[{MinLength}-{MaxLength}]")
                .Must(IsNoChinese).WithMessage("[{PropertyName}]不能包含中文")
                .WithName("城市");
            RuleFor(s => s.ShippingState)
                .Length(1, 30).When(s => !string.IsNullOrWhiteSpace(s.ShippingState)).WithMessage("[{PropertyName}]长度必须为[{MinLength}-{MaxLength}]")
                .WithName("省/州");
            RuleFor(s => s.CountryCode)
                .NotEmpty().WithMessage("[{PropertyName}]不能为空")
                .Length(2).WithMessage("[{PropertyName}]长度必须为2个字符")
                .Must(CountryCodeIsEnable).WithMessage("[{PropertyName}]不存在")
                .WithName("国家代码");
            RuleFor(s => s.ShippingZip)
                .Length(1, 15).WithMessage("[{PropertyName}]长度必须为[{MinLength}-{MaxLength}]")
                .Matches(@"^[A-Za-z0-9 ]+$").WithMessage("[{PropertyName}]只能是英文和数字")
                .When(s => !string.IsNullOrWhiteSpace(s.ShippingZip))
                .WithName("邮编");
            RuleFor(s => s.ShippingCompany)
                .Length(1, 30).WithMessage("[{PropertyName}]长度必须为[{MinLength}-{MaxLength}]")
                .Must(IsNoChinese).WithMessage("[{PropertyName}]不能包含中文")
                .When(s => !string.IsNullOrWhiteSpace(s.ShippingCompany))
                .WithName("收件人公司");
            RuleFor(s => s.ShippingPhone)
                .Length(1, 20).WithMessage("[{PropertyName}]长度必须为[{MinLength}-{MaxLength}]")
                .Matches(@"^[0-9]*$").WithMessage("[{PropertyName}]只能是数字")
                .When(s => !string.IsNullOrWhiteSpace(s.ShippingPhone))
                .WithName("电话");
            RuleFor(s => s.InsuredID)
                .Must(IsExistsInsuredID).WithMessage("[{PropertyName}-{PropertyValue}]值不存在或不可用")
                .WithName("保险类型");
            RuleFor(s => s.SensitiveTypeID)
                .Must(IsGoodsType).WithMessage("[{PropertyName}-{PropertyValue}]值不存在或不可用")
                .WithName("货品类型");
        }
        private bool IsNoChinese(string str)
        {
            if (!string.IsNullOrWhiteSpace(str))
            {
                var regex = new Regex(@"([\u4E00-\u9FA5]|[\uFE30-\uFFA0])+");
                return !regex.IsMatch(str);
            }
            else
            {
                return true;
            }
        }
        private bool IsExistsInsuredID(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return true;
            int id;

            if (int.TryParse(value, out id))
            {
                if (id < 1) return false;
            }
            var list = CacheHelper.Get("cache_InsuredList");
            List<InsuredCalculation> listInsuredCalculation;
            if (list == null)
            {
                var service = EngineContext.Current.Resolve<IInsuredCalculationService>();
                listInsuredCalculation = service.GetList();
                CacheHelper.Insert("cache_InsuredList", listInsuredCalculation);
            }
            else
            {
                listInsuredCalculation = list as List<InsuredCalculation>;
            }

            if (listInsuredCalculation == null) return false;

            if (listInsuredCalculation.Count == 0) return false;


            return (listInsuredCalculation.Any(a => a.InsuredID == id));

        }

        private bool IsGoodsType(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return true;

            int id;

            if (int.TryParse(value, out id))
            {
                if (id < 1) return false;
            }

            var list = CacheHelper.Get("cache_GoodsTypeList");
            List<SensitiveTypeInfo> listInsuredCalculation;
            if (list == null)
            {
                var service = EngineContext.Current.Resolve<ISensitiveTypeInfoService>();
                listInsuredCalculation = service.GetList();
                CacheHelper.Insert("cache_GoodsTypeList", listInsuredCalculation);
            }
            else
            {
                listInsuredCalculation = list as List<SensitiveTypeInfo>;
            }

            if (listInsuredCalculation == null) return false;

            if (listInsuredCalculation.Count == 0) return false;


            return (listInsuredCalculation.Any(a => a.SensitiveTypeID == id));



        }
        private bool ShippingMethodIsEnable(string value)
        {

            var list = GetShippingMethodList();
            if (list == null) return false;

            if (list.Count == 0) return false;
            var valueToUpper = value.Trim().ToUpperInvariant();

            var result = list.Any(m => m.Code.ToUpperInvariant() == valueToUpper);
            return result;
        }

        public bool CountryCodeIsEnable(string value)
        {
            var list = GetCountryList();
            if (list == null) return false;
            if (list.Count == 0) return false;
            var valueToUpper = value.Trim().ToUpperInvariant();
            return list.Any(m => m.CountryCode == valueToUpper);
        }

        private List<ShippingMethodModel> GetShippingMethodList()
        {
            //注释缓存，运输方式跟新后不能及时取到最新状态
            //var list = CacheHelper.Get("cache_ShippingMethods");

            //if (list == null)
            //{
            var freightService = EngineContext.Current.Resolve<IFreightService>();
            // var workContext = EngineContext.Current.Resolve<IWorkContext>();
            var methods = freightService.GetShippingMethods(null, false);
            if (methods != null && methods.Count > 0)
            {
                //CacheHelper.Insert("cache_ShippingMethods", methods);
                return methods;
            }
            else
            {
                return null;
            }
            //}

            //return list as List<ShippingMethodModel>;
        }

        private List<ShippingMethodCountryModel> GetCountryList()
        {
            var list = CacheHelper.Get("cache_CountryMethods");
            if (list == null)
            {
                var service = EngineContext.Current.Resolve<IFreightService>();
                var countrys = service.GetCountrys();
                if (countrys != null && countrys.Count > 0)
                {
                    CacheHelper.Insert("cache_CountryMethods", countrys);
                    return countrys;
                }
                return null;
            }
            return list as List<ShippingMethodCountryModel>;
        }

    }

    public static class CacheHelper
    {
        private static readonly Cache Cache;

        public static double SaveTime
        {
            get;
            set;
        }

        static CacheHelper()
        {
            Cache = HostingEnvironment.Cache;
            SaveTime = 5.0;
        }

        public static object Get(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                return null;
            }

            return Cache.Get(key);
        }

        public static T Get<T>(string key)
        {
            object obj = Get(key);
            return obj == null ? default(T) : (T)obj;
        }

        public static void Insert(string key, object value, CacheDependency dependency, CacheItemPriority priority, CacheItemRemovedCallback callback)
        {
            Cache.Insert(key, value, dependency, Cache.NoAbsoluteExpiration, TimeSpan.FromMinutes(SaveTime), priority, callback);
        }

        public static void Insert(string key, object value, CacheDependency dependency, CacheItemRemovedCallback callback)
        {
            Insert(key, value, dependency, CacheItemPriority.Default, callback);
        }

        public static void Insert(string key, object value, CacheDependency dependency)
        {
            Insert(key, value, dependency, CacheItemPriority.Default, null);
        }

        public static void Insert(string key, object value)
        {
            Insert(key, value, null, CacheItemPriority.Default, null);
        }

        public static void Remove(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                return;
            }

            Cache.Remove(key);
        }

        public static IList<string> GetKeys()
        {
            List<string> keys = new List<string>();
            IDictionaryEnumerator enumerator = Cache.GetEnumerator();
            while (enumerator.MoveNext())
            {
                keys.Add(enumerator.Key.ToString());
            }

            return keys.AsReadOnly();
        }

        public static void RemoveAll()
        {
            IList<string> keys = GetKeys();
            foreach (string key in keys)
            {
                Cache.Remove(key);
            }
        }
    }

    [Validator(typeof(ProductModelValidator))]
    public class ProductModel
    {
        public string ApplicationName { get; set; }
        public string PickingName { get; set; }
        public string Qty { get; set; }
        public string UnitWeight { get; set; }
        public string UnitPrice { get; set; }
        public string HSCode { get; set; }
        public string Remark { get; set; }
        public string ProductUrl { get; set; }
        public decimal Total
        {
            get
            {
                if (string.IsNullOrWhiteSpace(UnitPrice) || string.IsNullOrWhiteSpace(Qty)) return 0;

                decimal unitPrice;
                int qty;
                if (int.TryParse(Qty, out qty) && decimal.TryParse(UnitPrice, out unitPrice))
                {
                    return unitPrice * qty;
                }

                return 0;
            }
        }

        public DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public DateTime LastUpdatedOn { get; set; }
        public string LastUpdatedBy { get; set; }
    }

    public class ProductModelValidator : AbstractValidator<ProductModel>
    {
        public ProductModelValidator()
        {
            RuleFor(s => s.ApplicationName)
                .NotEmpty().WithMessage("[{PropertyName}]不能为空")
                .Length(1, 200).WithMessage("[{PropertyName}]长度必须为[{MinLength}-{MaxLength}]")
                .WithName("申报名称");
            RuleFor(s => s.PickingName)
                .Length(0, 300).WithMessage("[{PropertyName}]长度必须为[{MinLength}-{MaxLength}]").When(p => !string.IsNullOrWhiteSpace(p.HSCode))
                .WithName("申报中文名称");

            RuleFor(s => s.HSCode)
               .Length(0, 50).WithMessage("[{PropertyName}]长度必须为[{MinLength}-{MaxLength}]")
               .WithName("海关编码");
            RuleFor(s => s.Qty)
                .NotEmpty().WithMessage("[{PropertyName}]不能为空")
                .Must(s =>
                {
                    int result;
                    if (Int32.TryParse(s, out result))
                    {
                        return result >= 1;
                    }
                    return false;
                }).WithMessage("[{PropertyName}]必须为大于零的整数")
                //.GreaterThan(0).WithMessage("[{PropertyName}]必须为大于零的整数")
                .WithName("数量");
            RuleFor(s => s.UnitWeight)
                .NotEmpty().WithMessage("[{PropertyName}]不能为空")
                .Must(s =>
                {
                    decimal result;
                    if (decimal.TryParse(s, out result))
                    {
                        return result > 0;
                    }
                    return false;
                }).WithMessage("[{PropertyName}]必须为大于零的数字")
                .WithName("申报净重量");
            RuleFor(s => s.UnitPrice)
                .NotEmpty().WithMessage("[{PropertyName}]不能为空")
                .Must(s =>
                {
                    decimal result;
                    if (decimal.TryParse(s, out result))
                    {
                        return result > 0;
                    }
                    return false;
                }).WithMessage("[{PropertyName}]必须为大于零的数字")
                .WithName("单价");
            //RuleFor(s => s.GoodsTypeID).GreaterThan(0).WithMessage(string.Format("[包裹类型]必须为大于零的整数"));
        }
    }
    public class ProductNlpostModelValidator : AbstractValidator<ProductModel>
    {
        public ProductNlpostModelValidator()
        {
            RuleFor(s => s.ApplicationName)
                .NotEmpty().WithMessage("[{PropertyName}]不能为空")
                .Length(1, 50).WithMessage("[{PropertyName}]长度必须为[{MinLength}-{MaxLength}]")
                .Matches(@"^[A-Za-z0-9 ]+$").WithMessage("[{PropertyName}]只能是英文和数字")
                .WithName("申报名称");

            RuleFor(s => s.HSCode)
                .Length(1, 10).When(s => !string.IsNullOrWhiteSpace(s.HSCode)).WithMessage("[{PropertyName}]长度必须为[{MinLength}-{MaxLength}]")
                .Matches(@"^[0-9]*$").When(s => !string.IsNullOrWhiteSpace(s.HSCode)).WithMessage("[{PropertyName}]只能是数字")
                .WithName("海关编码");

            RuleFor(s => s.Qty)
                .NotEmpty().WithMessage("[{PropertyName}]不能为空")
                .Must(s =>
                    {
                        int result;
                        if (Int32.TryParse(s, out result))
                        {
                            return result >= 1;
                        }
                        return false;
                    }).WithMessage("[{PropertyName}]必须为大于零的整数")
                .WithName("数量");

            RuleFor(s => s.UnitWeight)
                .NotEmpty().WithMessage("[{PropertyName}]不能为空")
                .Must(s =>
                    {
                        decimal result;
                        if (decimal.TryParse(s, out result))
                        {
                            return result > 0;
                        }
                        return false;
                    }).WithMessage("[{PropertyName}]必须为大于零的数字")
                .WithName("申报净重量");
            RuleFor(s => s.UnitPrice)
                .NotEmpty().WithMessage("[{PropertyName}]不能为空")
                .Must(s =>
                    {
                        decimal result;
                        if (decimal.TryParse(s, out result))
                        {
                            return result > 0;
                        }
                        return false;
                    }).WithMessage("[{PropertyName}]必须为大于零的数字")
                .WithName("申报单价");

            RuleFor(s => s.PickingName)
                .Length(1, 500).When(s => !string.IsNullOrWhiteSpace(s.PickingName)).WithMessage("[{PropertyName}]长度必须为[{MinLength}-{MaxLength}]")
                .WithName("配货名称");

            RuleFor(s => s.Remark)
                .Length(1, 500).When(s => !string.IsNullOrWhiteSpace(s.Remark)).WithMessage("[{PropertyName}]长度必须为[{MinLength}-{MaxLength}]")
                .WithName("备注");
            RuleFor(s => s.ProductUrl)
                .Length(1, 500).When(s => !string.IsNullOrWhiteSpace(s.ProductUrl)).WithMessage("[{PropertyName}]长度必须为[{MinLength}-{MaxLength}]")
                .WithName("销售链接");
        }
    }
}