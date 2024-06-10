using System;
using System.Collections;

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

        public static OperationResult<VoidValue> RunForEach<T>(this Func<T, OperationResult<VoidValue>>[] funcs, T t)
        {
            var result = OperationResult<VoidValue>.Success();
            //CommandResult has record and log exception. No need to log again.
            if (funcs.IsNullOrEmpty()) return result;
            foreach (var func in funcs)
            {
                result = result.Then(() => func(t));
                if (!result.IsSuccess) break;
            }

            return result;
        }
    }
}