using System;
using System.Reflection;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;

namespace PreviousDevelopmentToRefactor.Environments
{
    public static class GeoExt
    {
        public static ObjectId BulgeVertexCollectionToPolyline(this BulgeVertexCollection bulgeVertexCollection,
            ObjectId ownerId, CommandTransBase cmdTransBase)
        {
            using (var poly = new Polyline())
            {
                var iVertex = 0;
                foreach (BulgeVertex bv in bulgeVertexCollection)
                    poly.AddVertexAt(iVertex++, bv.Vertex, bv.Bulge, 0.0, 0.0);

                cmdTransBase.AddEntToBlockTableRecord(poly, ownerId);
                return poly.Id;
            }
        }

        public static ObjectId Line2DToDbLine(this LineSegment2d line2d, Plane plane, ObjectId ownerId,
            CommandTransBase cmdTransBase)
        {
            using (var ent = new Line())
            {
                ent.StartPoint = new Point3d(plane, line2d.StartPoint);
                ent.EndPoint = new Point3d(plane, line2d.EndPoint);
                cmdTransBase.AddEntToBlockTableRecord(ent, ownerId);
                return ent.Id;
            }
        }

        public static ObjectId Arc2DToDbArc(this CircularArc2d arc2d, Plane plane, ObjectId ownerId,
            CommandTransBase cmdTransBase)
        {
            if (arc2d.IsClosed() || Math.Abs(arc2d.EndAngle - arc2d.StartAngle) < 1e-5)
                using (var ent = new Circle(new Point3d(plane, arc2d.Center), plane.Normal,
                           arc2d.Radius))
                {
                    cmdTransBase.AddEntToBlockTableRecord(ent, ownerId);
                    return ent.Id;
                }

            if (arc2d.IsClockWise) arc2d = arc2d.GetReverseParameterCurve() as CircularArc2d;

            var angle = new Vector3d(plane, arc2d.ReferenceVector).AngleOnPlane(plane);
            var startAngle = arc2d.StartAngle + angle;
            var endAngle = arc2d.EndAngle + angle;
            using (var ent = new Arc(new Point3d(plane, arc2d.Center), plane.Normal,
                       arc2d.Radius, startAngle, endAngle))
            {
                cmdTransBase.AddEntToBlockTableRecord(ent, ownerId);
                return ent.Id;
            }
        }

        public static ObjectId Ellipse2DToDbEllipse(this EllipticalArc2d ellipse2d, Plane plane, ObjectId ownerId,
            CommandTransBase commandTransBase)
        {
            //-------------------------------------------------------------------------------------------
            // Bug: Can not assign StartParam and EndParam of Ellipse:
            // Ellipse ent = new Ellipse(new Point3d(plane, e2d.Center), plane.Normal, 
            //      new Vector3d(plane,e2d.MajorAxis) * e2d.MajorRadius,
            //      e2d.MinorRadius / e2d.MajorRadius, e2d.StartAngle, e2d.EndAngle);
            // ent.StartParam = e2d.StartAngle; 
            // ent.EndParam = e2d.EndAngle;
            // error CS0200: Property or indexer 'Autodesk.AutoCAD.DatabaseServices.Curve.StartParam' cannot be assigned to -- it is read only
            // error CS0200: Property or indexer 'Autodesk.AutoCAD.DatabaseServices.Curve.EndParam' cannot be assigned to -- it is read only
            //---------------------------------------------------------------------------------------------
            // Workaround is using Reflection
            // 
            using (var ent = new Ellipse(new Point3d(plane, ellipse2d.Center), plane.Normal,
                       new Vector3d(plane, ellipse2d.MajorAxis) * ellipse2d.MajorRadius,
                       ellipse2d.MinorRadius / ellipse2d.MajorRadius, ellipse2d.StartAngle,
                       ellipse2d.EndAngle))
            {
                ent.GetType().InvokeMember("StartParam", BindingFlags.SetProperty, null,
                    ent, new object[] { ellipse2d.StartAngle });
                ent.GetType().InvokeMember("EndParam", BindingFlags.SetProperty, null,
                    ent, new object[] { ellipse2d.EndAngle });
                commandTransBase.AddEntToBlockTableRecord(ent, ownerId);
                return ent.Id;
            }
        }

        public static ObjectId Spline2DToDbSpline(this NurbCurve2d spline2d, Plane plane, ObjectId ownerId,
            CommandTransBase cmdTransBase)
        {
            if (spline2d.HasFitData)
            {
                var n2fd = spline2d.FitData;
                using (var p3ds = new Point3dCollection())
                {
                    foreach (var p in n2fd.FitPoints) p3ds.Add(new Point3d(plane, p));
                    using (var ent = new Spline(p3ds, new Vector3d(plane, n2fd.StartTangent),
                               new Vector3d(plane, n2fd.EndTangent),
                               /* n2fd.KnotParam, */ n2fd.Degree, n2fd.FitTolerance.EqualPoint))
                    {
                        cmdTransBase.AddEntToBlockTableRecord(ent, ownerId);
                        return ent.Id;
                    }
                }
            }

            var n2dd = spline2d.DefinitionData;
            using (var p3ds = new Point3dCollection())
            {
                var knots = new DoubleCollection(n2dd.Knots.Count);
                foreach (var p in n2dd.ControlPoints) p3ds.Add(new Point3d(plane, p));
                foreach (double k in n2dd.Knots) knots.Add(k);
                double period = 0;
                using (var ent = new Spline(n2dd.Degree, n2dd.Rational,
                           spline2d.IsClosed(), spline2d.IsPeriodic(out period),
                           p3ds, knots, n2dd.Weights, n2dd.Knots.Tolerance, n2dd.Knots.Tolerance))
                {
                    cmdTransBase.AddEntToBlockTableRecord(ent, ownerId);

                    return ent.Id;
                }
            }
        }

        public static ObjectId Curve2DToDbCurve(this Curve2d cv, Plane plane, ObjectId ownerId,
            CommandTransBase cmdTransBase)
        {
            var line2d = cv as LineSegment2d;
            var arc2d = cv as CircularArc2d;
            var ellipse2d = cv as EllipticalArc2d;
            var spline2d = cv as NurbCurve2d;
            if (line2d != null) return line2d.Line2DToDbLine(plane, ownerId, cmdTransBase);
            if (arc2d != null) return arc2d.Arc2DToDbArc(plane, ownerId, cmdTransBase);
            if (ellipse2d != null) return ellipse2d.Ellipse2DToDbEllipse(plane, ownerId, cmdTransBase);
            if (spline2d != null) return spline2d.Spline2DToDbSpline(plane, ownerId, cmdTransBase);
            throw new Exception($"\nCan't create dbobject for Curve2D:[{cv.GetType()}]");
        }
    }
}