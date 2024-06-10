using System;
using System.Linq;

namespace CommonUtils.Misc
{
    public static class ExceptionMessage
    {
        public static string IsNotExistingOrNotDwg(string dwgPath)
        {
            return $"[{dwgPath}] is not an existing file or not a dwg file";
        }

        public static string NullActiveDocument(string dwgPath)
        {
            return $"Commands need to be executed in Active Document. Drawing file [{dwgPath}] is not active document.";
        }

        public static string NullConstructor<T>(Type[] parameterTypes)
        {
            return
                $"Class [{typeof(T)}] doesn't have valid constructor with parameters of types [{string.Join(",", parameterTypes.Select(t => t.Name).ToArray())}]";
        }

        public static string NullConstructorParameter()
        {
            return
                "GetConstructor can not accept null as parameter";
        }

        public static string InvalidId(string idString)
        {
            return $"{idString} is not a valid ObjectId.";
        }

        public static string InvalidHandle(long handleValueAsLong)
        {
            return $"{handleValueAsLong} is not a valid Handle Value.";
        }

        public static string NullDatabase(string dwgPath)
        {
            return $"DwgDatabase read from [{dwgPath}] is null.";
        }
    }
}