using System;
using System.Collections.Generic;
using Autodesk.AutoCAD.DatabaseServices;
using Domain.Shared;

namespace ACADWrappers.Shared
{
    public static class TransactionExtension
    {

        public static bool Read<T>(this Transaction transaction,ObjectId objectId, Action<T> action) where T : DBObject
        {
            try
            {
                using (var dbObject = transaction.GetObject(objectId, OpenMode.ForRead))
                {
                    if (dbObject != null)
                    {
                        action((T)dbObject);
                    }
                }
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
        }
    }
}