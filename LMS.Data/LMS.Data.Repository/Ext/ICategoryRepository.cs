using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LMS.Data.Entity;

namespace LMS.Data.Repository
{
    public partial interface ICategoryRepository
    {
        int UpdateParentPath(IEnumerable<Category> categories);
    }
}
