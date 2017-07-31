using System.Data.Entity;
using System.Linq;

namespace EmployeeDataApp.Models
{
    public class EmployeeDbContext : DbContext, IEmployeeDbContext
    {
        public EmployeeDbContext()
        {
            this.Configuration.LazyLoadingEnabled = false;
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