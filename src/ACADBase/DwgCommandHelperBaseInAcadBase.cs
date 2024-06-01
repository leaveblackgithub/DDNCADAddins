using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.ApplicationServices.Core;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.ApplicationServices;
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

        public DwgCommandHelperBaseInAcadBase(string drawingFile = "", IMessageProvider messageProvider = null) : base(drawingFile, messageProvider)
        {
        }

        protected Document AppActiveDocument=> Application.DocumentManager.MdiActiveDocument;

        //databasehelper是和dwgcommmandhelper一一对应的指针，而这些funcs应该是databasehelper的方法。为啥搞这么复杂
        public CommandResult ExecuteDatabaseFuncs(params Func<IDatabaseHelper, CommandResult>[] databaseFuncs)
        {
            var result = new CommandResult();
            if (databaseFuncs.IsNullOrEmpty()) return result;

            acedDisableDefaultARXExceptionHandler(true);
            // Lock the document and execute the test actions.

            var oldDb = HostAppWorkingDatabase; //WorkingDatabase can not be disposed.
            using (AppActiveDocument.LockDocument())//不管三七二十一，lock当前document
            using (var db = GetDatabaseHelper())
            {
                //exception and message has been handled in RunForEach
                result = databaseFuncs.RunForEach(db,ActiveMsgProvider);
                if (!IsTargetDrawingActive()) HostAppWorkingDatabase = oldDb;
            }
            return result;
        }

        public override CommandResult ExecuteCommandInDbHelper()
        {
            var result = new CommandResult();

            acedDisableDefaultARXExceptionHandler(true);
            // Lock the document and execute the test actions.

            var oldDb = HostAppWorkingDatabase; //WorkingDatabase can not be disposed.
            using (AppActiveDocument.LockDocument())//不管三七二十一，lock当前document
            using (var db = GetDatabaseHelper())
            {
                //exception and message has been handled in RunForEach
                result = db.ExecuteCommand();
                if (!IsTargetDrawingActive()) HostAppWorkingDatabase = oldDb;
            }
            return result;

        }

        public Database HostAppWorkingDatabase//WorkingDatabase can not be disposed.
        {

            get => HostApplicationServices.WorkingDatabase;
            set => HostApplicationServices.WorkingDatabase = value;
        }

        //TODO Can't verify if acedDisableDefaultARXExceptionHandler is working
            // EntryPoint may vary across autocad versions
            [DllImport("accore.dll", EntryPoint = "?acedDisableDefaultARXExceptionHandler@@YAX_N@Z")]
        public static extern void acedDisableDefaultARXExceptionHandler(bool disable);
        

        public bool IsTargetDrawingActive()
        {
            bool result = DrawingFile.IsNullOrEmpty() || HostAppWorkingDatabase.Filename == DrawingFile;

#if AcConsole
            return result;
#else
            //TODO 无法解决Application里开图的问题，所以Application Command需要在当前图纸解决；
            if (!result) throw ArgumentExceptionOfInvalidDwgFile._(DrawingFile);
            return result;
#endif
        }
        protected virtual IDatabaseHelper GetDatabaseHelper()
        {
            Database database = null;
            
            database = IsTargetDrawingActive() ? HostAppWorkingDatabase : DatabaseExtension.GetDwgDatabase(DrawingFile);
            if (database == null) throw NullReferenceExceptionOfDatabase._(DrawingFile);
            return new DatabaseHelper(database);
        }
    }
}