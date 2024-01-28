using Autodesk.AutoCAD.DatabaseServices;
using CommonUtils;
using CommonUtils.CustomExceptions;
using NLog;

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
            catch (System.FormatException)
            {
                throw  ArgumentExceptionOfInvalidHandle._(HandleAsLong);
            }
        }

        public static Handle ToHandle(long handleAsLong) => new HandleValue(handleAsLong).ToHandle();
        public static HandleValue FromHandle(Handle handle) => new HandleValue(handle.Value);

        public static HandleValue FromObjectId(ObjectId objectId) => FromHandle(objectId.Handle);
        public static HandleValue FromObject(DBObject dbObject) => FromObjectId(dbObject.Id);
    }
}