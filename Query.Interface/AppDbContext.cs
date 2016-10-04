using System.Data.Entity;

namespace Query.Interface
{
    public class AppDbContext : DbContext
    {
        public AppDbContext() : base("name=AppDbContext")
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