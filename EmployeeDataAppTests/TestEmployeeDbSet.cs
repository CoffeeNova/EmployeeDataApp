using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EmployeeDataApp.Models;

namespace EmployeeDataApp.Tests
{
    public class TestEmployeeDbSet : TestDbSet<EmployeeModel>
    {
        public override EmployeeModel Find(params object[] keyValues)
        {
            return this.SingleOrDefault(model => model.Id == (int)keyValues.Single());
        }
    }

    public class TestProfessionsDbSet : TestDbSet<ProfessionModel>
    {
        public override ProfessionModel Find(params object[] keyValues)
        {
            return this.SingleOrDefault(model => model.EmployeeModelId == (int)keyValues[0]
                        && model.ProfessionName == (string)keyValues[1]);
        }
    }
}
