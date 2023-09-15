using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using PreviousDevelopmentToRefactor.Environments;

namespace PreviousDevelopmentToRefactor.Cropper
{
    public class MLeaderCropper : EntityCropper<MLeader>
    {
        public MLeaderCropper(MLeader entity, Curve boundary, WhichSideToKeep whichSideToKeep,
            CommandTransBase commandTransBase) : base(entity, boundary, whichSideToKeep, commandTransBase)
        {
        }

        internal override Point3d GetPosition()
        {
            return _entity.TextLocation;
        }
    }
}