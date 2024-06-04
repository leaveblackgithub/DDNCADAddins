using System;
using System.Runtime.ExceptionServices;
using System.Windows.Forms;
using NLog;
using NLog.Fluent;

namespace CommonUtils.Misc
{
    public class FuncResult
    {
        public enum CommandResultType
        {
            Success,
            Cancel
        }

        public FuncResult(string funcName="",CommandResultType resultType = CommandResultType.Success, Exception exception = null)
        {
            CancelMessage = "";
            StampString = GetStampString(funcName);
            Init(resultType, exception);
        }

        private string GetStampString(string funcName="")
        {
            return DateTimeUtils.AddTimeStampPrefix(funcName);
        }

        public string StampString { get; set; }

        private void Init(CommandResultType resultType, Exception exception)
        {
            if (exception != null && resultType != CommandResultType.Success)
                throw new ArgumentException("Command can not success with exception");
            ResultType = resultType;
            ExceptionInfo = GetExceptionInfo(exception);
        }

        private CommandResultType ResultType { get; set; }
        public ExceptionDispatchInfo ExceptionInfo { get; set; }
        public string CancelMessage { get; set; }

        public bool IsSuccess => ResultType == CommandResultType.Success;
        public bool IsCancel => ResultType == CommandResultType.Cancel;

        private static ExceptionDispatchInfo GetExceptionInfo(Exception exception)
        {
            if (exception == null) return null;
            LogManager.GetCurrentClassLogger().Error(exception);
            return ExceptionDispatchInfo.Capture(exception);
        }

        public FuncResult Success()
        {
            ResultType = CommandResultType.Success;
            ExceptionInfo = null;
            return this;
        }


        public FuncResult Cancel(Exception exception = null)
        {
            ResultType = CommandResultType.Cancel;
            ExceptionInfo = GetExceptionInfo(exception);
            CancelMessage = exception?.Message;
            return this;
        }
        public FuncResult Cancel(string message)
        {
            ResultType = CommandResultType.Cancel;
            CancelMessage=message;
            LogManager.GetCurrentClassLogger().Warn(message);
            return this;
        }
    }
}