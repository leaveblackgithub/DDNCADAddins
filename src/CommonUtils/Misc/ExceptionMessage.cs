using CommonUtils.CustomExceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonUtils.Misc
{
    public static class ExceptionMessage
    {
        public static string IsNotExistingDwg(this string dwgPath)
        {
            return $"[{dwgPath}] is not an existing dwg file";
        }

        public static string NullActiveDocument(string dwgPath)
        {
            return $"Commands need to be executed in Active Document. Drawing file [{dwgPath}] is not active document.";
        }

        public static string NullConstructor<T>(Type[] parameterTypes)
        {
            return
                $"Class [{typeof(T)}] doesn't have valid constructor with parameters of types [{string.Join(",",parameterTypes.Select(t => t.Name).ToArray())}]";

        }
        public static string NullConstructorParameter()
        {
            return
                $"GetConstructor can not accept null as parameter";

        }
        public static string InvalidId (string idString)
        {
            return $"{idString} is not a valid ObjectId.";
        }
        public static string InvalidHandle (long handleValueAsLong)
        {
            return $"{handleValueAsLong} is not a valid Handle Value.";
        }
    }

}
