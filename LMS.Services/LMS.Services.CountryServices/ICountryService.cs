using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LMS.Data.Entity;
namespace LMS.Services.CountryServices
{
    public interface ICountryService
    {
        List<Country> GetCountryList(string keyword);
        List<MouthCountry> GetMouthCountryList();
        List<CountryExt> GetCommonCountryList();
        Country GetCountryByCode(string Code);
    }
}
