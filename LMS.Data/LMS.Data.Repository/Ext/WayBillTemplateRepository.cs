using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LMS.Data.Context;
using LMS.Data.Entity;
using LighTake.Infrastructure.Common;

namespace LMS.Data.Repository
{
    public partial class WayBillTemplateRepository
    {
        public IEnumerable<WayBillTemplateExt> GetGetWayBillTemplateExtByName(string templateName)
        {
            var ctx = this.UnitOfWork as LMS_DbContext;
            Check.Argument.IsNotNull(ctx, "数据库对象");
            Check.Argument.IsNullOrWhiteSpace(templateName, "模板名称为空");
            int enable = WayBillTemplateInfo.StatusToValue(WayBillTemplateInfo.StatusEnum.Enable);
            var list =
                ctx.WayBillTemplates.Where(w => w.TemplateName.Equals(templateName) && w.Status == enable)
                    .Select(w => new WayBillTemplateExt
                    {
                        WayBillTemplateId = w.WayBillTemplateId,
                        TemplateName = w.TemplateName,
                        TemplateTypeId = w.TemplateTypeId,
                        ShippingMethodId = w.ShippingMethodId,
                        Status = w.Status,
                        TemplateContent = w.TemplateContent,
                        Remark = w.Remark,
                        RowNumber = w.RowNumber,
                        ColumnNumber = w.ColumnNumber,
                        TemplateHeadId = w.TemplateHeadId,
                        TemplateContentId = w.TemplateContentId,
                        LinkMode = w.LinkMode,
                        Countries = w.Countries,
                        CreatedOn = w.CreatedOn,
                        LastUpdatedOn = w.LastUpdatedOn,
                        TemplateBodyContent = (from wino in ctx.WayBillTemplateInfos
                            where
                                wino.TemplateModelId == w.TemplateContentId &&
                                wino.Status == enable
                            select wino.TemplateContent).FirstOrDefault(),
                        TemplateHead = (from wino in ctx.WayBillTemplateInfos
                            where wino.TemplateModelId == w.TemplateHeadId && wino.Status == enable
                            select wino.TemplateContent).FirstOrDefault(),
                    });
            return list.ToList();
        }

        public IEnumerable<WayBillTemplateExt> GetWayBillTemplateList(IEnumerable<int> shippingMethodIds,
            string templateTypeId)
        {
            var ctx = this.UnitOfWork as LMS_DbContext;
            Check.Argument.IsNotNull(ctx, "数据库对象");
            Check.Argument.IsNullOrWhiteSpace(templateTypeId, "模板类型Id");
            Check.Argument.IsNotNull(shippingMethodIds, "运输方式列表为空");
            var methodIds = shippingMethodIds as int[] ?? shippingMethodIds.ToArray();
            int enable = WayBillTemplateInfo.StatusToValue(WayBillTemplateInfo.StatusEnum.Enable);
            var list =
                ctx.WayBillTemplates.Where(
                    w =>
                        methodIds.Contains(w.ShippingMethodId) && w.TemplateTypeId == templateTypeId &&
                        w.Status == enable)
                    .Select(w => new WayBillTemplateExt
                    {
                        WayBillTemplateId = w.WayBillTemplateId,
                        TemplateName = w.TemplateName,
                        TemplateTypeId = w.TemplateTypeId,
                        ShippingMethodId = w.ShippingMethodId,
                        Status = w.Status,
                        TemplateContent = w.TemplateContent,
                        Remark = w.Remark,
                        RowNumber = w.RowNumber,
                        ColumnNumber = w.ColumnNumber,
                        TemplateHeadId = w.TemplateHeadId,
                        TemplateContentId = w.TemplateContentId,
                        LinkMode = w.LinkMode,
                        Countries = w.Countries,
                        CreatedOn = w.CreatedOn,
                        LastUpdatedOn = w.LastUpdatedOn,
                        TemplateBodyContent = (from wino in ctx.WayBillTemplateInfos
                            where
                                wino.TemplateModelId == w.TemplateContentId &&
                                wino.Status == enable
                            select wino.TemplateContent).FirstOrDefault(),
                        TemplateHead = (from wino in ctx.WayBillTemplateInfos
                            where wino.TemplateModelId == w.TemplateHeadId && wino.Status == enable
                            select wino.TemplateContent).FirstOrDefault(),
                    });
            return list.ToList();
        }


