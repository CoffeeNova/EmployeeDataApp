using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using EmployeeDataApp.Models;

namespace EmployeeDataApp.Helpers
{
    public abstract class EmployeeControllerHelper
    {
        public abstract List<EmployeeModel> FindEmployees(EmployeeModel searchModel);

        public static bool NameValidator(string name)
        {
            if (string.IsNullOrEmpty(name))
                return false;
            if (string.IsNullOrWhiteSpace(name))
                return false;
            if (!IsStringOnlyLetters(name))
                return false;
            return true;
        }

        public static bool AgeValidator(string age, ref int ageInt)
        {
            if (string.IsNullOrEmpty(age))
                return false;

            if (int.TryParse(age, out ageInt))
                return IsAgeInRange(ageInt);
            return false;
        }

        public static bool IsStringOnlyLetters(string str)
        {
            var pattern = @"^[a-zA-Z]+$";
            var regex = new Regex(pattern);
            return regex.IsMatch(str);
        }

        public static bool IsAgeInRange(int age)
        {
            return age >= 14 && age <= 99;
        }

        public static bool IsAgeInRange(string age, out int ageInt)
        {
            if (int.TryParse(age, out ageInt))
                return IsAgeInRange(ageInt);
            return false;
        }

        public static bool IsGenderFromSexParty(string sex)
        {
            return new List<string> { "Male", "Female" }.Any(s => s.Equals(sex, StringComparison.CurrentCultureIgnoreCase));
        }
    }
}