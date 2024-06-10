using System;
using System.Runtime.ExceptionServices;

namespace CommonUtils.Misc
{
    public abstract class MessageProviderBase : IMessageProvider
    {
        public abstract void Show(string message);

        //NO NEED TO LOG, LOG IN COMMANDRESULT
        public virtual void Error(Exception exception)
        {
            var exString = exception.ToString();
            Show(exString);
        }
    }
}