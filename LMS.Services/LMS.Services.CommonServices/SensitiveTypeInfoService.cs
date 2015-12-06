using System.Collections.Generic;
using System.Linq;
using LMS.Data.Entity;
using LMS.Data.Repository;

namespace LMS.Services.CommonServices
{
    public class SensitiveTypeInfoService : ISensitiveTypeInfoService
    {
        private readonly ISensitiveTypeInfoRepository _sensitiveTypeInfoRepository;

        public SensitiveTypeInfoService(ISensitiveTypeInfoRepository sensitiveTypeInfoRepository)
        {
            _sensitiveTypeInfoRepository = sensitiveTypeInfoRepository;
        }

        public List<SensitiveTypeInfo> GetList()
        {
            return _sensitiveTypeInfoRepository.GetAll().ToList();
        }
    }
}