using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LMS.Data.Entity.ExtModel
{
    public class DeliveryFeeExt:LighTake.Infrastructure.Seedwork.Entity
    {
        public virtual int DeliveryFeeID { get; set; }
        public virtual string WayBillNumber { get; set; }
        public virtual string CustomerOrderNumber { get; set; }
        public virtual string Trackingnumber { get; set; }
        public virtual string VenderName { get; set; }
        public virtual string VenderCode { get; set; }
        public virtual Nullable<int> VenderId { get; set; }
        public virtual Nullable<int> ShippingmethodID { get; set; }
        public virtual string ShippingmethodName { get; set; }
        public virtual Nullable<decimal> SetWeight { get; set; }
        public virtual Nullable<decimal> AprroveWeight { get; set; }
        public virtual int Status { get; set; }
        public virtual string Remark { get; set; }
        public virtual string Auditor { get; set; }
        public virtual Nullable<System.DateTime> AuditorDate { get; set; }
        public virtual System.DateTime CreatedOn { get; set; }
        public virtual string CreatedBy { get; set; }
        public virtual System.DateTime LastUpdatedOn { get; set; }
        public virtual string LastUpdatedBy { get; set; }

        /// <summary>
        /// 发货时间（出仓时间）
        /// </summary>
        public virtual System.DateTime? OutStorageCreatedOn { get; set; }
        public virtual string CountryCode { get; set; }
        public virtual string CountryChineseName { get; set; }

        /// <summary>
        /// 称重质量
        /// </summary>
        public virtual Nullable<decimal> Weight { get; set; }
        //运费
        public virtual decimal? Freight { get; set; }
        //挂号费
        public virtual decimal? Register { get; set; }
        //燃油费
        public virtual decimal? FuelCharge { get; set; }
        //关税预付服务费
        public virtual decimal? TariffPrepayFee { get; set; }
        //附加费
        public virtual decimal? Surcharge { get; set; }
        //超长超重超周长费
        public virtual decimal? OverWeightLengthGirthFee { get; set; }
        //安全附加费
        public virtual decimal? SecurityAppendFee { get; set; }
        //增值税率费
        public virtual decimal? AddedTaxFee { get; set; }
        //杂费
        public virtual decimal? OtherFee { get; set; }
        //总费用
        public virtual decimal? TotalFee { get; set; }

        //最终总费用
        public virtual decimal? TotalFeeFinal { get; set; }

        //服务商数据
        public virtual DeliveryImportAccountCheck VenderData { get; set; }

        //杂费备注
        public virtual string OtherFeeRemark { get; set; }

        public string CustomerName { get; set; }
    }

    public class DeliveryFeeExpressExt:LighTake.Infrastructure.Seedwork.Entity
    {
        public virtual int DeliveryFeeID { get; set; }
        public virtual string WayBillNumber { get; set; }
        public virtual string CustomerOrderNumber { get; set; }
        public virtual string Trackingnumber { get; set; }
        public virtual string VenderName { get; set; }
        public virtual string VenderCode { get; set; }
        public virtual Nullable<int> VenderId { get; set; }
        public virtual Nullable<int> ShippingmethodID { get; set; }
        public virtual string ShippingmethodName { get; set; }
        public virtual Nullable<decimal> SetWeight { get; set; }
        public virtual Nullable<decimal> AprroveWeight { get; set; }
        public virtual int Status { get; set; }
        public virtual string Remark { get; set; }
        public virtual string Auditor { get; set; }
        public virtual Nullable<System.DateTime> AuditorDate { get; set; }
        public virtual System.DateTime CreatedOn { get; set; }
        public virtual string CreatedBy { get; set; }
        public virtual System.DateTime LastUpdatedOn { get; set; }
        public virtual string LastUpdatedBy { get; set; }

        /// <summary>
        /// 发货时间（出仓时间）
        /// </summary>
        public virtual System.DateTime? OutStorageCreatedOn { get; set; }
        public virtual string CountryCode { get; set; }
        public virtual string CountryChineseName { get; set; }

        /// <summary>
        /// 称重质量
        /// </summary>
        public virtual Nullable<decimal> Weight { get; set; }
        //运费
        public virtual decimal? Freight { get; set; }
        //挂号费
        public virtual decimal? Register { get; set; }
        //燃油费
        public virtual decimal? FuelCharge { get; set; }
        //关税预付服务费
        public virtual decimal? TariffPrepayFee { get; set; }
        //附加费
        public virtual decimal? Surcharge { get; set; }
        //超长超重超周长费
        public virtual decimal? OverWeightLengthGirthFee { get; set; }
        //安全附加费
        public virtual decimal? SecurityAppendFee { get; set; }
        //增值税率费
        public virtual decimal? AddedTaxFee { get; set; }
        //杂费
        public virtual decimal? OtherFee { get; set; }
        //总费用
        public virtual decimal? TotalFee { get; set; }

        //最终总费用
        public virtual decimal? TotalFeeFinal { get; set; }

        //服务商数据
        public virtual ExpressDeliveryImportAccountCheck VenderData { get; set; }

        //杂费备注
        public virtual string OtherFeeRemark { get; set; }
    }

    public class WayBillNumberExtSimple : LighTake.Infrastructure.Seedwork.Entity
    {
        public virtual string OrderOrTrackNumber { get; set; }
        public virtual string WayBillNumber { get; set; }
        public virtual Nullable<decimal> SetWeight { get; set; }
        public virtual decimal? TotalFee { get; set; }
        public virtual string Trackingnumber { get; set; }
    }

    public class DeliveryFeeAnomalyEditExt : LighTake.Infrastructure.Seedwork.Entity
    {
        public string CustomerOrderNumber { get; set; }
        public string CustomerName { get; set; }
        public string Remark { get; set; }
        public string WayBillNumber { get; set; }

        public decimal? SetWeightOriginal { get; set; }
        public decimal? FreightOriginal { get; set; }
        public decimal? RegisterOriginal { get; set; }
        public decimal? FuelChargeOriginal { get; set; }
        public decimal? TariffPrepayFeeOriginal { get; set; }
        public decimal? SurchargeOriginal { get; set; }
        public decimal? OverWeightLengthGirthFeeOriginal { get; set; }
        public decimal? SecurityAppendFeeOriginal { get; set; }
        public decimal? AddedTaxFeeOriginal { get; set; }
        public decimal? OtherFeeOriginal { get; set; }
        public decimal? TotalFeeOriginal { get; set; }
        public string OtherFeeRemark { get; set; }

        public decimal? FreightFinal { get; set; }
        public decimal? RegisterFinal { get; set; }
        public decimal? FuelChargeFinal { get; set; }
        public decimal? TariffPrepayFeeFinal { get; set; }
        public decimal? SurchargeFinal { get; set; }
        public decimal? OverWeightLengthGirthFeeFinal { get; set; }
        public decimal? SecurityAppendFeeFinal { get; set; }
        public decimal? AddedTaxFeeFinal { get; set; }
        public decimal? OtherFeeFinal { get; set; }
        public decimal? TotalFeeFinal { get; set; }

        public decimal? SetWeightVender { get; set; }
        public decimal? OtherFeeVender { get; set; }
        public decimal? FreightVender { get; set; }
        public decimal? RegisterVender { get; set; }
        public decimal? FuelChargeVender { get; set; }
        public decimal? TariffPrepayFeeVender { get; set; }
        public decimal? SurchargeVender { get; set; }
        public decimal? OverWeightLengthGirthFeeVender { get; set; }
        public decimal? SecurityAppendFeeVender { get; set; }
        public decimal? AddedTaxFeeVender { get; set; }
        public string OtherFeeRemarkVender { get; set; }
        public decimal? TotalFeeVender { get; set; }
    }

    public class DeliveryFeeCheckModel:LighTake.Infrastructure.Seedwork.Entity
    {
        public virtual string OrderNumber { get; set; }
        public virtual string Remark { get; set; }
        public virtual int Status { get; set; }
    }
}
