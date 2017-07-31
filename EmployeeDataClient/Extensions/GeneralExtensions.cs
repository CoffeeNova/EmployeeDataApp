using System.Collections.Generic;

namespace EmployeeDataClient.Extensions
{
    public static class GeneralExtensions
    {
        public static bool EqualsAny<T>(this T subj, params T[] patterns)
        {
            foreach (var pattern in patterns)
                if (subj.Equals(pattern))
                    return true;
            return false;
        }

        public static bool EqualsAny<ConsoleKey>(this ConsoleKey key, ICollection<ConsoleKey> collection)
        {
            foreach (var k in collection)
                if (key.Equals(k))
                    return true;
            return false;
        }
    }
}
