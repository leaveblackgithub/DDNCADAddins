using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Forms;
using ACADAddins.Archive;
using ACADAddins.LibsOfDDNCrop;
using ACADAddins.Reference;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Internal;
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.Windows;
using Application = Autodesk.AutoCAD.ApplicationServices.Core.Application;

[assembly: CommandClass(typeof(GetCommandLineText))]

namespace ACADAddins.Reference
{
    public class GetCommandLineText : CommandBaseOfDDNCrop
    {
        //COULD USE IN ACCORECONSOLE
        [CommandMethod("CEDDTACTDP1")]
        public void CEDDTACTDP1()
        {
            CurOEditorHelper2.Command(new List<string> { "Time", "", "" });
            //            ed.Command(new object[] { "CEDDTACTDP2 ", "" });
        }


        [CommandMethod("CEDDTACTDP2")]
        public void CEDDTACTDP2()
        {
            var acCommandLineList = GetCommandLineValue();
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
            Utils.ShowHideTextWindow(true);
        }

        private string GetCommandLineValue()
        {
            //COULD NOT USE IN ACCORECONSOLE, BELONG TO ACMGD.DLL
            var acCommandLineList = Utils.GetLastCommandLines(11, true);
            acCommandLineList.Reverse();
            acCommandLineList.RemoveAt(7);
            acCommandLineList.RemoveAt(6);
            acCommandLineList.RemoveAt(6);
            var commandLineValue = Path.GetFullPath(CurODocHelper2.Name) + "\n";
            commandLineValue += string.Join("\n", acCommandLineList);
            return commandLineValue;
        }

        internal override O_CommandTransBase InitCommandTransBase(Transaction acTrans)
        {
            throw new NotImplementedException();
        }
    }
}