using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using EmployeeDataApp.DAL.Entities;

namespace EmployeeDataApp.DAL.Ef
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