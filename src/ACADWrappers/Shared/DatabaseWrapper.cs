using System;
using System.Collections.Generic;
using Autodesk.AutoCAD.DatabaseServices;
using CommonUtils;
using Domain.Shared;
using static CommonUtils.SharedDelegate;

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
            var symbolTableIdValue = DwgDatabase.GetType().GetProperty(symbolTableName + "Id")?.GetValue(DwgDatabase, null);
            if (symbolTableIdValue != null)
            {
                ObjectId symbolTableId = (ObjectId) symbolTableIdValue;
                return symbolTableId.OldIdPtr;
            }
            return IntPtr.Zero;
        }

        public bool RunInTransaction(ActionWithResult<Transaction> action)
        {
            bool result= false;
            using (var tr = DwgDatabase.TransactionManager.StartTransaction())
            {
                try
                {
                    if (action(tr))
                    {
                        result=true;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
                finally
                {
                    tr.Commit();
                }
                return result;
            }
        }

        public Dictionary<string, IntPtr> GetSymbolTableRecordNames(IntPtr symbolTableId)
        {
            ISymbolTableWrapper symbolTableWrapper = new SymbolTableWrapper(this, new ObjectId(symbolTableId));
            symbolTableWrapper.GetSymbolTableRecordNames();
            return symbolTableWrapper.SymbolTableRecordNames;
        }
    }
}