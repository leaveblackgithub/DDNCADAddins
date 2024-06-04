using System;
using CommonUtils.Misc;

namespace CommonUtils.DwgLibs
{
    public interface IDwgCommandHelper
    {
        void WriteMessage(string message);
        void ShowError(Exception exception);
        FuncResult ExecuteMain();
    }
}