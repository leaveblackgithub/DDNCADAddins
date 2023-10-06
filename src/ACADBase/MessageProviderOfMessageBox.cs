using System;
using System.Windows.Forms;

namespace ACADBase
{
    internal class MessageProviderOfMessageBox : IMessageProvider
    {
        public void Show(string message)
        {
            MessageBox.Show(message);
        }

        public void Error(Exception exception)
        {
            Show(exception.ToString());
        }
    }
}