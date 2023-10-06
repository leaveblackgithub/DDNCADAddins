using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using CADAddins.Archive;
using CADAddins.Environments;

namespace CADAddins.Cropper
{
    public class FaceCropper : EntityCropper<Face>
    {
        public FaceCropper(Face entity, Curve boundary, WhichSideToKeep whichSideToKeep,
            O_CommandTransBase oCommandTransBase) : base(entity, boundary, whichSideToKeep, oCommandTransBase)
        {
        }

        internal override Point3d GetPosition()
        {
            return _entity.GetVertexAt(1);
        }
    }
}