using System;
using Autodesk.AutoCAD.DatabaseServices;
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

        public FuncResult ToHandle(out Handle handle)
        {
            var result=new FuncResult();
            handle = default(Handle);
            try
            {
                handle= new Handle(HandleAsLong);
                return result;
            }
            catch (FormatException)
            {
                return result.Cancel(ExceptionMessage.InvalidHandle(HandleAsLong));
            }
        }

        public static FuncResult ToHandle(long handleAsLong,out Handle handle)
        {
            return new HandleValue(handleAsLong).ToHandle(out handle);
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