using System;
using System.Collections.Generic;
using Autodesk.AutoCAD.DatabaseServices;
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
        public IntPtr GetSymbolTableId(string symbolTableName)
        {
            var symbolTableIdValue = DwgDatabase.GetType().GetProperty(symbolTableName + "Id")?.GetValue(DwgDatabase, null);
            if (symbolTableIdValue != null)
            {
                ObjectId symbolTableId = (ObjectId) symbolTableIdValue;
                return symbolTableId.OldIdPtr;
            }
            return IntPtr.Zero;
        }

        //give a IntPtr of symboltableid, covert to objectid,
        //get names and objectid IntPtr of all symboltablerecord of this symboltable via getenumrator, return as dictionary<string,string>
        //use transaction to deal with object
        public Dictionary<string, IntPtr> GetSymbolTableRecordNames(IntPtr symbolTableId)
        {
            var symbolTableRecordNames = new Dictionary<string, IntPtr>();
            var symbolTableRecordId = new ObjectId(symbolTableId);
            using (var tr = DwgDatabase.TransactionManager.StartTransaction())
            {
                try
                {
                    var symbolTableRecord = tr.GetObject(symbolTableRecordId, OpenMode.ForRead) as SymbolTable;
                    if (symbolTableRecord != null)
                    {
                        //getenumberator is a method of symboltable, return a symboltableenumerator
                        var symbolTableRecordEnumerator = symbolTableRecord.GetEnumerator();
                        //use enumerator to get all symboltablerecord names and objectids as string
                        while (symbolTableRecordEnumerator.MoveNext())
                        {
                            var recordId = symbolTableRecordEnumerator.Current;
                            //use transaction to get record name
                            using (var record = tr.GetObject(recordId, OpenMode.ForRead) as SymbolTableRecord)
                            {
                                if (record != null) symbolTableRecordNames.Add(record.Name, recordId.OldIdPtr);
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    
                }
                finally { tr.Commit(); }
            }
            return symbolTableRecordNames;
        }
    }
}