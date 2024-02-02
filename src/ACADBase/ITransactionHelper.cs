using System;
using Autodesk.AutoCAD.DatabaseServices;
using CommonUtils.Misc;

namespace ACADBase
{
    public interface ITransactionHelper:IDisposable
    {
        T GetObject<T>(ObjectId objectId, OpenMode mode)
            where T : DBObject;

        HandleValue CreateInModelSpace<T>(ObjectId modelSpaceId)
            where T : Entity, new();

        CommandResult RunFuncsOnObject<T>(ObjectId objectId, Func<T, CommandResult>[] funcs) where T : DBObject;
        void Commit();
        void AddNewlyCreatedDBObject(DBObject obj, bool add);
    }
}