using ACADCommands;
using Autodesk.AutoCAD.Runtime;

[assembly: CommandClass(typeof(Cleanup))]

namespace ACADCommands
{
    public class Cleanup
    {
        [CommandMethod("Cleanup")]
        public static void Run()
        {
        }
    }
}