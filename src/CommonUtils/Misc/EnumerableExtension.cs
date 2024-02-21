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

        public static CommandResult RunForEach<T>(this Func<T, CommandResult>[] funcs, T t,
            IMessageProvider messageProvider = null)
        {
            //CommandResult has record and log exception. No need to log again.
            var result = new CommandResult();
            if (funcs.IsNullOrEmpty()) return result;
            foreach (var func in funcs)
            {
               result= RunForOnce(func, t, messageProvider);
               if (result.IsCancel) break;
            }
            return result;
        }

        public static SortedDictionary<string, CommandResult> TestForEach<T>(this Func<T, CommandResult>[] funcs, T t,
            IMessageProvider messageProvider = null)
        {
            //CommandResult has catch and record exception. No need to catch again.
            var results = new SortedDictionary<string, CommandResult>();
            if (funcs.IsNullOrEmpty()) return results;
            foreach (var func in funcs)
            {
                var result = RunForOnce(func, t, messageProvider);
                results.Add(result.StampString, result);
            }
            return results;
        }

        public static CommandResult RunForOnce<T>(Func<T, CommandResult> func, T t,
            IMessageProvider messageProvider = null)
        {
            var result = new CommandResult();
            try
            {
                result = func(t);
            }
            catch (Exception e)
            {
                result.Cancel(e);
                messageProvider?.Error(result.ExceptionInfo);
            }
            return result;
        }
    }
}