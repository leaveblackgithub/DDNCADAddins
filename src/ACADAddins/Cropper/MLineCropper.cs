using ACADAddins.Archive;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;

namespace ACADAddins.Cropper
{
    public class MLineCropper : EntityCropper<Mline>
    {
        public MLineCropper(Mline entity, Curve boundary, WhichSideToKeep whichSideToKeep,
            O_CommandTransBase oCommandTransBase) : base(entity, boundary, whichSideToKeep, oCommandTransBase)
        {
        }

        internal override Point3d GetPosition()
        {
            return _entity.VertexAt(0);
        }
    }
}