using System;
using Autodesk.AutoCAD.DatabaseServices;
using CommonUtils.CustomExceptions;
using CommonUtils.Misc;

namespace ACADBase
{
    public class HandleValue
    {
        public HandleValue(long handleAsLong)
        {
            HandleAsLong = handleAsLong;
        }

        public long HandleAsLong { get; set; }

        public OperationResult<Handle> ToHandle()
        {
            try
            {
                return OperationResult<Handle>.Success(new Handle(HandleAsLong));
            }
            catch (FormatException)
            {
                return OperationResult<Handle>.Failure(ExceptionMessage.InvalidHandle(HandleAsLong));
            }
        }

        public static HandleValue FromHandle(Handle handle)
        {
            return new HandleValue(handle.Value);
        }

        public static HandleValue FromObjectId(ObjectId objectId)
        {
            return FromHandle(objectId.Handle);
        }

        public static HandleValue FromObject(DBObject dbObject)
        {
            return FromObjectId(dbObject.Id);
        }
    }
}