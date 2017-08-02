using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeDataApp.DAL.Entities
{
    public class ProfessionModel
    {
        public string ProfessionName { get; set; }

        public virtual EmployeeModel EmployeeModel { get; set; }

        public int EmployeeModelId { get; set; }

    }
}
