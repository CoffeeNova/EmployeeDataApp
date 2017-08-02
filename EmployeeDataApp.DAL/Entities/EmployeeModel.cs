using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeDataApp.DAL.Entities
{
    public class EmployeeModel
    {
        public int Id { get; set; }
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public int Age { get; set; }

        public string Gender { get; set; }

        public virtual ICollection<ProfessionModel> Professions { get; set; } = new List<ProfessionModel>();
    }
}
