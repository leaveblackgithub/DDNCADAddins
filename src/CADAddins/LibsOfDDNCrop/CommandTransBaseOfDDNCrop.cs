using System.Collections.Generic;
using Autodesk.AutoCAD.DatabaseServices;
using CADAddins.Cropper;
using CADAddins.Environments;

namespace CADAddins.LibsOfDDNCrop
{
    public class CommandTransBaseOfDDNCrop : CommandTransBase
    {
        public CommandTransBaseOfDDNCrop(Transaction acTrans) : base(acTrans)
        {
        }

        public override bool Run()
        {
            var sset = CurEditorHelper.GetSelection("\nSelect entities to be cropped:");
            if (sset == null) return false;
            Curve boundary;
            while (true)
            {
                var boundaryId = CurEditorHelper.GetEntity("\nSelect a closed Curve as crop boundary: ",
                    "Invalid selection: requires a closed Curve", typeof(Curve));
                if (boundaryId == ObjectId.Null) return false;

                boundary = AcTrans.GetObjectForRead(boundaryId) as Curve;
                ;
                if (boundary.Closed) break;
                CurEditorHelper.WriteMessage("\nPolyline should be closed.");
            }

            IEnumerable<ObjectId> objectIds = sset.GetObjectIds();
            var result = CropEntitiesWithBoundary(objectIds, boundary, WhichSideToKeep.Inside);
//            CurEditorHelper.WriteMessage(result.ToString());
            return true;
        }
    }
}