using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EmployeeDataApp.Models;

namespace EmployeeDataApp.Extensions
{
    public static class CollectionExtension
    {
        public static IEnumerable<TSource> Unique<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            var seenKeys = new HashSet<TKey>();
            foreach (TSource element in source)
            {
                if (seenKeys.Add(keySelector(element)))
                {
                    yield return element;
                }
            }
        }

        //public static List<EmployeeModel> UniqueEmployee<EmployeeModel, EmployeeModel>(this List<EmployeeModel> source, Func)
    }
}