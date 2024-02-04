using System;
using System.Collections.Generic;
using System.Runtime.ExceptionServices;
using System.Windows.Forms;
using NLog;

namespace CommonUtils.Misc
{
    public abstract class MessageProviderBase:IMessageProvider
    {
        protected MessageProviderBase()
        {
            ErrorMessages = new SortedDictionary<string, string>();
        }

        public abstract void Show(string message);
        public SortedDictionary<string,string> ErrorMessages { get; }

        public void Error(Exception exception)
        {
            var exString = exception.ToString();
            ErrorMessages.Add(DateTimeUtils.AddTimeStampPrefix(exception.GetType().Name),exception.StackTrace);
            LogAndShowError(exString);
        }

        private void LogAndShowError(string exString)
        {
            LogManager.GetCurrentClassLogger().Error(exString);
            Show(exString);
        }

        public void Error(ExceptionDispatchInfo exceptionInfo)
        {
            Exception exception = exceptionInfo.SourceException;
            Error(exception);
        }
    }
}