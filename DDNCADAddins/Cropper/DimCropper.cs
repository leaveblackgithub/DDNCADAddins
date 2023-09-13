using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using DDNCADAddins.Environments;

namespace DDNCADAddins.Cropper
{
    public class DimCropper : EntityCropper<Dimension>
    {
        public DimCropper(Dimension entity, Curve boundary, WhichSideToKeep whichSideToKeep,
            CommandTransBase commandTransBase) : base(entity, boundary, whichSideToKeep, commandTransBase)
        {
        }

        internal override Point3d GetPosition()
        {
            return _entity.TextPosition;
        }
    }
}