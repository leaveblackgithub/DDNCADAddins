using System;
using Autodesk.AutoCAD.DatabaseServices;
using CommonUtils.Misc;

namespace ACADBase
{
    public interface ITransactionHelper : IDisposable
    {
        OperationResult<T> GetObject<T>(ObjectId objectId, OpenMode mode)
            where T : DBObject;

        HandleValue CreateObject<T>(ObjectId modelSpaceId)
            where T : Entity, new();

        OperationResult<VoidValue> RunFuncsOnObject<T>(ObjectId objectId, Func<T, OperationResult<VoidValue>>[] funcs)
            where T : DBObject;

        void Commit();
        void AddNewlyCreatedDBObject(DBObject obj, bool add);
    }
}