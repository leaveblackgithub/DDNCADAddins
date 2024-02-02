using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using NLog;

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
            var result = new CommandResult();
            try
            {
                if (!funcs.IsNullOrEmpty())
                    foreach (var func in funcs)
                    {
                        result = func(t);
                        if (result.IsCancel) break;
                    }
            }
            catch (Exception e)
            {
                result.Cancel(e);
            }

            return result;
        }

        public static IDictionary<string, CommandResult> TestForEach<T>(this Func<T, CommandResult>[] funcs, T t)
        {
            var results = new Dictionary<string, CommandResult>();
            if (!funcs.IsNullOrEmpty())
                foreach (var func in funcs)
                {
                    var result = new CommandResult(func.GetMethodInfo().Name);
                    try
                    {
                        func(t);
                    }
                    catch (Exception e)
                    {
                        result.Cancel(e);
                    }
                    finally
                    {
                        results.Add(result.StampString, result);
                    }
                }


            return results;
        }
    }
}