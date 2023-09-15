//using System.Collections.Generic;
//using System.IO;
//using System.Windows.Forms;
//using Autodesk.AutoCAD.Runtime;
//using CADCleanup.Environments;
//using CADCleanup.Reference;
//using Application = Autodesk.AutoCAD.ApplicationServices.Application;
//
//[assembly: CommandClass(typeof(GetCommandLineText))]
//
//namespace CADCleanup.Reference
//{
//    public class GetCommandLineText
//    {
//
//        [CommandMethod("CEDDTACTDP1")]
//        public void CEDDTACTDP1()
//        {
//            CadHelper.AcCurEd.Command(new object[] { "Time", "", "" });
////            ed.Command(new object[] { "CEDDTACTDP2 ", "" });
//        }
//
//
//        [CommandMethod("CEDDTACTDP2")]
//        public void CEDDTACTDP2()
//        {
//            string acCommandLineList = GetCommandLineValue();
//            MessageBox.Show(acCommandLineList);
//        }
//
//        private string GetCommandLineValue()
//        {
//            List<string> acCommandLineList = Autodesk.AutoCAD.Internal.Utils.GetLastCommandLines(11, true);
//            acCommandLineList.Reverse();
//            acCommandLineList.RemoveAt(7);
//            acCommandLineList.RemoveAt(6);
//            acCommandLineList.RemoveAt(6);
//            string commandLineValue = Path.GetFullPath(CadHelper.AcCurDoc.Name)+"\n";
//            commandLineValue += string.Join("\n", acCommandLineList);
//            return commandLineValue;
//        }
//    }
//}

namespace ACADReferenceCodes
{
}