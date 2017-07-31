using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EmployeeDataApp.Models
{
    public interface IProfessionKey
    {
        string ProfessionName { get; set; }

        int EmployeeModelId { get; set; }
    }
}