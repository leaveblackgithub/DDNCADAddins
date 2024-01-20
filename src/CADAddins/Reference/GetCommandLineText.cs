using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Forms;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.Windows;
using CADAddins.Archive;
using CADAddins.Environments;
using CADAddins.LibsOfDDNCrop;
using CADCleanup.Reference;
using Application = Autodesk.AutoCAD.ApplicationServices.Core.Application;

[assembly: CommandClass(typeof(GetCommandLineText))]

namespace CADCleanup.Reference
{
    public class GetCommandLineText:CommandBaseOfDDNCrop
    {
        //COULD USE IN ACCORECONSOLE
        [CommandMethod("CEDDTACTDP1")]
        public void CEDDTACTDP1()
        {
            CurOEditorHelper2.Command(new List<string>(){ "Time", "", "" });
//            ed.Command(new object[] { "CEDDTACTDP2 ", "" });
        }
        

        [CommandMethod("CEDDTACTDP2")]
        public void CEDDTACTDP2()
        {
            string acCommandLineList = GetCommandLineValue();
            MessageBox.Show(acCommandLineList);
        }

        //TODO 把排列窗口加到长程序里
        [CommandMethod("CEDDTACTDP3")]
        public void CEDDTACTDP3()
        {
            var window = Application.MainWindow;
            var tl = window.DeviceIndependentLocation;
            var size = window.DeviceIndependentSize;
            window.WindowState = Window.State.Normal;
            window.DeviceIndependentLocation = tl;
            window.DeviceIndependentSize = new Size(size.Width / 2, size.Height);
            Autodesk.AutoCAD.Internal.Utils.ShowHideTextWindow(true);
        }

        private string GetCommandLineValue()
        {
            //COULD NOT USE IN ACCORECONSOLE, BELONG TO ACMGD.DLL
            List<string> acCommandLineList = Autodesk.AutoCAD.Internal.Utils.GetLastCommandLines(11, true);
            acCommandLineList.Reverse();
            acCommandLineList.RemoveAt(7);
            acCommandLineList.RemoveAt(6);
            acCommandLineList.RemoveAt(6);
            string commandLineValue = Path.GetFullPath(CurODocHelper2.Name)+"\n";
            commandLineValue += string.Join("\n", acCommandLineList);
            return commandLineValue;
        }

        internal override O_CommandTransBase InitCommandTransBase(Transaction acTrans)
        {
            throw new System.NotImplementedException();
        }
    }
}

