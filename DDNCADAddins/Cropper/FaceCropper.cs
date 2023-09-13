using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using DDNCADAddins.Environments;

namespace DDNCADAddins.Cropper
{
    public class FaceCropper : EntityCropper<Face>
    {
        public FaceCropper(Face entity, Curve boundary, WhichSideToKeep whichSideToKeep,
            CommandTransBase commandTransBase) : base(entity, boundary, whichSideToKeep, commandTransBase)
        {
        }

        internal override Point3d GetPosition()
        {
            return _entity.GetVertexAt(1);
        }
    }
}