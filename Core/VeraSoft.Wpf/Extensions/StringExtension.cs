using System;
using System.Collections.Generic;

namespace VeraSoft.Wpf.Extensions
{
    public static class StringExtension
    {
        public static bool Contains(this string source, string toCheck, StringComparison comp)
        {
            return source?.IndexOf(toCheck, comp) >= 0;
        }
    }

    public static class ListExtension
    {
        public static bool Contains(this List<string> source, string toCheck, StringComparison comp)
        {
            foreach (var sourceItem in source)
            {
                if (sourceItem?.IndexOf(toCheck, comp) >= 0)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
