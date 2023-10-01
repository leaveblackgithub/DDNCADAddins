using System;
using Autodesk.AutoCAD.DatabaseServices;
using CommonUtils;

namespace ACADWrappers.Shared
{
    public static class SymbolTableExtension
    {
        public static void CycleReadRecord(this SymbolTable table, Transaction transaction,
            Action<SymbolTableRecord> action)
        {
            table.Cycle<ObjectId>(id => transaction.ReadDbObject(id, action));
        }
    }
}