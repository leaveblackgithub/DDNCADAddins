using Autodesk.AutoCAD.DatabaseServices;
using CommonUtils;

namespace ACADWrappers.Shared
{
    public static class SymbolTableExtension
    {
        public static bool CycleReadRecord(this SymbolTable table, Transaction transaction,
            SharedDelegate.ActionWithResult<SymbolTableRecord> action)
        {
            return table.Cycle<ObjectId>(id => transaction.ReadDbObject(id, action));
        }
    }
}