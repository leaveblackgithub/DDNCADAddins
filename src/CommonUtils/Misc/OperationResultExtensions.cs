﻿using System;

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
    }
}