using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LMS.Data.Entity;
using LMS.Data.Repository;

namespace LMS.Services.CommonServices
{
    public interface IGoodsTypeService
    {
        List<GoodsTypeInfo> GetList();
    }

    public class GoodsTypeService : IGoodsTypeService
    {
        private readonly IGoodsTypeInfoRepository _goodsTypeInfoRepository;

        public GoodsTypeService(IGoodsTypeInfoRepository goodsTypeInfoRepository)
        {
            _goodsTypeInfoRepository = goodsTypeInfoRepository;
        }

        public List<GoodsTypeInfo> GetList()
        {
            return _goodsTypeInfoRepository.GetList(p=>p.IsDelete==false).ToList();
        }
    }
}
