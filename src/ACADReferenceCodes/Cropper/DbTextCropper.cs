using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using CADAddins.Environments;

namespace CADAddins.Cropper
{
    public class DbTextCropper : EntityCropper<DBText>
    {
        public DbTextCropper(DBText entity, Curve boundary, WhichSideToKeep whichSideToKeep,
            CommandTransBase commandTransBase) : base(entity, boundary, whichSideToKeep, commandTransBase)
        {
        }

        internal override Point3d GetPosition()
        {
            return _entity.Position;
        }
    }
}
