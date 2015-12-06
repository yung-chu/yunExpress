using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using LMS.Data.Entity;
using LMS.Data.Repository;
using LighTake.Infrastructure.Common;

namespace LMS.Services.HomeServices
{
    public class HomeService : IHomeService
    {
        private readonly ICategoryRepository _categoryRepository;

        public HomeService(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public Category GetCategory(int categoryId)
        {
            return _categoryRepository.Get(categoryId);
        }

        public Category GetCategory(string categoryName)
        {
            Expression<Func<Category, bool>> filter = p => true;
            filter = filter.AndIf(p => p.Name == categoryName, !string.IsNullOrWhiteSpace(categoryName));
            return _categoryRepository.Single(filter);
        }

        public List<Category> GetAllChildCategoryList(int parentId)
        {
            Expression<Func<Category, bool>> filter = x => true;
            filter = filter.AndIf(x => GetParentPathCondition(x.ParentPath).Contains(GetParentPathCondition(parentId.ToString())), parentId > 0);

            return _categoryRepository.GetFiltered(filter, x => x.OrderBy(x2 => x2.Sort).ThenBy(x2 => x2.CategoryID)).ToList();
        }

        public List<Category> GetCategoryList(int parentId)
        {
            return _categoryRepository.GetList(p => p.ParentID == parentId);
        }

        public bool SetCategorySort(int categoryId, bool upward)
        {
            bool result = false;
            using (var transaction = new TransactionScope())
            {
                var productcategory = _categoryRepository.First(p => p.CategoryID == categoryId);
                if (productcategory == null)
                {
                    return result;
                }
                Category category = null;

                //排序向前
                if (upward)
                {
                    Func<IQueryable<Category>, IOrderedQueryable<Category>> orderBy = o => o.OrderByDescending(g => g.Sort);
                    category = _categoryRepository.First(p => p.ParentID == productcategory.ParentID && p.CategoryID != productcategory.CategoryID &&
                            p.Sort <= productcategory.Sort, orderBy);

                }
                else
                {
                    Func<IQueryable<Category>, IOrderedQueryable<Category>> orderBy = o => o.OrderBy(g => g.Sort);
                    category = _categoryRepository.First(p =>
                                                                                p.ParentID == productcategory.ParentID &&
                                                                                p.CategoryID != productcategory.CategoryID &&
                                                                                p.Sort >= productcategory.Sort, orderBy);

                }

                if (category != null)
                {
                    int oldsort = productcategory.Sort;
                    int newsort = category.Sort;
                    if (oldsort == newsort)
                    {
                        if (upward)
                        {
                            --newsort;
                        }
                        else
                        {
                            ++newsort;
                        }
                    }
                    category.Sort = oldsort;
                    productcategory.Sort = newsort;
                    category.LastUpdatedOn = productcategory.LastUpdatedOn = DateTime.Now;
                    _categoryRepository.Modify(category);
                    _categoryRepository.Modify(productcategory);
                    _categoryRepository.UnitOfWork.Commit();
                    result = true;
                }
                //else
                //{
                //    result = _categoryRepository.Count(x => x.ParentID == productcategory.ParentID) == 1;
                //}

                transaction.Complete();
            }
            return result;
        }

        /// <summary>
        /// 添加分类
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        public bool AddCategory(Category category)
        {
            int categoryId = _categoryRepository.GetFiltered(x => true).Max(x => x.CategoryID) + 1;

            category.CategoryID = categoryId;
            if (category.ParentID == 0)
            {
                category.Level = 1;
                category.ParentPath = category.CategoryID.ToString();
            }
            else
            {
                var cate = GetCategory(category.ParentID);
                if (cate == null)
                {
                    return false;
                }
                category.Level = cate.Level + 1;
                category.ParentPath = cate.ParentPath + "," + category.CategoryID;
            }

            int index = SortCategory(category.ParentID);

            int sort = index != 0 ? ++index : (int)_categoryRepository.Count(x => x.ParentID == category.ParentID) + 1;

            category.Sort = sort;
            _categoryRepository.Add(category);
            _categoryRepository.UnitOfWork.Commit();

            return true;
        }

        private int SortCategory(int parentId)
        {
            int index = 0;
            var cates = _categoryRepository.GetFiltered(x => x.ParentID == parentId);
            if (cates.Count() != cates.GroupBy(x => x.Sort).Select(x => x.Key).Count())
            {
                foreach (var cate in cates)
                {
                    cate.Sort = ++index;
                    _categoryRepository.Modify(cate);
                }
            }
            return index;
        }

        /// <summary>
        /// 修改分类
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        public int UpdateCategory(Category category)
        {
            int categoryId = category.CategoryID;

            int index = SortCategory(category.ParentID);
            if (index > 0)
                _categoryRepository.UnitOfWork.Commit();

            var cate = GetCategory(categoryId);
            if (cate == null)
            {
                return 0;
            }

            cate.Name = category.Name;
            cate.EnglishName = category.EnglishName;
            cate.Status = category.Status;
            if (!string.IsNullOrWhiteSpace(category.Pic))
            {
                cate.Pic = category.Pic;
            }
	        cate.SeoTitle = category.SeoTitle;
            cate.SeoKeywords = category.SeoKeywords;
            cate.SeoDescription = category.SeoDescription;
            cate.LastUpdatedOn = category.LastUpdatedOn;
            cate.LastUpdatedBy = category.LastUpdatedBy;
            cate.ParentID = category.ParentID;
            cate.Description = category.Description;

            if (category.ParentID == 0)
            {
                cate.Level = 1;
                cate.ParentPath = category.CategoryID.ToString();
            }
            else
            {
                var cate2 = GetCategory(category.ParentID);
                if (cate2 == null)
                {
                    return 0;
                }
                if (GetParentPathCondition(cate2.ParentPath).Contains(GetParentPathCondition(categoryId.ToString())))
                {
                    return 2; //不能将分类修改为其下级分类的子类
                }
                cate.Level = cate2.Level + 1;
                cate.ParentPath = cate2.ParentPath + "," + category.CategoryID;
            }
            _categoryRepository.Modify(cate);
            _categoryRepository.UnitOfWork.Commit();

            //更新
            UpdateParentPath(category);

            return 1;
        }

        public bool HasEnableChild(int categoryId)
        {
            var cate = GetCategory(categoryId);
            if (cate == null)
            {
                return false;
            }
            var exist = _categoryRepository.GetFiltered(x => GetParentPathCondition(x.ParentPath).Contains(GetParentPathCondition(categoryId.ToString())) && x.Status == (int)EnableStatus.Enabled).Any();
            if (exist)
            {
                return true;
            }
            return false;

        }

        /// <summary>
        /// 更新父分类路径
        /// </summary>
        /// <param name="category"></param>
        private void UpdateParentPath(Category category)
        {
            int categoryId = category.CategoryID;
            string path = category.ParentPath;
            int level = category.Level;

            var cates = _categoryRepository.GetFiltered(x => x.ParentID == categoryId);
            foreach (var cate in cates)
            {
                cate.ParentPath = path + "," + cate.CategoryID;
                cate.Level = level + 1;
                //_categoryRepository.Modify(cate);
            }
            if (cates.Any())
                _categoryRepository.UpdateParentPath(cates);
            foreach (var cate in cates)
            {
                UpdateParentPath(cate);
            }
        }


        /// <summary>
        /// 删除分类
        /// </summary>
        /// <param name="categoryId"></param>
        /// <returns></returns>
        public int DeleteCategory(int categoryId)
        {
            var pcate = _categoryRepository.GetFiltered(x => x.ParentID == categoryId).Any();
            if (pcate)
            {
                return 2;
            }
            _categoryRepository.Remove(x => x.CategoryID == categoryId);
            _categoryRepository.UnitOfWork.Commit();
            return 1;
        }

        private string GetParentPathCondition(string path)
        {
            return ",{0},".FormatWith(path);
        }


    }
}
