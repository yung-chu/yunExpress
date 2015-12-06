using System.Collections.Generic;
using LMS.Data.Entity;

namespace LMS.Services.CommonServices
{
    public interface ISensitiveTypeInfoService
    {
        List<SensitiveTypeInfo> GetList();
    }
}