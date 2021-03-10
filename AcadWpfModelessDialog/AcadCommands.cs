using Autodesk.AutoCAD.Runtime;
using CadApp = Autodesk.AutoCAD.ApplicationServices.Application;
using AcadWpfModelessDialog.View;
using AcadWpfModelessDialog.ViewModel;
using AcadWpfModelessDialog.Utils;
using ModelessDialogWithAction.Model;

//[assembly: CommandClass(typeof(ModelessDialogWithAction.MyCommands))]
//[assembly: ExtensionApplication(typeof(ModelessDialogWithAction.MyCommands))]

namespace AcadWpfModelessDialog
{
    public class AcadCommands : IExtensionApplication
    {
        public const string ADD_LINE_COMMAND = "ADDLINE";
        public const string ADD_CIRCLE_COMMAND = "ADDCIRCLE";

        private static WpfEntitiesDialogView _dialog = null;
        private static WpfEntitiesDialogViewModel _dialogViewModel = null;

        #region IExtensionApplication

        public void Initialize()
        {
            var dwg = CadApp.DocumentManager.MdiActiveDocument;
            var ed = dwg.Editor;

            try
            {
                ed.WriteMessage($"\nInitializing custom add-in \"{this.GetType().Name}\"...");

                _dialogViewModel = new WpfEntitiesDialogViewModel(CadApp.DocumentManager);
                _dialog = new WpfEntitiesDialogView(_dialogViewModel);

                ed.WriteMessage($"\nIntializing done.\n");
            }
            catch (System.Exception ex)
            {
                ed.WriteMessage($"\nInitializing error:\n{ex.Message}\n");
            }
        }

        public void Terminate()
        {

        }

        #endregion

        [CommandMethod("ShowWpfDialog", CommandFlags.Session)]
        public static void ShowWpfDialog()
        {
            var dwg = CadApp.DocumentManager.MdiActiveDocument;
            if (_dialogViewModel.DocumentPointer != dwg.UnmanagedObject)
            {
                _dialogViewModel.RefreshEntityData(dwg);
            }

            CadApp.ShowModelessWindow(_dialog);
        }

        [CommandMethod(ADD_LINE_COMMAND, CommandFlags.NoHistory)]
        public static void AddLineCommand()
        {
            var dwg = CadApp.DocumentManager.MdiActiveDocument;
            var ed = dwg.Editor;

            try
            {
                CadUtil.AddLine(dwg);
            }
            catch (System.Exception ex)
            {
                ed.WriteMessage($"\nError:\n{ex.Message}\n");
            }
        }

        [CommandMethod(ADD_CIRCLE_COMMAND, CommandFlags.NoHistory)]
        public static void AddCircleCommand()
        {
            var dwg = CadApp.DocumentManager.MdiActiveDocument;
            var ed = dwg.Editor;

            try
            {
                CadUtil.AddCircle(dwg);
            }
            catch (System.Exception ex)
            {
                ed.WriteMessage($"\nError:\n{ex.Message}\n");
            }
        }

    }
}