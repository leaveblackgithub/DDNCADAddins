using System;
using System.Runtime.ExceptionServices;
using NLog;

namespace CommonUtils.Misc
{
    public class FuncResult
    {
        public enum FuncResultType
        {
            Success,
            Cancel
        }

        public FuncResult(string funcName = "", FuncResultType resultType = FuncResultType.Success,
            Exception exception = null)
        {
            CancelMessage = "";
            StampString = GetStampString(funcName);
            Init(resultType, exception);
        }

        public string StampString { get; set; }

        private FuncResultType ResultType { get; set; }
        public ExceptionDispatchInfo ExceptionInfo { get; set; }
        public string CancelMessage { get; set; }

        public bool IsSuccess => ResultType == FuncResultType.Success;
        public bool IsCancel => ResultType == FuncResultType.Cancel;

        private string GetStampString(string funcName = "")
        {
            return DateTimeUtils.AddLongTimeStampPrefix(funcName);
        }

        private void Init(FuncResultType resultType, Exception exception)
        {
            if (exception != null && resultType != FuncResultType.Success)
                throw new ArgumentException("Command can not success with exception");
            ResultType = resultType;
            ExceptionInfo = GetExceptionInfo(exception);
        }

        private static ExceptionDispatchInfo GetExceptionInfo(Exception exception)
        {
            if (exception == null) return null;
            LogManager.GetCurrentClassLogger().Error(exception);
            return ExceptionDispatchInfo.Capture(exception);
        }

        public FuncResult Success()
        {
            ResultType = FuncResultType.Success;
            ExceptionInfo = null;
            return this;
        }


        public FuncResult Cancel(Exception exception = null)
        {
            ResultType = FuncResultType.Cancel;
            ExceptionInfo = GetExceptionInfo(exception);
            CancelMessage = exception?.Message;
            return this;
        }

        public FuncResult Cancel(string message)
        {
            ResultType = FuncResultType.Cancel;
            CancelMessage = message;
            LogManager.GetCurrentClassLogger().Warn(message);
            return this;
        }
    }
}