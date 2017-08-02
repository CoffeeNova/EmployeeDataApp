using System.Data.Entity;
using System.Linq;
using EmployeeDataApp.DAL.Entities;

namespace EmployeeDataApp.DAL.Ef
{
    public class EmployeeDbContext : DbContext, IEmployeeDbContext
    {
        public EmployeeDbContext()
        {
            this.Configuration.LazyLoadingEnabled = false;
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<EmployeeDbContext, EmployeeDataApp.DAL.Migrations.Configuration>());
        }

        public EmployeeDbContext(string connectionString) : base(connectionString)
        {
            this.Configuration.LazyLoadingEnabled = false;
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<EmployeeDbContext, EmployeeDataApp.DAL.Migrations.Configuration>());
        }

        public DbSet<EmployeeModel> Employees { get; set; }

        public DbSet<ProfessionModel> Professions { get; set; }

        public void MarkAsModified(EmployeeModel item)
        {
            Entry(item).State = EntityState.Modified;
        }

        public void MarkAsModified(ProfessionModel item)
        {
            Entry(item).State = EntityState.Modified;
        }

        public EmployeeModel FindModelById(int id)
        {
            return Employees
                .Where(model => model.Id == id)
                .Include(model => model.Professions)
                .SingleOrDefault();
        }

    }
}