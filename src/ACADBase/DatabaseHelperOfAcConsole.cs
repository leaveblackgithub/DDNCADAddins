#if AcConsoleTest
using Autodesk.AutoCAD.DatabaseServices;
using CommonUtils.CustomExceptions;
using CommonUtils.Misc;

namespace ACADBase
{
    public class DatabaseHelperOfAcConsole : DatabaseHelper
    {

        public DatabaseHelperOfAcConsole(string drawingFile = "", IMessageProvider messageProvider = null) : base(drawingFile,
            messageProvider)
        {

        }
        public override IMessageProvider ActiveMsgProvider
        {
            get => FldMsgProvider;
            set => FldMsgProvider = value ?? new MessageProviderOfMessageBox();
        }
    }
}
#endif