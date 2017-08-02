using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using EmployeeDataApp.DAL.Ef;
using EmployeeDataApp.DAL.Entities;
using EmployeeDataApp.DAL.Interfaces;

namespace EmployeeDataApp.DAL.Repositories
{
    public class EmployeeModelRepository : IRepository<EmployeeModel>
    {
        public EmployeeModelRepository(EmployeeDbContext context)
        {
            _db = context;
        }
        public IEnumerable<EmployeeModel> GetAll()
        {
            throw new NotImplementedException();
        }

        public EmployeeModel Get(int id)
        {
            return  _db.Employees
                .Where(model => model.Id == id)
                .Include(model => model.Professions)
                .SingleOrDefault();
        }

        public IEnumerable<EmployeeModel> Find(Func<EmployeeModel, bool> predicate)
        {
            return _db.Employees
                .Include(model => model.Professions)
                .Where(predicate);

        }

        public void Create(EmployeeModel item)
        {
            _db.Employees.Add(item);
        }

        public void Update(EmployeeModel item)
        {
            _db.Entry(item).State = EntityState.Modified;
        }

        public void Delete(int id)
        {
            var item = _db.Employees
                .Where(model => model.Id == id)
                .Include(model => model.Professions)
                .SingleOrDefault();

            if (item != null)
                _db.Employees.Remove(item);
        }

        private EmployeeDbContext _db;
    }
}
