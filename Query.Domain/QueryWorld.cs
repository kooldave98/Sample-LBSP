using System.Data.Entity;
using CodeStructures;
using LbspSOA;
using Query.Interface;

namespace Query.Domain
{
    public class QueryWorld : IWorld
    {
        public DbContext db_context;

        public QueryWorld(DbContext db_context)
        {
            this.db_context = Guard.IsNotNull(db_context, nameof(db_context));
        }

        public static QueryWorld seed_world()
        {
            return new QueryWorld(new AppDbContext());
        }
    }
}
