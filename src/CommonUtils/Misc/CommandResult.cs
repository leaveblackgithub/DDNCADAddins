using System;
using System.Runtime.ExceptionServices;
using NLog;

namespace CommonUtils.Misc
{
    public class CommandResult
    {
        public enum CommandResultType
        {
            Success,
            Cancel
        }

        public CommandResult(CommandResultType resultType = CommandResultType.Success, Exception exception = null)
        {
            if (exception != null && resultType != CommandResultType.Success)
                throw new ArgumentException("Command can not success with exception");
            ResultType = resultType;
            ExceptionInfo = GetExceptionInfo(exception);
        }

        private CommandResultType ResultType { get; set; }
        public ExceptionDispatchInfo ExceptionInfo { get; set; }

        public bool IsSuccess => ResultType == CommandResultType.Success;
        public bool IsCancel => ResultType == CommandResultType.Cancel;

        private static ExceptionDispatchInfo GetExceptionInfo(Exception exception)
        {
            if (exception == null) return null;
            LogManager.GetCurrentClassLogger().Error(exception);
            return ExceptionDispatchInfo.Capture(exception);
        }

        public CommandResult Success()
        {
            ResultType = CommandResultType.Success;
            ExceptionInfo = null;
            return this;
        }


        public CommandResult Cancel(Exception exception = null)
        {
            ResultType = CommandResultType.Cancel;
            ExceptionInfo = GetExceptionInfo(exception);
            return this;
        }
    }
}