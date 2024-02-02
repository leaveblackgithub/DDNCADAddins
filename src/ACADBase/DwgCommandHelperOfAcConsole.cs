using Autodesk.AutoCAD.DatabaseServices;
using CommonUtils.CustomExceptions;
using CommonUtils.Misc;
using NLog;

namespace ACADBase
{
    /// <summary>
    ///     Base class for CadCommand.
    /// </summary>
    public class DwgCommandHelperOfAcConsole : DwgCommandHelperBase
    {
        private readonly ILogger _logger = LogManager.GetCurrentClassLogger();

        public DwgCommandHelperOfAcConsole(string drawingFile = "", IMessageProvider messageProvider = null) :
            base(drawingFile, messageProvider)
        {
        }

        protected override Database GetActiveDatabaseBeforeCommand()
        {
            return IsNewDrawingOrExisting() ? null : HostApplicationServices.WorkingDatabase;
        }

        protected override IDatabaseHelper GetDwgDatabaseHelper()
        {
            Database dwgDatabase;
            dwgDatabase = new Database(DefaultDrawing, false);

            if (IsNewDrawingOrExisting())
                dwgDatabase = DwgDocument.Database;
            else
                dwgDatabase.ReadDwgFile(DrawingFile, FileOpenMode.OpenForReadAndWriteNoShare, true, null);
            if (dwgDatabase == null) throw NullReferenceExceptionOfDatabase._(DrawingFile);
            return new DatabaseHelper(dwgDatabase);
        }
    }
}