using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using CADAddins.Archive;
using CADAddins.Environments;

namespace CADAddins.Cropper
{
    public class DimCropper : EntityCropper<Dimension>
    {
        public DimCropper(Dimension entity, Curve boundary, WhichSideToKeep whichSideToKeep,
            O_CommandTransBase oCommandTransBase) : base(entity, boundary, whichSideToKeep, oCommandTransBase)
        {
        }

        internal override Point3d GetPosition()
        {
            return _entity.TextPosition;
        }
    }
}