using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeDataClient.Extensions
{
    public static class StringExtensions
    {
        public static bool EqualsAny(this string str, params string[] patterns)
        {
            foreach (string pattern in patterns)
                if (str.Equals(pattern))
                    return true;
            return false;
        }

        public static bool EqualsAny(this string str, StringComparison comparisonType, params string[] patterns)
        {
            foreach (string pattern in patterns)
                if (str.Equals(pattern, comparisonType))
                    return true;
            return false;
        }
    }
}
