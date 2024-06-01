using System;

namespace CommonUtils.DwgLibs
{
    public interface IDwgCommandHelper
    {
        void WriteMessage(string message);
        void ShowError(Exception exception);
    }
}