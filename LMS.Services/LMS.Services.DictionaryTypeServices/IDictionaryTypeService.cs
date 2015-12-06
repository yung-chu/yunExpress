using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using LMS.Data.Entity;

namespace LMS.Services.DictionaryTypeServices
{
    public interface IDictionaryTypeService
    {
        void AddDictionaryType(DictionaryType dictionaryType);

        void UpdateDictionaryType(DictionaryType dictionaryType);

        IList<DictionaryType> GetList(DictionaryTypeListParam param);

        /// <summary>
        ///获取类型名称
        /// </summary>
        /// <param name="dicTypeId">字典类型ID</param>
        /// <returns></returns>
        string GetName(string dicTypeId);

        /// <summary>
        /// 获取字典类型
        /// </summary>
        /// <param name="dictionaryTypeParentId">父节点ID</param>
        /// <returns></returns>
        List<SelectListItem> GetSelectList(string dictionaryTypeParentId);

        /// <summary>
        /// 获取字典类型
        /// </summary>
        /// <param name="dictionaryTypeParentId">父节点ID</param>
        /// <param name="dictionaryTypeId">字典类型ID</param>
        /// <returns></returns>
        List<SelectListItem> GetSelectList(string dictionaryTypeParentId, string dictionaryTypeId);

    }
}
