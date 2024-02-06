using System;
using System.Collections;
using System.Collections.Generic;

namespace CommonUtils.Misc
{
    public static class EnumerableExtension
    {
        public static bool IsNullOrEmpty(this IEnumerable enumerable)
        {
            return enumerable == null || !enumerable.GetEnumerator().MoveNext();
        }

        public static CommandResult RunForEach<T>(this Func<T, CommandResult>[] funcs, T t)
        {
            //CommandResult has catch and record exception. No need to catch again.
            var result = new CommandResult();
            if (funcs.IsNullOrEmpty()) return result;
            foreach (var func in funcs)
            {
                result = func(t);
                if (result.IsCancel) break;
            }

            return result;
        }

        public static SortedDictionary<string, CommandResult> TestForEach<T>(this Func<T, CommandResult>[] funcs, T t)
        {
            //CommandResult has catch and record exception. No need to catch again.
            var results = new SortedDictionary<string, CommandResult>();
            if (funcs.IsNullOrEmpty()) return results;
            foreach (var func in funcs)
            {
                var result = func(t);
                results.Add(result.StampString, result);
            }


            return results;
        }
    }
}