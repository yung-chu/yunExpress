using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Transactions;

using LMS.Data.Context;
using LMS.Data.Entity;
using System.Data.SqlClient;
using System.ComponentModel;
using LighTake.Infrastructure.Common;

namespace LMS.Data.Repository
{
    public partial class TaskRepository
    {
        public bool Save(IEnumerable<Entity.Task> tasks)
        {
            using (var ctx = new LMS_DbContext())
            {
                ctx.Configuration.AutoDetectChangesEnabled = false;
                //ctx.Configuration.ValidateOnSaveEnabled = false;

                using (var transactionScope = new TransactionScope())
                {
                    foreach (var t in tasks)
                    {
                        ctx.Tasks.Add(t);
                    }
                    ctx.SaveChanges();
                    transactionScope.Complete();
                }
                return true;
            }
        }

        public bool Retry(long[] ids)
        {
            if (ids.Any(t => t <= 0))
            {
                throw new BusinessLogicException("任务ID无效.");
            }

            using (var ctx = new LMS_DbContext())
            {
                StringBuilder sb = new StringBuilder();
                foreach (var t in ids)
                {
                    sb.AppendFormat("{0},", t);
                }

                string sql = @"UPDATE Task SET Status=0 WHERE TaskID in ({0})";
                sql = string.Format(sql, sb.ToString().TrimEnd(','));

                var result = ctx.Database.ExecuteSqlCommand(sql);
                ctx.SaveChanges();
                return true;
            }
        }

        public bool Update(long taskId, int status, string error, bool timesIncrease)
        {
            if (taskId <= 0) return false;
            var task = new Task();
            task.TaskID = taskId;
            task.Status = status; //update

            using (var ctx = new LMS_DbContext())
            {
                string fileds = " Status=@Status ";
                var myParams = new List<SqlParameter>();
                myParams.Add(new SqlParameter("Status", status));

                string sql = @"UPDATE dbo.Task SET {0} WHERE TaskID =@TaskID";

                if (!string.IsNullOrWhiteSpace(error))
                {
                    fileds = string.Concat(fileds, " ,Error = @Error +'<br/><br/>'+ Error ");
                    myParams.Add(new SqlParameter("Error", error));
                }
                if (timesIncrease)
                {
                    fileds = string.Concat(fileds, " , Times =Times+1");
                }


                myParams.Add(new SqlParameter("TaskID", taskId));
                var result = ctx.Database.ExecuteSqlCommand(string.Format(sql, fileds), myParams.ToArray());

                //ctx.Tasks.Attach(task);
                //var entry = ctx.Entry(task);
                //entry.State = EntityState.Modified;
                //entry.Property(e => e.TaskKey).IsModified = false; //needs no modification
                //entry.Property(e => e.TaskType).IsModified = false; //needs no modification
                //entry.Property(e => e.Times).IsModified = false; //needs no modification
                //entry.Property(e => e.Error).IsModified = false; //needs no modification
                //entry.Property(e => e.Body).IsModified = false; //needs no modification
                //entry.Property(e => e.CreateOn).IsModified = false; //needs no modification

                ctx.SaveChanges();
                return true;
            }
        }

        public bool Delete(long taskId)
        {
            if (taskId <= 0) return false;
            var task = new Task();
            task.TaskID = taskId;

            using (var ctx = new LMS_DbContext())
            {
                ctx.Tasks.Attach(task);

                var entry = ctx.Entry(task);
                entry.State = System.Data.Entity.EntityState.Deleted;

                ctx.SaveChanges();
                return true;
            }
        }

        public IPagedList<Task> List(int taskType, int status, string taskKey, int pageIndex, int pageSize = 50)
        {
            using (var ctx = new LMS_DbContext())
            {
                IQueryable<Task> q;
                if (!string.IsNullOrWhiteSpace(taskKey))
                {
                    q = ctx.Tasks.Where(
                        t => t.TaskType == taskType &&
                           t.Status == status &&
                           t.TaskKey == taskKey.Trim());
                }
                else
                {
                    q = ctx.Tasks.Where(
                        t => t.TaskType == taskType &&
                        t.Status == status);
                }
                return q.OrderBy(t => t.TaskID).ToPagedList<Task>(pageIndex, pageSize);
            }
        }

    }
}
