using ACADReferenceCodes.CancelCommands;
using Autodesk.AutoCAD.ApplicationServices.Core;
using Autodesk.AutoCAD.Runtime;

[assembly: CommandClass(typeof(CancelCommand))]

namespace ACADReferenceCodes.CancelCommands
{
    public class CancelCommand
    {
        [CommandMethod("loop")]
        public static void Loop()
        {
            var document = Application.DocumentManager.MdiActiveDocument;
            var ed = document.Editor;
            // Create and add our message filter
            var filter = new MyMessageFilter();
            System.Windows.Forms.Application.AddMessageFilter(filter);
            // Start the loop
            while (true)
            {
                // Check for user input events
                System.Windows.Forms.Application.DoEvents();
                // Check whether the filter has set the flag
                if (filter.bCanceled)
                {
                    ed.WriteMessage("nLoop cancelled.");
                    break;
                }

                ed.WriteMessage("nInside while loop...");
            }

            // We're done - remove the message filter
            System.Windows.Forms.Application.RemoveMessageFilter(filter);
        }
    }
    // Our message filter class
}