using System;
using ACADBase;
using Autodesk.AutoCAD.ApplicationServices.Core;
using Autodesk.AutoCAD.DatabaseServices;
using CADAddins.Archive;

namespace CADAddins.Environments
{
    public class CommandBase : O_CommandBase
    {
        public IDwgCommandHelper ActiveDwgCommandHelper;

        public CommandBase()
        {
            ActiveDwgCommandHelper = new DwgCommandHelper("",
                new MessageProviderOfEditor(Application.DocumentManager.CurrentDocument.Editor));
        }

        public override void RunCommand()
        {
            throw new NotImplementedException();
        }
    }
}