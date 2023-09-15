using System;
using System.Collections.Generic;
using Autodesk.AutoCAD.ApplicationServices.Core;
using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;

namespace PreviousDevelopmentToRefactor.Reference
{
    //添加图层的返回状态
    public enum AddLayerStatuts
    {
        AddLayerOK,
        IllegalLayerName,
        LayerNameExist
    }

    //添加图层的返回值
    public struct AddLayerResult
    {
        public AddLayerStatuts statuts;
        public string layerName;
    }

    //修改图层属性的返回状态
    public enum ChangeLayerPropertyStatus
    {
        ChangeOK,
        LayerIsNotExist
    }

    public static class LayerTool
    {
        /// <summary>
        ///     添加图层
        /// </summary>
        /// <param name="db">图形数据库</param>
        /// <param name="layerName">图层名</param>
        /// <returns>AddLayerResult</returns>
        public static AddLayerResult AddLayer(this Database db, string layerName)
        {
            //声明AddLayerResult类型的数据，用户返回
            var res = new AddLayerResult();
            try
            {
                SymbolUtilityServices.ValidateSymbolName(layerName, false);
            }
            catch (Exception)
            {
                res.statuts = AddLayerStatuts.IllegalLayerName;
                return res;
            }

            using (var trans = db.TransactionManager.StartTransaction())
            {
                //打开层表
                var lt = (LayerTable)trans.GetObject(db.LayerTableId, OpenMode.ForRead);
                //新建层表记录
                if (!lt.Has(layerName))
                {
                    var ltr = new LayerTableRecord { Name = layerName };
                    //判断要创建的图层名是否已经存在,不存在则创建
                    //升级层表打开权限
                    lt.UpgradeOpen();
                    lt.Add(ltr);
                    //降低层表打开权限
                    lt.DowngradeOpen();
                    trans.AddNewlyCreatedDBObject(ltr, true);
                    trans.Commit();
                    res.statuts = AddLayerStatuts.AddLayerOK;
                    res.layerName = layerName;
                }
                else
                {
                    res.statuts = AddLayerStatuts.LayerNameExist;
                }
            }

            return res;
        }


        /// <summary>
        ///     修改图层颜色
        /// </summary>
        /// <param name="db">图形数据库</param>
        /// <param name="LayerName">图层名</param>
        /// <param name="colorIndex">图层颜色</param>
        /// <returns>ChangeLayerPropertyStatus</returns>
        public static ChangeLayerPropertyStatus ChangeLayerColor(this Database db, string LayerName, short colorIndex)
        {
            ChangeLayerPropertyStatus status;
            using (var trans = db.TransactionManager.StartTransaction())
            {
                //打开层表
                var lt = (LayerTable)trans.GetObject(db.LayerTableId, OpenMode.ForRead);
                //判断指定的图形名是否存在
                if (lt.Has(LayerName))
                {
                    var ltr = (LayerTableRecord)lt[LayerName].GetObject(OpenMode.ForWrite);
                    ltr.Color = Color.FromColorIndex(ColorMethod.ByAci, colorIndex);
                    trans.Commit();
                    status = ChangeLayerPropertyStatus.ChangeOK;
                }
                else
                {
                    status = ChangeLayerPropertyStatus.LayerIsNotExist;
                }
            }

            return status;
        }


        /// <summary>
        ///     锁定图层
        /// </summary>
        /// <param name="db">图形数据库</param>
        /// <param name="LayerName">图层名</param>
        /// <returns>bool</returns>
        public static bool LockLayer(this Database db, string LayerName)
        {
            var isOk = true;
            using (var trans = db.TransactionManager.StartTransaction())
            {
                //打开层表
                var lt = (LayerTable)trans.GetObject(db.LayerTableId, OpenMode.ForRead);
                //判断指定的图形名是否存在
                if (lt.Has(LayerName))
                {
                    var ltr = (LayerTableRecord)lt[LayerName].GetObject(OpenMode.ForWrite);
                    ltr.IsLocked = true;
                    trans.Commit();
                }
                else
                {
                    isOk = false;
                }
            }

            return isOk;
        }


