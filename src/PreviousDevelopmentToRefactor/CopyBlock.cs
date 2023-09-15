using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Runtime;
using PreviousDevelopmentToRefactor;
using PreviousDevelopmentToRefactor.Environments;

[assembly: CommandClass(typeof(CopyBlock))]

namespace PreviousDevelopmentToRefactor
{
    internal class CopyBlock : CommandBase
    {
        internal override CommandTransBase InitCommandTransBase(Transaction acTrans)
        {
            return new CommandTransBaseOfDuplicateBlockAs(acTrans);
        }

        [CommandMethod("CopyXC")]
        public override void Run()
        {
            base.Run();
        }
    }

    public class CommandTransBaseOfDuplicateBlockAs : CommandTransBase
    {
        public CommandTransBaseOfDuplicateBlockAs(Transaction acTrans) : base(acTrans)
        {
        }

        public override bool Run()
        {
            BlockReference bref;
            string newname;
            var btId = CurDocHelper.BlockTableId;
            var brefId = CurEditorHelper.GetEntity("\nSelect main block instance to copy: ",
                "\nMust be a type of the BlockReference!", typeof(BlockReference));
            if (brefId == ObjectId.Null) return false;
            bref = (BlockReference)GetObjectForRead(brefId);
            CurEditorHelper.WriteMessage($"\nBlock Selected with name: {bref.Name}");
            newname = CurEditorHelper.GetString("\nEnter new block name: ");
            if (newname == "") return false;
            if (DuplicateBlockDefAndAssign(bref, newname)) return true;
            return false;
        }
    }
}