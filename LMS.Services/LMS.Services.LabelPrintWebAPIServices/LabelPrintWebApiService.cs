using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Linq.Expressions;
using System.Text;
using LMS.Data.Entity;
using LMS.Data.Repository;

namespace LMS.Services.LabelPrintWebAPIServices
{
    public class LabelPrintWebApiService : ILabelPrintWebApiService
    {
        private readonly IWayBillInfoRepository _wayBillInfoRepository;
        public LabelPrintWebApiService(IWayBillInfoRepository wayBillInfoRepository)
        {
            _wayBillInfoRepository = wayBillInfoRepository;
        }
        public List<LabelPrintExt> GetLabelPrintExtList(IEnumerable<string> orderNumbers,string customercode)
        {
            
            return _wayBillInfoRepository.GetLabelPrintExtList(orderNumbers,customercode);

        }

    }
}
