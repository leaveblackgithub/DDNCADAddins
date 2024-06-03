using System;
using Autodesk.AutoCAD.DatabaseServices;
using CommonUtils.CustomExceptions;

namespace ACADBase
{
    public static class DbObjectExtension
    {
        public static Type GetDbObjectDrivedClass(string className)
        {
            Type dbObjectType = Type.GetType("Autodesk.AutoCAD.DatabaseServices." + className);
            if (dbObjectType == null || !typeof(DBObject).IsAssignableFrom(dbObjectType))
            {
                throw ArgumentExceptionOfWrongDbObjectType._(className);
            }
            return dbObjectType;
        }
    }
}