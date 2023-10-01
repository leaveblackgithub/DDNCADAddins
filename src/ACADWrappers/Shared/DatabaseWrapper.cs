using System;
using System.Collections.Generic;
using Autodesk.AutoCAD.DatabaseServices;
using CommonUtils;
using Domain.Shared;

namespace ACADWrappers.Shared
{
    public class DatabaseWrapper : IDatabaseWrapper
    {
        public DatabaseWrapper(Database dwgDatabase)
        {
            DwgDatabase = dwgDatabase;
        }

        public Database DwgDatabase { get; }

        //give a string of symboltable name, add "Id" as suffix, use reflection to get the related symboltableid property of database,return as string
        public IntPtr GetSymbolTableIdIntPtr(string symbolTableName)
        {
            return DwgDatabase.GetPropertyValue<ObjectId>(symbolTableName + "Id").OldIdPtr;
        }
        public void RunInTransaction(Action<Transaction> action)
        {
            Exception exception = null;
            using (var tr = DwgDatabase.TransactionManager.StartTransaction())
            {
                try
                {
                    action(tr);
                    tr.Commit();
                }
                catch (Exception e)
                {
                    exception= e;
                    tr.Abort();
                }
                if (exception != null) throw exception;
            }
        }

        public Dictionary<string, IntPtr> GetSymbolTableRecordNames(IntPtr symbolTableId)
        {
            ISymbolTableWrapper symbolTableWrapper = new SymbolTableWrapper(this, new ObjectId(symbolTableId));
            symbolTableWrapper.ReadSymbolTableRecordNames();
            return symbolTableWrapper.SymbolTableRecordNames;
        }
    }
}