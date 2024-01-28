using System;
using System.Collections;
using NLog;

namespace CommonUtils.Misc
{
    public static class EnumerableExtension
    {
        public static bool IsNullOrEmpty(this IEnumerable enumerable)
        {
            return enumerable == null || !enumerable.GetEnumerator().MoveNext();
        }

        public static CommandResult RunForEach<T>(this Func<T, CommandResult>[] funcs,T t)
        {
            CommandResult result = new CommandResult();
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
                LogManager.GetCurrentClassLogger().Error(e);
            }
            return result;
        }
    }
}