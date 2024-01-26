using System;
using System.Collections;

namespace CommonUtils
{
    public static class EnumerableExtension
    {
        public static void Cycle<T>(this IEnumerable enumerable, Action<T> action)
        {
            var enumerator = enumerable.GetEnumerator();
            while (enumerator.MoveNext()) action((T)enumerator.Current);
        }
    }
}