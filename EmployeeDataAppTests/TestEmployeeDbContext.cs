//using System;
//using System.Collections.Generic;
//using System.Data.Entity;
//using System.Data.Entity.Infrastructure;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using EmployeeDataApp.Models;

//namespace EmployeeDataApp.Tests
//{
//    public class TestEmployeeDbContext : IEmployeeDbContext
//    {
//        public TestEmployeeDbContext()
//        {
//            Employees = new TestEmployeeDbSet();
//            Professions = new TestProfessionsDbSet();
//        }

//        public DbSet<EmployeeModel> Employees { get; set; }
//        public DbSet<ProfessionModel> Professions { get; set; }
//        public void MarkAsModified(EmployeeModel item)
//        {
//        }

//        public void MarkAsModified(ProfessionModel item)
//        {
//        }

//        public int SaveChanges()
//        {
//            return 0;
//        }

//        public DbEntityEntry Entry(object entity)
//        {
//        }

//        public void Dispose()
//        {
//        }
//    }
//}
