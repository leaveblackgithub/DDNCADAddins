using System;
using Autodesk.AutoCAD.DatabaseServices;
using CommonUtils.Misc;

namespace ACADBase
{
    public interface IDatabaseHelper:IDisposable
    {
        CommandResult RunFuncInTransaction<T>(HandleValue handleValue,
            params Func<T, CommandResult>[] funcs) where T : DBObject;

        CommandResult CreateInModelSpace<T>(out HandleValue handleValue,
            params Func<T, CommandResult>[] funcs)
            where T : Entity, new();

        bool TryGetObjectId(HandleValue handleValue, out ObjectId objectId);
        CommandResult ExecuteCommand();
    }
}