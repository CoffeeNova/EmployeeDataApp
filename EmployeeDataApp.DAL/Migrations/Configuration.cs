using System.Data.Entity.Migrations;

namespace EmployeeDataApp.DAL.Migrations
{
    internal sealed class Configuration : DbMigrationsConfiguration<EmployeeDataApp.DAL.Ef.EmployeeDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(EmployeeDataApp.DAL.Ef.EmployeeDbContext context)
        {

        }
    }
}
