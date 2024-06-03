#if ApplicationTest
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.ApplicationServices.Core;
using CommonUtils.Misc;

namespace ACADBase
{
    public class DatabaseHelperOfApplication : DatabaseHelper
    {

        public DatabaseHelperOfApplication(string drawingFile, IMessageProvider messageProvider) : base(drawingFile,
            messageProvider)
        {

        }

        public DatabaseHelperOfApplication():base()
        {
        }

        protected Document CadDocument => Application.DocumentManager.CurrentDocument;
        

        public override IMessageProvider ActiveMsgProvider
        {
            get => FldMsgProvider;
            set => FldMsgProvider = value ?? new MessageProviderOfEditor(CadDocument.Editor);
        }
    }
}
#endif