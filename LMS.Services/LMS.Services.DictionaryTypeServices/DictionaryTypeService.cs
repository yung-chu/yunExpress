using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using LMS.Core;
using LMS.Data.Entity;
using LMS.Data.Repository;
using LighTake.Infrastructure.Common;

namespace LMS.Services.DictionaryTypeServices
{
   public class DictionaryTypeService:IDictionaryTypeService
    {

        private readonly IDictionaryTypeRepository _dictionaryTypeRepository;
        private readonly IWorkContext _workContext;

       public DictionaryTypeService(IDictionaryTypeRepository dictionaryTypeRepository,IWorkContext workContext)
       {
           _dictionaryTypeRepository = dictionaryTypeRepository;
           _workContext = workContext;
       }

       public void AddDictionaryType(DictionaryType dictionaryType)
        {
            Check.Argument.IsNotNull(dictionaryType, "字典类型");
            dictionaryType.CreatedBy = _workContext.User.UserUame;
            dictionaryType.CreatedOn = DateTime.Now;
            dictionaryType.LastUpdatedBy = _workContext.User.UserUame;
            dictionaryType.LastUpdatedOn = DateTime.Now;
            _dictionaryTypeRepository.Add(dictionaryType);
            _dictionaryTypeRepository.UnitOfWork.Commit();
        }

        public void UpdateDictionaryType(DictionaryType dictionaryType)
        {
            Check.Argument.IsNotNull(dictionaryType, "字典类型");
            Check.Argument.IsNullOrWhiteSpace(dictionaryType.DicTypeId, "字典类型ID");
            if (_dictionaryTypeRepository.Exists(p => p.DicTypeId == dictionaryType.DicTypeId))
            {
                throw new ArgumentException("该字典类型表:不存在ID为:{0}".FormatWith(dictionaryType.DicTypeId));
            }
            dictionaryType.LastUpdatedBy = _workContext.User.UserUame;
            dictionaryType.LastUpdatedOn = DateTime.Now;
            _dictionaryTypeRepository.Modify(dictionaryType);
            _dictionaryTypeRepository.UnitOfWork.Commit();
        }

        public IList<DictionaryType> GetList(DictionaryTypeListParam param)
        {
            Expression<Func<DictionaryType, bool>> filter = p => true;
            filter = filter.AndIf(p => p.ParentId == param.ParentId, !string.IsNullOrWhiteSpace(param.ParentId))
                                  .AndIf(p=>p.IsEnable==param.IsEnable,true)
                                  .AndIf(p => p.IsDelete == param.IsDelete, true)
                                  .AndIf(p => p.IsParent == param.IsDelete, true);
            Func<IQueryable<DictionaryType>, IOrderedQueryable<DictionaryType>>
              orderBy = o => o.OrderByDescending(p => p.CreatedOn);
            return _dictionaryTypeRepository.GetList(filter);
        }

        public string GetName(string dicTypeId)
        {
            return _dictionaryTypeRepository.First(p=>p.DicTypeId==dicTypeId).Name;
        }

        /// <summary>
        /// 获取字典类型
        /// </summary>
        /// <param name="dictionaryTypeParentId">父节点ID</param>
        /// <returns></returns>
        public List<SelectListItem> GetSelectList(string dictionaryTypeParentId)
        {
            return GetSelectList(dictionaryTypeParentId,"");
        }

        /// <summary>
        /// 获取字典类型
        /// </summary>
        /// <param name="dictionaryTypeParentId">父节点ID</param>
        /// <param name="dictionaryTypeId">字典类型ID</param>
        /// <returns></returns>
        public List<SelectListItem> GetSelectList(string dictionaryTypeParentId, string dictionaryTypeId)
        {
            var param = new DictionaryTypeListParam()
            {
                ParentId = dictionaryTypeParentId
            };
            var selectListItems = new List<SelectListItem>();
            this.GetList(param).ToList()
                           .ForEach(p => selectListItems.Add(new SelectListItem()
                           {
                               Text = p.Name,
                               Value = p.DicTypeId,
                               Selected = p.DicTypeId == dictionaryTypeId
                           }));
            return selectListItems;
        }



    }
}
