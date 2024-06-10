using System;
using Autodesk.AutoCAD.DatabaseServices;
using CommonUtils.Misc;

namespace ACADBase
{
    public interface IDatabaseHelper:IDisposable
    {
        OperationResult<VoidValue> RunFuncInTransaction<T>(HandleValue handleValue,
            params Func<T, OperationResult<VoidValue>>[] funcs)
            where T : DBObject;

        OperationResult<HandleValue> CreateInCurrentSpace<T>()
            where T : Entity, new();

        OperationResult<ObjectId> TryGetObjectId(HandleValue handleValue);
    }
}