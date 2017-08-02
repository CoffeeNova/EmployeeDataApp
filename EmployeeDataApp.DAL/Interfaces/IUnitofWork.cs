using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EmployeeDataApp.DAL.Entities;

namespace EmployeeDataApp.DAL.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IRepository<EmployeeModel> EmployeeModel { get; }

        IRepository<ProfessionModel> ProfessionModel { get; }
    }
}
