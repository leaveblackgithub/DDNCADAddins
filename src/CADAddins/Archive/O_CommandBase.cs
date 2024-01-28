﻿using Autodesk.AutoCAD.DatabaseServices;
using CommonUtils;
using CommonUtils.Misc;

namespace CADAddins.Archive
{
    public abstract class O_CommandBase : DisposableClass
    {
        public O_DocHelper O_CurDocHelper => O_CadHelper.CurDocHelper;
        public Database AcCurDb => O_CurDocHelper.AcCurDb;
        public O_EditorHelper O_CurEditorHelper => O_CurDocHelper.CurEditorHelper;

        public abstract void RunCommand();

        protected override void DisposeUnManaged()
        {
            O_CadHelper.Quit();
        }

        protected override void DisposeManaged()
        {
        }

        protected void EndCommands(bool EndOrCancel)
        {
            if (!EndOrCancel)
                O_CurEditorHelper.WriteMessage($"\nCommand [{GetType().Name}] Cancelled.");
            else
                O_CurEditorHelper.WriteMessage($"\nCommand [{GetType().Name}] Succeeded.");
            Dispose();
        }
    }
}