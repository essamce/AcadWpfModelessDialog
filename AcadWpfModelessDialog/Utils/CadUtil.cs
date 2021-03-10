using System.Collections.Generic;
using System.Linq;

using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using AcadWpfModelessDialog.Model;

namespace AcadWpfModelessDialog.Utils
{
    public class CadUtil
    {
        public static IEnumerable<EntityInfo> CollectEntitiesInModelSpace(Document dwg)
        {
            var lst = new List<EntityInfo>();

            using (var tran = dwg.TransactionManager.StartTransaction())
            {
                var model = (BlockTableRecord)tran.GetObject(
                    SymbolUtilityServices.GetBlockModelSpaceId(dwg.Database), OpenMode.ForRead);
                foreach (ObjectId id in model)
                {
                    var ent = (Entity)tran.GetObject(id, OpenMode.ForRead);
                    lst.Add(new EntityInfo
                    {
                        EntityId = id,
                        EntityType = id.ObjectClass.DxfName,
                        EntityLayer = ent.Layer
                    });
                }

                tran.Commit();
            }

            return from e in lst
                   orderby
                   e.EntityType ascending,
                   e.EntityLayer ascending,
                   e.EntityId.ToString() ascending
                   select e;
        }

        public static ObjectId AddLine(Document dwg)
        {
            var res = dwg.Editor.GetPoint("\nSelect line's Start Point:");
            if (res.Status == PromptStatus.OK)
            {
                var start = res.Value;
                var opt = new PromptPointOptions(
                    "\nSelect line's End Point:");
                opt.UseBasePoint = true;
                opt.BasePoint = start;
                opt.UseDashedLine = true;
                res = dwg.Editor.GetPoint(opt);
                if (res.Status == PromptStatus.OK)
                {
                    var end = res.Value;
                    return CreateLine(dwg.Database, start, end);
                }
            }
            dwg.Editor.WriteMessage("\n*Cancel*");
            return ObjectId.Null;
        }

        public static ObjectId AddCircle(Document dwg)
        {
            var res = dwg.Editor.GetPoint("\nSelect circle's Center Point:");
            if (res.Status == PromptStatus.OK)
            {
                var center = res.Value;
                var opt = new PromptDoubleOptions(
                    "\nEnter circle's Radius:");
                opt.AllowNegative = false;
                opt.AllowNone = false;
                opt.DefaultValue = 300.0;

                var dRes = dwg.Editor.GetDouble(opt);
                if (dRes.Status == PromptStatus.OK)
                {
                    var radius = dRes.Value;
                    return CreateCircle(dwg.Database, center, radius);
                }
            }
            dwg.Editor.WriteMessage("\n*Cancel*");
            return ObjectId.Null;
        }

        private static ObjectId CreateLine(Database db, Point3d startPt, Point3d endPt)
        {
            var line = new Line(startPt, endPt);
            line.SetDatabaseDefaults(db);

            return AddEntityToModelSpace(db, line);
        }

        private static ObjectId CreateCircle(Database db, Point3d centerPt, double radius)
        {
            var circle = new Circle();
            circle.Center = centerPt;
            circle.Radius = radius;
            circle.SetDatabaseDefaults(db);

            return AddEntityToModelSpace(db, circle);
        }

        private static ObjectId AddEntityToModelSpace(Database db, Entity ent)
        {
            var id = ObjectId.Null;
            using (var tran = db.TransactionManager.StartTransaction())
            {
                var model = (BlockTableRecord)tran.GetObject(
                    SymbolUtilityServices.GetBlockModelSpaceId(db), OpenMode.ForWrite);
                model.AppendEntity(ent);
                tran.AddNewlyCreatedDBObject(ent, true);

                tran.Commit();
            }
            return id;
        }
    }
}