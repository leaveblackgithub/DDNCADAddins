#if ApplicationTest
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.ApplicationServices.Core;
using CommonUtils.Misc;

namespace ACADBase
{
    public class DatabaseHelperOfApplication : DatabaseHelper
    {
        public DatabaseHelperOfApplication(string drawingFile) : base(drawingFile)
        {
        }

        public DatabaseHelperOfApplication()
        {
        }

        protected Document CadDocument => Application.DocumentManager.CurrentDocument;
        
    }
}
#endif