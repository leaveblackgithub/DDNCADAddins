using System;

namespace CommonUtils.Misc
{
    public interface IMessageProvider
    {
        void Show(string message);
        void Error(Exception exception);
    }
}