using System;
using System.IO;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.ApplicationServices.Core;
using Autodesk.AutoCAD.DatabaseServices;
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

        protected override DatabaseHelper GetDwgDatabaseHelper()
        {
            Database dwgDatabase;
            dwgDatabase = new Database(DefaultDrawing, false);

            if (IsNewDrawingOrExisting())
                dwgDatabase = DwgDocument.Database;
            else
                dwgDatabase.ReadDwgFile(DrawingFile, FileOpenMode.OpenForReadAndWriteNoShare, true, null);
            return new DatabaseHelper(dwgDatabase);
        }

    }

}