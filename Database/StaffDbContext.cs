using Microsoft.EntityFrameworkCore;

namespace Staff.Database
{
    internal class StaffDbContext : DbContext
    {
        public DbSet<Employee> Employees => Set<Employee>();
        public DbSet<Department> Departments => Set<Department>();

        public StaffDbContext(DbContextOptions<StaffDbContext> options)
            : base(options)
        {
        }

        //  This doesn't work for in-mem dbs; change to SQLite :memory: if you want it to work.
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Department>()
                .HasIndex(d => d.Name)
                .IsUnique();
        }
    }
}
