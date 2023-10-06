using System;

namespace ACADBase
{
    public interface IMessageProvider
    {
        void Show(string message);
        void Error(Exception exception);
    }
}