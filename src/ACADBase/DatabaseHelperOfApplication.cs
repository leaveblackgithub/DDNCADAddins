#if ApplicationTest
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.ApplicationServices.Core;
using CommonUtils.Misc;

namespace ACADBase
{
    public class DatabaseHelperOfApplication : DatabaseHelper
    {

        public DatabaseHelperOfApplication(string drawingFile = "", IMessageProvider messageProvider = null) : base(drawingFile,
            messageProvider)
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