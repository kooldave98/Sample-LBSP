using System.Data.Entity;
using System.Linq;
using CodeStructures;

namespace Query.Interface
{
    public class Repository<T> where T : class
    {
        public Repository(DbContext context)
        {
            this.context = Guard.IsNotNull(context, nameof(context));
        }

        public IQueryable<T> Entities { get { return context.Set<T>(); } }

        public void add(T entity)
        {
            context.Set<T>().Add(entity);
        }

        public void remove(T entity)
        {
            context.Set<T>().Attach(entity);
            context.Entry(entity).State = EntityState.Deleted;
            context.Set<T>().Remove(entity);
        }

        public void remove_all()
        {
            foreach (var item in context.Set<T>())
            {
                remove(item);
            }
        }

        private readonly DbContext context;

    }
}
