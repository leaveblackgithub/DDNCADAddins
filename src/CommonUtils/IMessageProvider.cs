using System;

namespace CommonUtils
{
    public interface IMessageProvider
    {
        void Show(string message);
        void Error(Exception exception);
    }
}