using System;
using System.Runtime.ExceptionServices;

namespace CommonUtils.Misc
{
    public interface IMessageProvider
    {
        void Show(string message);
        void Error(Exception exception);
        void Error(ExceptionDispatchInfo exceptionInfo);
    }
}