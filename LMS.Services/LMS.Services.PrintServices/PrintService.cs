using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LMS.Data.Entity;
using LMS.Data.Repository;

namespace LMS.Services.PrintServices
{
    /// <summary>
    /// 该打印服务只供打印标签对外接口使用
    /// </summary>
    public class PrintService : IPrintService
    {
        private readonly IWayBillTemplateRepository _wayBillTemplateRepository;
        public PrintService(IWayBillTemplateRepository wayBillTemplateRepository)
        {
            _wayBillTemplateRepository = wayBillTemplateRepository;
        }
        public List<LabelPrintModel> GetLabelPrint(List<string> orderNumbers, string customerCode, string templateTypeId)
        {
            var list = new List<LabelPrintModel>();
            if (orderNumbers.Any())
            {
                 _wayBillTemplateRepository.GetLabelPrint(orderNumbers, customerCode, templateTypeId).ForEach(p =>
                     {
                         list.Add(p);
                         orderNumbers.Remove(p.OrderNumber);
                     });
                if (orderNumbers.Any())
                {
                    orderNumbers.ForEach(p => list.Add(new LabelPrintModel()
                        {
                            IsHavePrint = false,
                            OrderNumber = p,
                            ShippingMethodId = 0
                        }));
                }
            }
            return list;
        }
    }
}
