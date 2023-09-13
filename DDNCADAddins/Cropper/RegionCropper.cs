using System.Linq;
using Autodesk.AutoCAD.BoundaryRepresentation;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using DDNCADAddins.Environments;

namespace DDNCADAddins.Cropper
{
    public class RegionCropper : EntityCropper<Region>
    {
        public RegionCropper(Region entity, Curve boundary, WhichSideToKeep whichSideToKeep,
            CommandTransBase commandTransBase) : base(entity, boundary, whichSideToKeep, commandTransBase)
        {
        }

        internal override Point3d GetPosition()
        {
            using (var brep = new Brep(_entity))
            {
                return brep.Vertices.ElementAt(0).Point;
            }
        }
    }
}