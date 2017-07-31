using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using EmployeeDataApp.Models;

namespace EmployeeDataApp.Helpers
{
    public class ClientHelper : EmployeeControllerHelper
    {
        public ClientHelper(IEmployeeDbContext context)
        {
            _context = context;
        }

        public override List<EmployeeModel> FindEmployees(EmployeeModel searchModel)
        {
            if (_context == null)
                throw new InvalidOperationException($"Please define {nameof(searchModel)} first.");

            var searchProf = searchModel.Professions.Select(p => p.ProfessionName).ToList();
            return _context.Employees
                .Where(model => searchModel.FirstName == null || model.FirstName == searchModel.FirstName)
                .Where(model => searchModel.LastName == null || model.LastName == searchModel.LastName)
                .Where(model => searchModel.Age == 0 || model.Age == searchModel.Age)
                .Where(model => searchModel.Gender == null || model.Gender == searchModel.Gender)
                .Where(eModel => searchProf.FirstOrDefault() == null ||
                                 searchProf.All(profName => eModel.Professions.Any(profModel => profModel.ProfessionName == profName)))
                .Include(model => model.Professions).ToList();
        }

        private readonly IEmployeeDbContext _context;
    }
}