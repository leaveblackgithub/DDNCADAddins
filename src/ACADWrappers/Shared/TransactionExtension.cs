using System;
using System.Collections.Generic;
using Autodesk.AutoCAD.DatabaseServices;
using Domain.Shared;
using static CommonUtils.SharedDelegate;

namespace ACADWrappers.Shared
{
    public static class TransactionExtension
    {

        public static bool ReadDbObject<T>(this Transaction transaction,ObjectId objectId, ActionWithResult<T> action) where T : DBObject
        {
            try
            {
                using (var dbObject = transaction.GetObject(objectId, OpenMode.ForRead))
                {
                    if (dbObject != null)
                    {
                        if (!action((T)dbObject)) { return false; }
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