using LMS.Data.Context;
using LighTake.Infrastructure.Seedwork.EF;

namespace LMS.Data.Repository
{
    public class BaseRepository<T> : Repository<T> where T : LighTake.Infrastructure.Seedwork.Entity
    {
        public BaseRepository()
            : base(new LMS_DbContext())
        {

        }

    }
}
