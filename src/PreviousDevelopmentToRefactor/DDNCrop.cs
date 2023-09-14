using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Runtime;
using PreviousDevelopmentToRefactor;
using PreviousDevelopmentToRefactor.Environments;
using PreviousDevelopmentToRefactor.LibsOfDDNCrop;

[assembly: CommandClass(typeof(DDNCrop))]

namespace PreviousDevelopmentToRefactor
{
    public class DDNCrop : CommandBase
    {
        [CommandMethod("DDNCrop")]
        public override void Run()
        {
            base.Run();
        }

        internal override CommandTransBase InitCommandTransBase(Transaction acTrans)
        {
            return new CommandTransBaseOfDDNCrop(acTrans);
        }
    }
}
//            CurEditorHelper.WriteMessage($"\nWorking at Drawing [{CurDocHelper.Name}]...");
//            var sset = CurEditorHelper.GetSelection("\nSelect entities to be cropped:");
//            if (sset == null)
//            {
//                EndCommands(false);
//                return;
//            }
//
//            Curve boundary;
//            while (true)
//            {
//                ObjectId boundaryId = CurEditorHelper.GetEntity("\nSelect a closed Curve as crop boundary: ",
//                    "Invalid selection: requires a closed Curve", typeof(Curve));
//                if (boundaryId == ObjectId.Null)
//                {
//                    EndCommands(false);
//                    return;
//                }
//
////                CurEditorHelper.WriteMessage($"\n{boundaryId.GetObject(OpenMode.ForRead).GetType().Name}");
//                boundary = CurTransHelper.GetObjectForRead(boundaryId) as Curve;
//                ;
//                if (boundary.Closed) break;
//                CurEditorHelper.WriteMessage("\nPolyline should be closed.");
//            }
//
//            foreach (var id in sset.GetObjectIds())
//            {
//                var dbObj = CurTransHelper.GetObjectForRead(id);
//                if (dbObj is Entity)
//                {
//                    CurEditorHelper.WriteMessage($"\n{dbObj.GetType().Name}:{dbObj.Id.ToString()}");
//                    ExTrimObjWithBoundary(dbObj, boundary, WhichSideToKeep.Inside);
//
//                }
//            }
//
//            EndCommands(true);
//        }
//
//        private void ExTrimObjWithBoundary(DBObject dbObj, Curve boundary, WhichSideToKeep whichSideToKeep)
//        {
//            var ent = dbObj as Entity;
//            Extents3d boundingbox = ent.GeometricExtents;
//            BboxContainment bBoxContainment = boundary.GetBBoxContainment(ent);
//            if (bBoxContainment.ToString() == whichSideToKeep.ToString()) return;
//            if (bBoxContainment != BboxContainment.MayIntersect)
//            {
//                CurTransHelper.GetObjectForWrite(ent.ObjectId);
//                ent.Erase();
//                return;
//            }
//            if (ent is Curve)
//            {
//                Curve curve = dbObj as Curve;
//                TrimCurveWithBoundary(curve, boundary, whichSideToKeep);
//            }
//        }
//
//        private void TrimCurveWithBoundary(Curve curve, Curve boundary, WhichSideToKeep whichSideToKeep)
//        {
//            var pt3dCollection = new Point3dCollection();
//            curve.IntersectWith(boundary, Intersect.OnBothOperands, boundary.GetPlane(), pt3dCollection,
//                IntPtr.Zero, IntPtr.Zero);
//
//            CurEditorHelper.WriteMessage($"\n{curve.GetType().Name}:{curve.Id.ToString()}:{pt3dCollection.Count}");
//            if (pt3dCollection.Count==0) return;
//            CurTransHelper.GetObjectForWrite(curve.ObjectId);
//            DBObjectCollection curves = curve.GetSortedSplitCurves(pt3dCollection);
//
//            CurEditorHelper.WriteMessage($"\n{curve.GetType().Name}:{curve.Id.ToString()}:{curves.Count}");
//            bool updateFirstSegement = true;
//            foreach (DBObject dbObj in curves)
//            {
//                Curve curveSegment=dbObj as Curve;
////                Point3d ptMid = curveSegment.GetCurveMidPoint();
////                PointContainment containment = boundary.GetPointContainment(ptMid);
////                bool KeepCurve;
////                if (containment == PointContainment.OnBoundary)
////                {
////                    KeepCurve = true;
////                }
////                else
////                {
////                    KeepCurve = trimOutsideOrInside & (containment == PointContainment.Outside);
////                }
////                if(!KeepCurve)continue;
////                if (updateFirstSegement&curve.GetType()==curveSegment.GetType()){
////                    curve.SetFromGeCurve(curveSegment.GetGeCurve());
////                    updateFirstSegement = false;
////                }
////                else
////                {
////                    AddEntityToCurrentSpace(curveSegment);
////                }
//                AddEntityToCurrentSpace(curveSegment);
//                dbObj.Dispose();
////                Point3d ptMid=curveSegment.GetPointAtParameter()
//            }
//
//            if (updateFirstSegement)
//            {
//                curve.Erase();
//            }
//
//        }
//    }
//}