        public WayBillTemplateExt GetWayBillTemplate(int shippingMethodId, string templateName)
        {
            var ctx = this.UnitOfWork as LMS_DbContext;
            Check.Argument.IsNotNull(ctx, "数据库对象");
            Check.Argument.IsNullOrWhiteSpace(templateName, "模板名称");
            Check.Argument.IsNotNull(shippingMethodId, "运输方式");

            int enable = WayBillTemplateInfo.StatusToValue(WayBillTemplateInfo.StatusEnum.Enable);
            var list =
                ctx.WayBillTemplates.Where(
                    w => w.ShippingMethodId == shippingMethodId && w.TemplateName == templateName && w.Status == enable)
                    .Select(w => new WayBillTemplateExt
                    {
                        WayBillTemplateId = w.WayBillTemplateId,
                        TemplateName = w.TemplateName,
                        TemplateTypeId = w.TemplateTypeId,
                        ShippingMethodId = w.ShippingMethodId,
                        Status = w.Status,
                        TemplateContent = w.TemplateContent,
                        Remark = w.Remark,
                        RowNumber = w.RowNumber,
                        ColumnNumber = w.ColumnNumber,
                        TemplateHeadId = w.TemplateHeadId,
                        TemplateContentId = w.TemplateContentId,
                        LinkMode = w.LinkMode,
                        Countries = w.Countries,
                        CreatedOn = w.CreatedOn,
                        LastUpdatedOn = w.LastUpdatedOn,
                        TemplateBodyContent = (from wino in ctx.WayBillTemplateInfos
                            where
                                wino.TemplateModelId == w.TemplateContentId &&
                                wino.Status == enable
                            select wino.TemplateContent).FirstOrDefault(),
                        TemplateHead = (from wino in ctx.WayBillTemplateInfos
                            where wino.TemplateModelId == w.TemplateHeadId && wino.Status == enable
                            select wino.TemplateContent).FirstOrDefault(),
                    });
            return list.FirstOrDefault();
        }


        public bool GetCanPrint(string templateName, string number)
        {
            var ctx = this.UnitOfWork as LMS_DbContext;
            Check.Argument.IsNotNull(ctx, "数据库对象");
            Check.Argument.IsNullOrWhiteSpace(templateName, "模板名称");
            Check.Argument.IsNotNull(number, "单号");

            var list = from w in ctx.WayBillInfos
                join t in ctx.WayBillTemplates on w.InShippingMethodID equals t.ShippingMethodId
                where
                    (w.WayBillNumber == number || w.TrackingNumber == number ||
                     w.CustomerOrderNumber == number) && w.Status != (int) WayBill.StatusEnum.Delete
                     && t.TemplateName == templateName&& t.Status == (int) WayBillTemplateInfo.StatusEnum.Enable
                     && t.Countries.Contains(w.CountryCode)
                select 1;

            return list.Any();
        }

        public List<LabelPrintModel> GetLabelPrint(List<string> orderNumbers, string customerCode, string templateTypeId)
        {
            var ctx = this.UnitOfWork as LMS_DbContext;
            Check.Argument.IsNotNull(ctx, "数据库对象");
            var list =
                (from c in
                     ctx.CustomerOrderInfos.Where(
                         c => orderNumbers.Contains(c.CustomerOrderNumber) && c.CustomerCode == customerCode)
                 where (from w in ctx.WayBillTemplates.Where(w=>w.TemplateTypeId==templateTypeId) select w.ShippingMethodId).Contains(c.ShippingMethodId.Value)
                 select new LabelPrintModel
                     {
                         OrderNumber = c.CustomerOrderNumber,
                         ShippingMethodId = c.ShippingMethodId ?? 0,
                         IsHavePrint = true
                     }).ToList();
            return list;
        }
    }
}
