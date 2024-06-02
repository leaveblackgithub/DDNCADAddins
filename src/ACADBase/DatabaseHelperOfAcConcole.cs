#if AcConsoleTest
using Autodesk.AutoCAD.DatabaseServices;
using CommonUtils.CustomExceptions;
using CommonUtils.Misc;

namespace ACADBase
{
    public class DatabaseHelperOfAcConcole : DatabaseHelper
    {
        public DatabaseHelperOfAcConcole(string drawingFile, IMessageProvider messageProvider = null)
        {
            CadDatabase = GetDatabase(drawingFile);
            ActiveMsgProvider = messageProvider;
        }

        protected  Database GetDatabase(string drawingFile)
        {
            Database database = null;

            database =HostApplicationServiceWrapper.IsTargetDrawingActive(drawingFile) ? HostApplicationServiceWrapper.GetWorkingDatabase() : DatabaseExtension.GetDwgDatabase(drawingFile);
            if (database == null) throw NullReferenceExceptionOfDatabase._(drawingFile);
            return database;
        }

        public override IMessageProvider ActiveMsgProvider
        {
            get => FldMsgProvider;
            set => FldMsgProvider = value ?? new MessageProviderOfMessageBox();
        }
    }
}
#endif