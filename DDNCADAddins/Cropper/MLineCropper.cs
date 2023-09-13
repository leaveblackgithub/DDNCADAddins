using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using DDNCADAddins.Environments;

namespace DDNCADAddins.Cropper
{
    public class MLineCropper : EntityCropper<Mline>
    {
        public MLineCropper(Mline entity, Curve boundary, WhichSideToKeep whichSideToKeep,
            CommandTransBase commandTransBase) : base(entity, boundary, whichSideToKeep, commandTransBase)
        {
        }

        internal override Point3d GetPosition()
        {
            return _entity.VertexAt(0);
        }
    }
}