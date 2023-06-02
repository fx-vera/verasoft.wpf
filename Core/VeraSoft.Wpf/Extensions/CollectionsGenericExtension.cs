using System.Collections.Generic;

namespace System.Linq
{
    public static class CollectionsGenericExtension
    {
        public static List<TSource> ToList<TSource>(this IEnumerable<TSource> source)
        {
            if (source == null)
                return new List<TSource>();
            return (List<TSource>)source;
        }
    }
}
