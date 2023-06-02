using System.Collections.Generic;
using System.Linq;

namespace System.Linq
{
    public static class EnumerableExtension
    {
        // Resumen:
        //     Crea un System.Collections.Generic.List`1 a partir de un System.Collections.Generic.IEnumerable`1.
        //
        // Parámetros:
        //   source:
        //     El System.Collections.Generic.IEnumerable`1 para crear un System.Collections.Generic.List`1
        //     de.
        //
        // Parámetros de tipo:
        //   TSource:
        //     Tipo de los elementos de source.
        //
        // Devuelve:
        //     Un System.Collections.Generic.List`1 que contiene los elementos de la secuencia
        //     de entrada.
        //
        // Excepciones:
        //   T:System.ArgumentNullException:
        //     El valor de source es null.
        public static List<TSource> ToList<TSource>(this IEnumerable<TSource> source, bool makeSafe)
        {
            if (makeSafe && source == null)
                return new List<TSource>();
            else if (source == null)
                throw new ArgumentNullException();
            return (List<TSource>)source;
        }

        public static TSource FirstOrDefault<TSource>(this IEnumerable<TSource> source, bool makeSafe)
        {
            if (source == null || source.ToList().Count == 0)
            {
                return default(TSource);
            }
            else if (makeSafe)
            {
                return default;
            }
            else
            {
                return source.ToList().FirstOrDefault();
            }
        }

    }
}
