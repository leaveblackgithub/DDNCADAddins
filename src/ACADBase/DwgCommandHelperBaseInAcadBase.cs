using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.ApplicationServices.Core;
using Autodesk.AutoCAD.DatabaseServices;
using CommonUtils.Misc;
using System;
using System.IO;
using System.Runtime.InteropServices;
using CommonUtils;
using CommonUtils.DwgLibs;
using CommonUtils.StringLibs;
using CommonUtils.CustomExceptions;

namespace ACADBase
{
    public class DwgCommandHelperBaseInAcadBase : DwgCommandHelperBase,IDwgCommandHelperInAcadBase

    {
        private Document _dwgDocument;

        public DwgCommandHelperBaseInAcadBase(string drawingFile = "", IMessageProvider messageProvider = null) : base(drawingFile, messageProvider)
        {
        }

        protected Document DwgDocument=>_dwgDocument ??= Application.DocumentManager.MdiActiveDocument;

        //databasehelper是和dwgcommmandhelper一一对应的指针，而这些funcs应该是databasehelper的方法。为啥搞这么复杂
        public CommandResult ExecuteDatabaseFuncs(params Func<IDatabaseHelper, CommandResult>[] databaseFuncs)
        {
            var result = new CommandResult();
            if (databaseFuncs.IsNullOrEmpty()) return result;

            acedDisableDefaultARXExceptionHandler(true);
            // Lock the document and execute the test actions.

            var oldDb = ActiveDatabase; //WorkingDatabase can not be disposed.
            using (DwgDocument.LockDocument())//应该用当前document
            using (var db = GetDwgDatabaseHelper())
            {
                //exception and message has been handled in RunForEach
                result = databaseFuncs.RunForEach(db,ActiveMsgProvider);
                if (!IsNewDrawingOrExisting()) ActiveDatabase = oldDb;
            }
            return result;
        }


        //TODO Can't verify if acedDisableDefaultARXExceptionHandler is working
        // EntryPoint may vary across autocad versions
        [DllImport("accore.dll", EntryPoint = "?acedDisableDefaultARXExceptionHandler@@YAX_N@Z")]
        public static extern void acedDisableDefaultARXExceptionHandler(bool disable);

        public Database ActiveDatabase
        {
            get => HostApplicationServices.WorkingDatabase.IsDataBaseSavedAsDwg()
                ? HostApplicationServices.WorkingDatabase
                : null;
            set => HostApplicationServices.WorkingDatabase = value ?? DatabaseExtension.NewDrawingDatabase();
        }

        public bool IsTargetDrawingActive()
        {
            return ActiveDatabase!=null&&ActiveDatabase.Filename == DrawingFile;
        }
        protected virtual IDatabaseHelper GetDwgDatabaseHelper()
        {
            Database database = null;
            if (DrawingFile.IsNullOrEmpty()) database= DatabaseExtension.NewDrawingDatabase();
            else if (IsTargetDrawingActive()) database = ActiveDatabase;
            else database = DatabaseExtension.GetDwgDatabase(DrawingFile);
            if (database == null) throw NullReferenceExceptionOfDatabase._(DrawingFile);
            
            return new DatabaseHelper(database);
        }
    }
}