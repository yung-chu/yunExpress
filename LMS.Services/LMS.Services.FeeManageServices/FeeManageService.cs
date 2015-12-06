using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Linq.Expressions;
using System.Text;
using LMS.Core;
using LMS.Data.Entity;
using LMS.Data.Entity.ExtModel;
using LMS.Data.Repository;
using LighTake.Infrastructure.Common;

namespace LMS.Services.FeeManageServices
{
    public class FeeManageService : IFeeManageService
    {
        private IWorkContext _workContext;
        private IFeeTypeRepository _feeTypeRepository;
        private IWayBillInfoRepository _wayBillInfoRepository;
        public FeeManageService(IWorkContext workContext, IFeeTypeRepository feeTypeRepository,
            IWayBillInfoRepository wayBillInfoRepository)
        {
            _workContext = workContext;
            _feeTypeRepository = feeTypeRepository;
            _wayBillInfoRepository = wayBillInfoRepository;
        }
        public List<FeeType> GetFeeTypeList(string feeTypeName, bool? isEnable)
        {
            Expression<Func<FeeType, bool>> filter = p => true;
            filter = filter.AndIf(p => p.FeeTypeName.Contains(feeTypeName), !string.IsNullOrWhiteSpace(feeTypeName))
                           .AndIf(p => p.IsEnable == isEnable, isEnable.HasValue);
            return _feeTypeRepository.GetList(filter).ToList();
        }

        public void CreateFeeType(FeeType feeType)
        {
            Check.Argument.IsNotNull(feeType, "费用类型");
            Check.Argument.IsNullOrWhiteSpace(feeType.FeeTypeName, "费用名称");
            _feeTypeRepository.Add(feeType);
            _feeTypeRepository.UnitOfWork.Commit();
        }

        public void UpdateFeeType(FeeType feeType)
        {
            Check.Argument.IsNotNull(feeType, "费用类型");
            Check.Argument.IsNullOrWhiteSpace(feeType.FeeTypeName, "费用名称");
            _feeTypeRepository.Modify(feeType);
            _feeTypeRepository.UnitOfWork.Commit();
        }

        public FeeType GetFeeType(int feeTypeId)
        {
            Check.Argument.IsNotNull(feeTypeId, "费用类型ID");
            return _feeTypeRepository.First(p => p.FeeTypeID == feeTypeId);
        }

        public IPagedList<InFeeInfoExt> GetInFeeInfoPagedList(InFeeListParam param, out decimal totalFee)
        {
            param.StartTime = param.StartTime.HasValue ? param.StartTime.Value : new DateTime(2013, 1, 1);
            param.EndTime = param.EndTime.HasValue ? param.EndTime.Value : new DateTime(2020, 1, 1);
            return _wayBillInfoRepository.GetInFeeInfoExtPagedList(param, out totalFee);
        }

        public IPagedList<OutFeeInfoExt> GetOutFeeInfoPagedList(OutFeeListParam param, out decimal totalFee)
        {
            param.StartTime = param.StartTime.HasValue ? param.StartTime.Value : new DateTime(2013, 1, 1);
            param.EndTime = param.EndTime.HasValue ? param.EndTime.Value : new DateTime(2020, 1, 1);
            return _wayBillInfoRepository.GetOutFeeInfoExtPagedList(param, out totalFee);
        }

        public IList<InFeeInfoExt> GetInFeeInfoList(InFeeListParam param, out decimal totalFee)
        {
            param.StartTime = param.StartTime.HasValue ? param.StartTime.Value : new DateTime(2013, 1, 1);
            param.EndTime = param.EndTime.HasValue ? param.EndTime.Value : new DateTime(2020, 1, 1);
            return _wayBillInfoRepository.GetInFeeInfoExtList(param, out totalFee);
        }

        public IList<InFeeTotalInfoExt> GetInFeeTotalInfoList(InFeeTotalListParam param, out decimal totalFee)
        {
            return _wayBillInfoRepository.GetInFeeTotalInfoExtList(param, out totalFee);
        }

        public IList<OutFeeInfoExt> GetOutFeeInfoList(OutFeeListParam param, out decimal totalFee)
        {
            param.StartTime = param.StartTime.HasValue ? param.StartTime.Value : new DateTime(2013, 1, 1);
            param.EndTime = param.EndTime.HasValue ? param.EndTime.Value : new DateTime(2020, 1, 1);
            return _wayBillInfoRepository.GetOutFeeInfoExtList(param, out totalFee);
        }

       

    }
}
