#if AcConsoleTest
using Autodesk.AutoCAD.DatabaseServices;
using CommonUtils.CustomExceptions;
using CommonUtils.Misc;

namespace ACADBase
{
    public class DatabaseHelperOfAcConsole : DatabaseHelper
    {

        public DatabaseHelperOfAcConsole(string drawingFile  ) : base(drawingFile)
        {

        }

        public DatabaseHelperOfAcConsole():base()
        {
        }
    }
}
#endif