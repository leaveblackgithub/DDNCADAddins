using System;
using CommonUtils.Misc;

namespace ACADBaseOfApplication
{
    public class MessageProviderOfEditor : IMessageProvider
    {
        private static readonly Lazy<MessageProviderOfEditor> Instance =
            new Lazy<MessageProviderOfEditor>(() => new MessageProviderOfEditor());

        private MessageProviderOfEditor()
        {
        }

        public static MessageProviderOfEditor _ => Instance.Value;

        public void WriteMessage(string message)
        {
            // Use ActiveEditorWrapper to write the message to the AutoCAD command line
            ActiveEditorWrapper._.WriteMessage(message);
            RecentMessage = message;
        }

        public string RecentMessage { get; private set; }
    }
}