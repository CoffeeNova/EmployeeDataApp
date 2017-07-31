using System.Collections.Generic;
using EmployeeDataApp.Models;

namespace EmployeeDataApp.Comparers
{
    public class EmployeeEqualityComparer<T> : IEqualityComparer<T> where T : EmployeeModel
    {
        public bool Equals(T x, T y)
        {
            return x.Id == y.Id;
        }

        public int GetHashCode(T obj)
        {
            return obj.Id.GetHashCode();
        }
    }
}