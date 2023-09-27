using System;
using Autodesk.AutoCAD.DatabaseServices;
using CommonUtils;

namespace ACADWrappers.Shared
{
    public static class SymbolTableExtension
    {
        public static bool CycleRecord(this SymbolTable table, Transaction transaction, Action<SymbolTableRecord> action) 
        {
            return table.Cycle<ObjectId>(id => transaction.Read<SymbolTableRecord>(id, (Action<SymbolTableRecord>)action));
        }
    }
}