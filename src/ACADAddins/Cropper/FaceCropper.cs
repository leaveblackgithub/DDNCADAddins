using ACADAddins.Archive;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;

namespace ACADAddins.Cropper
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