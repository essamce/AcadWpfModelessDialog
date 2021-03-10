using Autodesk.AutoCAD.DatabaseServices;

namespace AcadWpfModelessDialog.Model
{
    public class EntityInfo
    {
        public ObjectId EntityId { set; get; }
        public string EntityType { set; get; }
        public string EntityLayer { set; get; }
    }
}
