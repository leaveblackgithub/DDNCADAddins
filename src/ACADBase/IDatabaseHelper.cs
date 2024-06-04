using System;
using Autodesk.AutoCAD.DatabaseServices;
using CommonUtils.Misc;

namespace ACADBase
{
    public interface IDatabaseHelper:IDisposable
    {
        FuncResult RunFuncInTransaction<T>(HandleValue handleValue,
            params Func<T, FuncResult>[] funcs) where T : DBObject;

        FuncResult CreateInCurrentSpace<T>(out HandleValue handleValue)
            where T : Entity, new();

        FuncResult TryGetObjectId(HandleValue handleValue, out ObjectId objectId);
    }
}