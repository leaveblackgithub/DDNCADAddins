using System;
using Autodesk.AutoCAD.DatabaseServices;
using CommonUtils.CustomExceptions;

namespace ACADBase
{
    public class HandleValue
    {
        public HandleValue(long handleAsLong)
        {
            HandleAsLong = handleAsLong;
        }

        public long HandleAsLong { get; set; }

        public Handle ToHandle()
        {
            try
            {
                return new Handle(HandleAsLong);
            }
            catch (FormatException)
            {
                throw ArgumentExceptionOfInvalidHandle._(HandleAsLong);
            }
        }

        public static Handle ToHandle(long handleAsLong)
        {
            return new HandleValue(handleAsLong).ToHandle();
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