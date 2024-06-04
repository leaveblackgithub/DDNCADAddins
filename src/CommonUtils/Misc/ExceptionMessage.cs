using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonUtils.Misc
{
    public static class ExceptionMessage
    {
        public static string IdReferToWrongType <T>(string idString)
        {
            return $"{idString} is not referring to a DbObject of Type {typeof(T).Name}.";
        }
        public static string InvalidDwgFile(string dwgPath)
        {
            return $"Commands need to be executed in Active Document. Drawing file [{dwgPath}] is not active document.";
        }

        public static string InvalidHandle (long handleValueAsLong)
        {
            return $"{handleValueAsLong} is not a valid Handle Value.";
        }

        public static string InvalidId(string idString)
        {
            return $"{idString} is not a valid ObjectId.";
        }
        public static string InvalidProperty<T>(object obj, string propertyName)
        {
            return $"Type {obj.GetType().Name} doesn't contain property {propertyName} of type {typeof(T).Name}";
        }
        public static string WrongDbObjectType(string dbObjectClassName)
        {
            return $"{dbObjectClassName} is not referring to a valid DbObject Class";
        }
        public static string DwgFileNotFound(string dwgPath)
        {
            return $"Drawing file [{dwgPath}] not found.";
        }
        public static string NullActiveDocument()
        {
            return $"Active Document is null.";
        }
        public static string NullConstructor<T>()
        {
            return $"Class [{typeof(T)}] doesn't have ";
        }
        public static string NullDatabase(string dwgPath)
        {
            return $"DwgDatabase read from [{dwgPath}] is null.";
        }
    }
}
