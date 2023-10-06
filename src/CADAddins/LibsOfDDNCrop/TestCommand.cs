using System;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using CADAddins.Archive;
using CADAddins.LibsOfDDNCrop;

[assembly: CommandClass(typeof(TestCommand))]

namespace CADAddins.LibsOfDDNCrop
{
    public class TestCommand : O_CommandBase
    {
        [CommandMethod("TestCommand")]
        public override void RunCommand()
        {
            using (var trans = O_CurDocHelper.StartTransaction())
            {
                ObjectId boundaryId = O_CurEditorHelper.GetEntity("\nSelect a closed Curve as crop boundary: ",
                    "Invalid selection: requires a closed Curve", typeof(Curve));
                ObjectId objectId = O_CurEditorHelper.GetEntity("\nSelect the entity ");
                var boundary = trans.GetObject(boundaryId, OpenMode.ForRead) as Curve;
                var ent = trans.GetObject(objectId, OpenMode.ForRead) as Entity;
                var pt3dCollection = new Point3dCollection();
                boundary.BoundingBoxIntersectWith(ent, Intersect.OnBothOperands, boundary.GetPlane(), pt3dCollection,
                    IntPtr.Zero, IntPtr.Zero);
                O_CurEditorHelper.WriteMessage(pt3dCollection.Count.ToString());
                trans.Commit();
                O_CadHelper.Quit();
            }
        }

        [CommandMethod("TestCommand2")]
        public void RunCommand2()
        {
            using (var trans = O_CurDocHelper.StartTransaction())
            {
                ObjectId objectId = O_CurEditorHelper.GetEntity("\nSelect the hatch ");
                var sset = O_CurEditorHelper.GetSelection("\nSelect segments for loops");
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