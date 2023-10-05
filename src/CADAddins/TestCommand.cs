using System;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using CADAddins;
using CADAddins.Archive;

[assembly: CommandClass(typeof(TestCommand))]

namespace CADAddins
{
    public class TestCommand : O_CommandBase
    {
        [CommandMethod("TestCommand")]
        public override void RunCommand()
        {
            using (var trans = CurDocHelper.StartTransaction())
            {
                ObjectId boundaryId = CurEditorHelper.GetEntity("\nSelect a closed Curve as crop boundary: ",
                    "Invalid selection: requires a closed Curve", typeof(Curve));
                ObjectId objectId = CurEditorHelper.GetEntity("\nSelect the entity ");
                var boundary = trans.GetObject(boundaryId, OpenMode.ForRead) as Curve;
                var ent = trans.GetObject(objectId, OpenMode.ForRead) as Entity;
                var pt3dCollection = new Point3dCollection();
                boundary.BoundingBoxIntersectWith(ent, Intersect.OnBothOperands, boundary.GetPlane(), pt3dCollection,
                    IntPtr.Zero, IntPtr.Zero);
                CurEditorHelper.WriteMessage(pt3dCollection.Count.ToString());
                trans.Commit();
                O_CadHelper.Quit();
            }
        }

        [CommandMethod("TestCommand2")]
        public void RunCommand2()
        {
            using (var trans = CurDocHelper.StartTransaction())
            {
                ObjectId objectId = CurEditorHelper.GetEntity("\nSelect the hatch ");
                var sset = CurEditorHelper.GetSelection("\nSelect segments for loops");
                var hatch = trans.GetObject(objectId, OpenMode.ForWrite) as Hatch;
                hatch.RemoveLoopAt(0);
                hatch.AppendLoop(HatchLoopTypes.Outermost, new ObjectIdCollection(sset.GetObjectIds()));
                hatch.EvaluateHatch(false);

                trans.Commit();
                O_CadHelper.Quit();
            }
        }
    }
}