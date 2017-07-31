using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web;

namespace EmployeeDataApp.Models
{
    public interface IEmployeeDbContext : IDisposable
    {
        DbSet<EmployeeModel> Employees { get; set; }

        DbSet<ProfessionModel> Professions { get; set; }

        void MarkAsModified(EmployeeModel item);

        void MarkAsModified(ProfessionModel item);

        EmployeeModel FindModelById(int id);

        int SaveChanges();

        DbEntityEntry Entry(object entity);
    }
}