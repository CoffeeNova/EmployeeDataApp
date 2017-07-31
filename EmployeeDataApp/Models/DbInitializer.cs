using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace EmployeeDataApp.Models
{
    public class EmployeeDbInitializer : CreateDatabaseIfNotExists<EmployeeDbContext>
    {
        protected override void Seed(EmployeeDbContext context)
        {
            context.Employees.Add(new EmployeeModel
            {
                FirstName = "TestFirstName",
                LastName = "TestLastName",
                Age = 20,
                Gender = "Male",
                Professions = new List<ProfessionModel>
             {
                 new ProfessionModel
                 {
                     ProfessionName = "TestProfessionOne"
                 },
                 new ProfessionModel
                 {
                     ProfessionName = "TestProfessionTwo"
                 }
             }
            });
            base.Seed(context);
        }
    }

    public class ApplicationDbInitializer : CreateDatabaseIfNotExists<ApplicationDbContext>
    {
        protected override void Seed(ApplicationDbContext context)
        { 
            context.Users.Add(new ApplicationUser
            {
                Email = "service@epam.com",
                Id = "6507dac2-e9da-41a3-9343-b3e9af195dc8",
                PasswordHash = "AAPMaSS+NoZQYywgPCGC4Lu5P5XEBE59pm3pDmBfmf9QId2qjaVUmyRynbFF5ern3w==",
                SecurityStamp = "e8cc03cf-aaf0-4d6a-a3e1-07542f81fc34",
                UserName = "service@epam.com",
            });

            base.Seed(context);
        }
    }
}