        /// <summary>
        ///     解除锁定图层
        /// </summary>
        /// <param name="db">图形数据库</param>
        /// <param name="LayerName">图层名</param>
        /// <returns>bool</returns>
        public static bool UnLockLayer(this Database db, string LayerName)
        {
            var isOk = true;
            using (var trans = db.TransactionManager.StartTransaction())
            {
                //打开层表
                var lt = (LayerTable)trans.GetObject(db.LayerTableId, OpenMode.ForRead);
                //判断指定的图形名是否存在
                if (lt.Has(LayerName))
                {
                    var ltr = (LayerTableRecord)lt[LayerName].GetObject(OpenMode.ForWrite);
                    ltr.IsLocked = false;
                    trans.Commit();
                }
                else
                {
                    isOk = false;
                }
            }

            return isOk;
        }


        /// <summary>
        ///     修改图层的线宽
        /// </summary>
        /// <param name="db">图形数据库</param>
        /// <param name="LayerName">图层名</param>
        /// <param name="lineWeight">线宽</param>
        /// <returns>bool</returns>
        public static bool ChangleLineWeight(this Database db, string LayerName, LineWeight lineWeight)
        {
            var isOk = true;
            using (var trans = db.TransactionManager.StartTransaction())
            {
                //打开层表
                var lt = (LayerTable)trans.GetObject(db.LayerTableId, OpenMode.ForRead);
                //判断指定的图形名是否存在
                if (lt.Has(LayerName))
                {
                    var ltr = (LayerTableRecord)lt[LayerName].GetObject(OpenMode.ForWrite);
                    ltr.LineWeight = lineWeight;
                    trans.Commit();
                }
                else
                {
                    isOk = false;
                }
            }

            return isOk;
        }


        /// <summary>
        ///     设置当前图层
        /// </summary>
        /// <param name="db">图形数据库</param>
        /// <param name="LayerName">图层名</param>
        /// <returns>bool</returns>
        public static bool SetCurrentLayer(this Database db, string LayerName)
        {
            var isSetOk = false;
            using (var trans = db.TransactionManager.StartTransaction())
            {
                //打开层表
                var lt = (LayerTable)trans.GetObject(db.LayerTableId, OpenMode.ForRead);
                //判断要设置的图层名是否存在
                if (lt.Has(LayerName))
                {
                    //获取指定图层名的ObjectId
                    var layerId = lt[LayerName];
                    //判断要设置的图形是否已是当前图层
                    if (db.Clayer != layerId) db.Clayer = layerId;
                    isSetOk = true;
                }

                trans.Commit();
            }

            return isSetOk;
        }


        public static List<LayerTableRecord> GetAllLayers(this Database db)
        {
            var layerList = new List<LayerTableRecord>();
            using (var trans = db.TransactionManager.StartTransaction())
            {
                //打开层表
                var lt = (LayerTable)trans.GetObject(db.LayerTableId, OpenMode.ForRead);
                lt.GenerateUsageData();
                foreach (var item in lt)
                {
                    var ltr = (LayerTableRecord)item.GetObject(OpenMode.ForRead);
                    layerList.Add(ltr);
                }
            }

            return layerList;
        }


        /// <summary>
        ///     获取所有图层名
        /// </summary>
        /// <param name="db">图形数据库</param>
        /// <returns>List<string></returns>
        public static List<string> GetAllLayersName(this Database db)
        {
            var layerNamesList = new List<string>();
            using (var trans = db.TransactionManager.StartTransaction())
            {
                //打开层表
                var lt = (LayerTable)trans.GetObject(db.LayerTableId, OpenMode.ForRead);
                foreach (var item in lt)
                {
                    var ltr = (LayerTableRecord)item.GetObject(OpenMode.ForRead);
                    layerNamesList.Add(ltr.Name);
                }
            }

            return layerNamesList;
        }


