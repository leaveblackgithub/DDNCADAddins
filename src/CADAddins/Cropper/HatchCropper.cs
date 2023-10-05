using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using CADAddins.Environments;

namespace CADAddins.Cropper
{
    public class HatchCropper : EntityCropper<Hatch>
    {
        public HatchCropper(Hatch entity, Curve boundary, WhichSideToKeep whichSideToKeep,
            CommandTransBase commandTransBase) : base(entity, boundary, whichSideToKeep, commandTransBase)
        {
        }

        internal override Point3d GetPosition()
        {
            var hatchLoop = _entity.GetLoopAt(0);
            if (hatchLoop.IsPolyline) return hatchLoop.Polyline[0].Vertex.ToPoint3d();
            return hatchLoop.Curves[0].StartPoint.ToPoint3d();
        }
    }
}