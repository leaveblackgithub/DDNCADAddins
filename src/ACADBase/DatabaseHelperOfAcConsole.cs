#if AcConsoleTest
using Autodesk.AutoCAD.DatabaseServices;
using CommonUtils.Misc;

namespace ACADBase
{
    public class DatabaseHelperOfAcConsole : DatabaseHelper
    {

        public DatabaseHelperOfAcConsole(string drawingFile , IMessageProvider messageProvider ) : base(drawingFile,
            messageProvider)
        {

        }

        public DatabaseHelperOfAcConsole():base()
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