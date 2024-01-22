using System;
using Autodesk.AutoCAD.DatabaseServices;

namespace ACADBase
{
    public interface IDwgCommandHelper
    {
        void ExecuteDataBaseActions(params Action<Database>[] databaseActions);
        void WriteMessage(string message);
        void ShowError(Exception exception);
    }
}