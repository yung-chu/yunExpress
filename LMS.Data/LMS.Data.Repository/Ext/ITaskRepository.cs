using LighTake.Infrastructure.Common;
using LMS.Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LMS.Data.Repository
{
    public partial interface ITaskRepository
    {
        /// <summary>
        /// 保存任务
        /// </summary>
        /// <param name="tasks"></param>
        /// <returns></returns>
        bool Save(IEnumerable<Task> tasks);

       /// <summary>
       /// 修改
       /// </summary>
       /// <param name="taskId"></param>
       /// <param name="status">状态-1失败,0未处理,1处理中</param>
       /// <param name="error"></param>
       /// <param name="timesIncrease"></param>
       /// <returns></returns>
        bool Update(long taskId, int status, string error, bool timesIncrease);

        /// <summary>
        /// 删除一个任务
        /// </summary>
        /// <param name="taskId"></param>
        /// <returns></returns>
        bool Delete(long taskId);

        /// <summary>
        /// 再次提交
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        bool Retry(long[] ids);

        /// <summary>
        /// 查询任务
        /// </summary>
        /// <param name="taskType">类型</param>
        /// <param name="status">状态</param>
        /// <param name="taskKey">key</param>
        /// <param name="pageIndex">pageIndex</param>
        /// <param name="pageSize">pageSize</param>
        /// <returns></returns>
        IPagedList<Task> List(int taskType, int status, string taskKey, int pageIndex, int pageSize = 50);

    }
}
