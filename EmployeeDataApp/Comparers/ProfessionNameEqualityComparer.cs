using System.Collections.Generic;
using EmployeeDataApp.Models;

namespace EmployeeDataApp.Comparers
{
    public class ProfessionNameEqualityComparer<T> : IEqualityComparer<T> where T : IProfessionKey
    {
        public bool Equals(T x, T y)
        {
            return x.ProfessionName == y.ProfessionName
            && x.EmployeeModelId == y.EmployeeModelId;
        }

        public int GetHashCode(T obj)
        {
            return obj.ProfessionName.GetHashCode();

        }
    }
}