using System;

namespace CommonUtils.Misc
{
    public static class OperationResultExtensions
    {
        public static OperationResult<TOut> Then<TIn, TOut>(this OperationResult<TIn> result,
            Func<OperationResult<TOut>> next)
        {
            if (!result.IsSuccess) return OperationResult<TOut>.Failure(result.ErrorMessage);

            return next();
        }

        public static OperationResult<T> ForEach<T>(this Func<OperationResult<T>>[] funcs)
        {
            if(funcs.IsNullOrEmpty())return OperationResult<T>.Failure(ExceptionMessage.InvalidArguments());
            OperationResult<T> result = default(OperationResult<T>);
            foreach (var func in funcs)
            {
                result=func();
                if(!result.IsSuccess) return result;
            }

            return result;
        }
    }
}