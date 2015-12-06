using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LMS.Data.Entity;

namespace LMS.Services.HomeServices
{
    public interface IHomeService
    {
        Category GetCategory(int categoryId);

        Category GetCategory(string categoryName);

        List<Category> GetAllChildCategoryList(int parentId);

        List<Category> GetCategoryList(int parentId);

        bool SetCategorySort(int categoryId, bool upward);

        /// <summary>
        /// 添加分类
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        bool AddCategory(Category category);

        /// <summary>
        /// 修改分类
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        int UpdateCategory(Category category);

        /// <summary>
        /// 删除分类
        /// </summary>
        /// <param name="categoryId"></param>
        /// <returns></returns>
        int DeleteCategory(int categoryId);

        bool HasEnableChild(int categoryId);

    }
}
