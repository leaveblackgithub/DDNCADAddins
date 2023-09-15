using System.Collections.Generic;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Internal;
using Autodesk.AutoCAD.Runtime;
using PreviousDevelopmentToRefactor;
using PreviousDevelopmentToRefactor.Environments;

[assembly: CommandClass(typeof(ReadHatchLoopType))]

namespace PreviousDevelopmentToRefactor
{
    public class ReadHatchLoopType : CommandBase
    {
        [CommandMethod("RLT")]
        public override void Run()
        {
            base.Run();
        }

        internal override CommandTransBase InitCommandTransBase(Transaction acTrans)
        {
            return new CommandTransBaseOfReadHatchLoopType(acTrans);
        }
    }

    public class CommandTransBaseOfReadHatchLoopType : CommandTransBase
    {
        public CommandTransBaseOfReadHatchLoopType(Transaction acTrans) : base(acTrans)
        {
        }

        public override bool Run()
        {
            var result = new List<ObjectId>();
            foreach (var id in SelectHatches().GetObjectIds())
            {
                var hatch = GetObjectForRead(id) as Hatch;
                var prompt = hatch.GetHatchLoopTypes();

                if (prompt.Contains("SelfIntersecting"))
                {
                    result.Add(id);
                    CurEditorHelper.WriteMessage(prompt);
                }
            }

            Utils.SelectObjects(result.ToArray());
            return true;
        }

        private SelectionSet SelectHatches()
        {
            var value = new TypedValue[1];
            value.SetValue(new TypedValue((int)DxfCode.Start, "HATCH"), 0);
            var filter = new SelectionFilter(value);

            return CurEditorHelper.SelectAll(filter);
        }
    }
}