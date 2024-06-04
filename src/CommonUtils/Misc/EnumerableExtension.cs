using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace CommonUtils.Misc
{
    public static class EnumerableExtension
    {
        public enum ShowSuccessFuncName
        {
            Show = 1,
            Hide = 0
        }

        public static bool IsNullOrEmpty(this IEnumerable enumerable)
        {
            return enumerable == null || !enumerable.GetEnumerator().MoveNext();
        }

        public static FuncResult RunForEach(this Func<FuncResult>[] funcs,
            IMessageProvider messageProvider = null)
        {
            //CommandResult has record and log exception. No need to log again.
            var result = new FuncResult();
            if (funcs.IsNullOrEmpty()) return result;
            foreach (var func in funcs)
            {
                result = func.RunForOnce(messageProvider);
                if (result.IsCancel) break;
            }

            return result;
        }

        public static FuncResult RunForOnce(this Func<FuncResult> func,
            IMessageProvider messageProvider = null, ShowSuccessFuncName showSuccessFuncName = ShowSuccessFuncName.Hide)
        {
            var result = new FuncResult();
            //try/catch again for safety
            try
            {
                result = func();
                if (result.IsSuccess && showSuccessFuncName == ShowSuccessFuncName.Show)
                    messageProvider?.Show(func.GetMethodInfo().Name);
                if (result.IsCancel) messageProvider?.Error(result.ExceptionInfo);
            }
            catch (Exception e)
            {
                result.Cancel(e);
                messageProvider?.Error(e);
            }

            return result;
        }

        public static FuncResult RunForEach<T>(this Func<T, FuncResult>[] funcs, T t,
            IMessageProvider messageProvider = null)
        {
            //CommandResult has record and log exception. No need to log again.
            var result = new FuncResult();
            if (funcs.IsNullOrEmpty()) return result;
            foreach (var func in funcs)
            {
                result = RunForOnce(func, t, messageProvider);
                if (result.IsCancel) break;
            }

            return result;
        }

        public static SortedDictionary<string, FuncResult> TestForEach<T>(this Func<T, FuncResult>[] funcs, T t,
            IMessageProvider messageProvider = null)
        {
            //CommandResult has catch and record exception. No need to catch again.
            var results = new SortedDictionary<string, FuncResult>();
            if (funcs.IsNullOrEmpty()) return results;
            foreach (var func in funcs)
            {
                var result = func.RunForOnce(t, messageProvider);
                results.Add(result.StampString, result);
            }

            return results;
        }

        public static FuncResult RunForOnce<T>(this Func<T, FuncResult> func, T t,
            IMessageProvider messageProvider = null, ShowSuccessFuncName showSuccessFuncName = ShowSuccessFuncName.Hide)
        {
            var result = new FuncResult();
            //try/catch again for safety
            try
            {
                result = func(t);
                if (result.IsSuccess && showSuccessFuncName == ShowSuccessFuncName.Show)
                    messageProvider?.Show(func.GetMethodInfo().Name);
                if (result.IsCancel) messageProvider?.Error(result.ExceptionInfo);
            }
            catch (Exception e)
            {
                result.Cancel(e);
                messageProvider?.Error(e);
            }

            return result;
        }
    }
}