using System;
using System.Windows.Forms;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.DatabaseServices.Filters;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using PreviousDevelopmentToRefactor;
using Exception = System.Exception;
using MgdAcApplication = Autodesk.AutoCAD.ApplicationServices.Core.Application;

[assembly: CommandClass(typeof(XClipRetriever))]

namespace PreviousDevelopmentToRefactor
{
    public class XClipRetriever
    {
        [CommandMethod("GetXClip", CommandFlags.Modal | CommandFlags.UsePickSet)]
        public static void RetrieveXClipBoundary_Method()
        {
            var ed = MgdAcApplication.DocumentManager.MdiActiveDocument.Editor;
            try
            {
                if (ed.SelectImplied().Status != PromptStatus.OK) throw new Exception("Nothing has been pre-selected!");

                var BlockReferenceRXClass = RXObject.GetClass(typeof(BlockReference));
                using (var tr = MgdAcApplication.DocumentManager.MdiActiveDocument.TransactionManager
                           .StartTransaction())
                {
                    foreach (var id in ed.SelectImplied().Value.GetObjectIds())
                        if (id.ObjectClass == BlockReferenceRXClass)
                        {
                            var blkRef = (BlockReference)tr.GetObject(id, OpenMode.ForRead);
                            if (blkRef.ExtensionDictionary != ObjectId.Null)
                            {
                                var extdict = (DBDictionary)tr.GetObject(blkRef.ExtensionDictionary, OpenMode.ForRead);
                                if (extdict.Contains("ACAD_FILTER"))
                                {
                                    var dict = (DBDictionary)tr.GetObject(extdict.GetAt("ACAD_FILTER"),
                                        OpenMode.ForRead);
                                    if (dict.Contains("SPATIAL"))
                                    {
                                        var filter = (SpatialFilter)tr.GetObject(dict.GetAt("SPATIAL"),
                                            OpenMode.ForRead);
                                        MessageBox.Show(filter.Id.ToString());
                                        DrawPolygon(blkRef.Database, filter.Definition.Normal,
                                            blkRef.BlockTransform,
                                            filter.Definition.GetPoints());
                                    }
                                }
                            }
                        }

                    tr.Commit();
                }
            }
            catch (Exception ex)
            {
                ed.WriteMessage(Environment.NewLine + ex.Message);
            }
        }

        public static ObjectId DrawPolygon(Database db, Vector3d normal, Matrix3d mat, Point2dCollection vertices)
        {
            var ret = ObjectId.Null;

            var tr = db.TransactionManager.TopTransaction;
            var btr = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite);
            if (vertices.Count > 2)
                using (var pl = new Polyline())
                {
                    pl.SetDatabaseDefaults();
                    pl.ColorIndex = 3;
                    pl.Closed = true;
                    for (var i = 0; i < vertices.Count; i++) pl.AddVertexAt(0, vertices[i], 0, 0, 0);
                    pl.TransformBy(mat);
                    btr.AppendEntity(pl);
                    tr.AddNewlyCreatedDBObject(pl, true);
                    ret = pl.ObjectId;
                }
            else
                using (var pl = new Polyline())
                {
                    pl.SetDatabaseDefaults();
                    pl.ColorIndex = 3;
                    pl.Closed = true;
                    pl.AddVertexAt(0, vertices[0], 0, 0, 0);
                    pl.AddVertexAt(1, new Point2d(vertices[0].X, vertices[1].Y), 0, 0, 0);
                    pl.AddVertexAt(2, vertices[1], 0, 0, 0);
                    pl.AddVertexAt(3, new Point2d(vertices[1].X, vertices[0].Y), 0, 0, 0);
                    pl.TransformBy(mat);
                    btr.AppendEntity(pl);
                    tr.AddNewlyCreatedDBObject(pl, true);
                    ret = pl.ObjectId;
                }

            return ret;
        }
    }
}