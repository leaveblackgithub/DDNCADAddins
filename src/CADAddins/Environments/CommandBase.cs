using System;
using ACADBase;
using Autodesk.AutoCAD.ApplicationServices.Core;
using CADAddins.Archive;

namespace CADAddins.Environments
{
    public class CommandBase : O_CommandBase
    {
        public IDwgCommandHelperInAcadBase ActiveDwgCommandHelper;

        public CommandBase()
        {
            ActiveDwgCommandHelper = new DwgCommandHelperBaseInAcadBase("",
                new MessageProviderOfEditor(Application.DocumentManager.CurrentDocument.Editor));
        }

        public override void RunCommand()
        {
            throw new NotImplementedException();
        }
    }
}