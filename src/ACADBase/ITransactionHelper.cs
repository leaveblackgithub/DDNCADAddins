using System;
using Autodesk.AutoCAD.DatabaseServices;
using CommonUtils.Misc;

namespace ACADBase
{
    public interface ITransactionHelper:IDisposable
    {
        FuncResult GetObject<T>(ObjectId objectId, OpenMode mode,out T t)
            where T : DBObject;

        FuncResult CreateObjectInModelSpace<T>(ObjectId modelSpaceId,out HandleValue handleValue)
            where T : Entity, new();

        FuncResult RunFuncsOnObject<T>(ObjectId objectId, Func<T, FuncResult>[] funcs) where T : DBObject;
        void Commit();
        void AddNewlyCreatedDBObject(DBObject obj, bool add);
    }
}