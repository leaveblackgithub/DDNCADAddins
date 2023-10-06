using System;
using Autodesk.AutoCAD.DatabaseServices;

namespace ACADBase
{
    public interface IDwgCommandBase
    {
        void ExecuteDataBaseActions(params Action<Database>[] databaseActions);
    }
}