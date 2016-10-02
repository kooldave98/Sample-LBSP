using System.Linq;
using System.Data.Entity;

namespace Query.Service
{
    public class Repository<T> where T : class
    {
        public Repository()
        {
            context = new AppDbContext();
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

        public void commit()
        {
            context.SaveChanges();
        }

        private readonly AppDbContext context;

    }

    public class AppDbContext : DbContext
    {
        public AppDbContext()
            : base("name=AppDbContext")
        {

            InitialiseDatabase();
        }

        public DbSet<Host> Hosts { get; set; }


        private void InitialiseDatabase()
        {
            Database.SetInitializer(new CreateDatabaseIfNotExists<AppDbContext>());
            Database.Initialize(false);
        }
    }

}