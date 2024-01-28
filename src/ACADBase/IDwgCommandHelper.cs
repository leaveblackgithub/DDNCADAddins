using System;
using Autodesk.AutoCAD.DatabaseServices;
using CommonUtils;

namespace ACADBase
{
    public interface IDwgCommandHelper
    {
        void WriteMessage(string message);
        void ShowError(Exception exception);
        CommandResult ExecuteDatabaseFuncs(params Func<DatabaseHelper, CommandResult>[] databaseFuncs);
    }
}