        /// <summary>
        ///     删除图层
        /// </summary>
        /// <param name="db">图形数据库</param>
        /// <param name="layerName">图层名</param>
        /// <returns>bool</returns>
        public static bool DeleteLayer(this Database db, string layerName)
        {
            if (layerName == "0" || layerName == "Defpoints") return false;
            var isDeleteOK = false;
            using (var trans = db.TransactionManager.StartTransaction())
            {
                var lt = (LayerTable)trans.GetObject(db.LayerTableId, OpenMode.ForRead);
                lt.GenerateUsageData();
                //判断要删除的图层名是否存在
                if (lt.Has(layerName))
                {
                    var ltr = (LayerTableRecord)lt[layerName].GetObject(OpenMode.ForWrite);
                    if (!ltr.IsUsed && db.Clayer != lt[layerName])
                    {
                        ltr.Erase();
                        isDeleteOK = true;
                    }
                }
                else
                {
                    isDeleteOK = true;
                }

                trans.Commit();
            }

            return isDeleteOK;
        }


        /// <summary>
        ///     删除所有未使用的图层
        /// </summary>
        /// <param name="db"></param>
        public static void DeleteNotUsedLayer(this Database db)
        {
            using (var trans = db.TransactionManager.StartTransaction())
            {
                var lt = (LayerTable)trans.GetObject(db.LayerTableId, OpenMode.ForRead);
                lt.GenerateUsageData();
                foreach (var item in lt)
                {
                    var ltr = (LayerTableRecord)item.GetObject(OpenMode.ForWrite);
                    if (!ltr.IsUsed) ltr.Erase();
                }

                trans.Commit();
            }
        }

        /// <summary>
        ///     强行删除图层及图层上的所有实体对象
        /// </summary>
        /// <param name="db">图形数据库</param>
        /// <param name="layerName">图层名</param>
        /// <param name="delete"></param>
        /// <returns></returns>
        public static bool DeleteLayer(this Database db, string layerName, bool delete)
        {
            if (layerName == "0" || layerName == "Defpoints") return false;
            var isDeleteOK = false;
            using (var trans = db.TransactionManager.StartTransaction())
            {
                var lt = (LayerTable)trans.GetObject(db.LayerTableId, OpenMode.ForRead);
                lt.GenerateUsageData();
                if (lt.Has(layerName))
                {
                    var ltr = (LayerTableRecord)lt[layerName].GetObject(OpenMode.ForWrite);
                    if (delete)
                    {
                        if (ltr.IsUsed) ltr.deleteAllEntityInLayer();
                        if (db.Clayer == ltr.ObjectId) db.Clayer = lt["0"];
                        ltr.Erase();
                        isDeleteOK = true;
                    }
                    else
                    {
                        if (!ltr.IsUsed && db.Clayer != lt[layerName])
                        {
                            ltr.Erase();
                            isDeleteOK = true;
                        }
                    }
                }
                else
                {
                    isDeleteOK = true;
                }

                trans.Commit();
            }

            return isDeleteOK;
        }


        /// <summary>
        ///     删除指定图层上的所有实体对象
        /// </summary>
        /// <param name="ltr"></param>
        public static void deleteAllEntityInLayer(this LayerTableRecord ltr)
        {
            var db = HostApplicationServices.WorkingDatabase;
            var ed = Application.DocumentManager.MdiActiveDocument.Editor;
            TypedValue[] value =
            {
                new TypedValue((int)DxfCode.LayerName, ltr.Name)
            };
            var filter = new SelectionFilter(value);
            var psr = ed.SelectAll(filter);
            if (psr.Status == PromptStatus.OK)
            {
                var ids = psr.Value.GetObjectIds();
                using (var trans = db.TransactionManager.StartTransaction())
                {
                    for (var i = 0; i < ids.Length; i++)
                    {
                        var ent = (Entity)ids[i].GetObject(OpenMode.ForWrite);
                        ent.Erase();
                    }

                    trans.Commit();
                }
            }
        }
    }
}