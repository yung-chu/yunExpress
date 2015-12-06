using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using LMS.Data.Context;
using LMS.Data.Entity;
using LighTake.Infrastructure.Seedwork.EF;

namespace LMS.Data.Repository
{
    public partial class CategoryRepository 
    {
        public int UpdateParentPath(IEnumerable<Category> categories)
        {
            LMS_DbContext dbContext = new LMS_DbContext();
            StringBuilder sql = new StringBuilder();

            int index = 0;
            List<SqlParameter> parameters = new List<SqlParameter>();

            foreach (var category in categories)
            {
				sql.AppendFormat(@"update Category set ParentPath=@ParentPath{0},Level={1} where CategoryID={2};", index, category.Level, category.CategoryID);
				parameters.Add(new SqlParameter(@"@ParentPath" + index, category.ParentPath));
			    ++index;
            }


	     	return dbContext.ExecuteCommand(sql.ToString(), parameters.ToArray());

        }
    }
}
