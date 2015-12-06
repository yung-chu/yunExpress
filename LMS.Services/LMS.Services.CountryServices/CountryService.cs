using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Linq.Expressions;
using System.Text;
using LMS.Data.Entity;
using LMS.Data.Repository;

namespace LMS.Services.CountryServices
{
    public class CountryService : ICountryService
    {
        private ICountryRepository _countryRepository;
        private IMouthCountryRepository _mouthCountryRepository;
        public CountryService(ICountryRepository countryRepository, IMouthCountryRepository mouthCountryRepository)
        {
            _countryRepository = countryRepository;
            _mouthCountryRepository = mouthCountryRepository;
        }
        public List<Country> GetCountryList(string keyword)
        {
            Expression<Func<Country, bool>> filter = p => true;
            filter = filter.AndIf(p => p.CountryCode.Contains(keyword) || p.ChineseName.Contains(keyword), !string.IsNullOrWhiteSpace(keyword));
            return _countryRepository.GetList(filter).ToList();
        }

        public List<MouthCountry> GetMouthCountryList()
        {
            return _mouthCountryRepository.GetAll().ToList();
        }

        public Country GetCountryByCode(string Code)
        {
            return _countryRepository.Single(p => p.CountryCode == Code);
        }

        /// <summary>
        /// 获取国家列表包含常用国家
        /// </summary>
        /// <returns></returns>
        public List<CountryExt> GetCommonCountryList()
        {
            return _countryRepository.GetCommonCountryList();
        }
    }
}
