using System;
using CommonUtils.Misc;

namespace ACADBase
{
    public interface IDwgCommandHelper
    {
        void WriteMessage(string message);
        void ShowError(Exception exception);
        CommandResult ExecuteDatabaseFuncs(params Func<IDatabaseHelper, CommandResult>[] databaseFuncs);
    }
}