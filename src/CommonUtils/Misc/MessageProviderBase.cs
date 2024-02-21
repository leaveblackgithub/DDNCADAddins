using System;
using System.Collections.Generic;
using System.Runtime.ExceptionServices;
using System.Windows.Forms;
using NLog;

namespace CommonUtils.Misc
{
    public abstract class MessageProviderBase:IMessageProvider
    {

        public abstract void Show(string message);

        //NO NEED TO LOG, LOG IN COMMANDRESULT
        public void Error(Exception exception)
        {
            var exString = exception.ToString();
            Show(exString);
        }

        public void Error(ExceptionDispatchInfo exceptionInfo)
        {
            Exception exception = exceptionInfo.SourceException;
            Error(exception);
        }
    }
}