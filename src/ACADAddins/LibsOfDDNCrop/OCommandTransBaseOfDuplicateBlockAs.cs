using ACADAddins.Archive;
using ACADAddins.LibsOfDDNCrop;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Runtime;

[assembly: CommandClass(typeof(CopyBlock))]

namespace ACADAddins.LibsOfDDNCrop
{
    public class OCommandTransBaseOfDuplicateBlockAs : O_CommandTransBase
    {
        public OCommandTransBaseOfDuplicateBlockAs(Transaction acTrans) : base(acTrans)
        {
        }

        public override bool Run()
        {
            BlockReference bref;
            string newname;
            var btId = CurODocHelper2.BlockTableId;
            var brefId = CurOEditorHelper2.GetEntity("\nSelect main block instance to copy: ",
                "\nMust be a type of the BlockReference!", typeof(BlockReference));
            if (brefId == ObjectId.Null) return false;
            bref = (BlockReference)GetObjectForRead(brefId);
            CurOEditorHelper2.WriteMessage($"\nBlock Selected with name: {bref.Name}");
            newname = CurOEditorHelper2.GetString("\nEnter new block name: ");
            if (newname == "") return false;
            if (DuplicateBlockDefAndAssign(bref, newname)) return true;
            return false;
        }
    }
}