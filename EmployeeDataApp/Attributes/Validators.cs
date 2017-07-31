using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using EmployeeDataApp.Controllers;
using EmployeeDataApp.Helpers;

namespace EmployeeDataApp.Attributes
{
    public class RequiredNameAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            var str = value as string;
            if (str == null)
                return false;

            return EmployeeControllerHelper.NameValidator(str);
        }
    }

    public class RequiredGenderAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            var str = value as string;
            if (str == null)
                return true;

            return EmployeeControllerHelper.IsGenderFromSexParty(str);
        }
    }
}