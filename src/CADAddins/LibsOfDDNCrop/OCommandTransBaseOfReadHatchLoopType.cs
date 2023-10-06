using System.Collections.Generic;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Internal;
using Autodesk.AutoCAD.Runtime;
using CADAddins.Archive;
using CADAddins.Environments;
using CADAddins.LibsOfDDNCrop;

[assembly: CommandClass(typeof(ReadHatchLoopType))]

namespace CADAddins.LibsOfDDNCrop
{
    public class OCommandTransBaseOfReadHatchLoopType : O_CommandTransBase
    {
        public OCommandTransBaseOfReadHatchLoopType(Transaction acTrans) : base(acTrans)
        {
        }

        public override bool Run()
        {
            var result = new List<ObjectId>();
            foreach (var id in SelectHatches().GetObjectIds())
            {
                var plot = true;
                var hatch = GetObjectForRead(id) as Hatch;
                var prompt = hatch.GetHatchLoopTypes();

                if (prompt.Contains("SelfIntersecting"))
                {
                    result.Add(id);
                    CurOEditorHelper2.WriteMessage(prompt);
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

            return CurOEditorHelper2.SelectAll(filter);
        }
    }
}