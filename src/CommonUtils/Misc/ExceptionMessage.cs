using CommonUtils.CustomExceptions;
using NUnit.Framework.Internal;
using System;
using System.Linq;
using System.Reflection;

namespace CommonUtils.Misc
{
    public static class ExceptionMessage
    {

        public static string IdReferToWrongType<T> (string idString)
        {
            return $"{idString} is not referring to a DbObject of Type {typeof(T).Name}.";
        }
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

        public static string InvalidProperty<T> (object obj, string propertyName)
        {
            return 
                $"Type {obj.GetType().Name} doesn't contain property {propertyName} of type {typeof(T).Name}";
        }
        public static string NullDatabase(string dwgPath)
        {
            return $"DwgDatabase read from [{dwgPath}] is null.";
        }
        public static string UnExpexctedError(Exception exception)
        {
            return $"Unexpected Error: {exception.ToString()}";
        }


        public static string NoActiveDocument()
        {
            return "Active Document is null.";
        }

        public static string NoActiveEditor()
        {
            return "Active Editor is null.";
        }

        public static string InvalidMethod<T>(object obj,string methodName)
        {
            return $"Method {methodName} of type {obj.GetType()} should return type {typeof(T)}.";
        }

        public static string NullMethodParameters()
        {
            return "Method parameters can not be null.";
        }

        public static string ReadonlyProperty(object o, string propertyname)
        {
            return
                $"Type {o.GetType().Name}'s property {propertyname} is readonly";
        }

        public static string InvalidArguments()
        {
            return "Invalid Arguments.";
        }
    }
}