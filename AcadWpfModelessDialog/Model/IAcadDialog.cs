using Autodesk.AutoCAD.ApplicationServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelessDialogWithAction.Model
{
    public interface IAcadDialog
    {
        IntPtr DocumentPointer { get; }
        string NotifyMessage { get; set; }

        void RefreshEntityData(Document dwg);
    }
}
