using Autodesk.AutoCAD.ApplicationServices;
using ModelessDialogWithAction.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using AcadWpfModelessDialog.Model;
using AcadWpfModelessDialog.Utils;
using CadApp = Autodesk.AutoCAD.ApplicationServices.Application;

namespace AcadWpfModelessDialog.ViewModel
{
    public class WpfEntitiesDialogViewModel : ViewModelBase//, IAcadDialog
    {
        #region props
        // model
        public ObservableCollection<EntityInfo> AcadEntities { get; set; }
        // view
        public Array SupportedAcadEntities { get => Enum.GetValues(typeof(SupportedAcadEntity)); }

        private string _notifyMessage;
        public string NotifyMessage
        {
            get => _notifyMessage;
            set
            {
                _notifyMessage = value;
                OnPropertyChanged();
            }
        }
        private bool _isBusy;
        public bool IsBusy
        {
            get => _isBusy;
            set
            {
                if (value == _isBusy) return;
                _isBusy = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region ctor
        public WpfEntitiesDialogViewModel()
        {
            AcadEntities = new ObservableCollection<EntityInfo>();
            AddEntityCmd = new DelegateCommand(OnAddEntity);
            RefreshCmd = new DelegateCommand(OnRefresh);
        }
        public WpfEntitiesDialogViewModel(DocumentCollection dwgManager) : this()
        {
            _dwgManager = dwgManager;
            _dwgManager.DocumentActivated += (o, docCollection) =>
            {
                if (docCollection.Document.UnmanagedObject != DocumentPointer)
                {
                    try
                    {
                        IsBusy = true;
                        RefreshEntityData(docCollection.Document);
                    }
                    finally
                    {
                        IsBusy = false;
                    }
                }
            };
            RefreshEntityData(_dwgManager.MdiActiveDocument);
        }
        #endregion

        #region methods
        private void OnRefresh(object param)
        {
            RefreshEntityData(_dwgManager.MdiActiveDocument);
            NotifyMessage = string.Empty;
        }
        private void OnAddEntity(object param)
        {
            // get command param
            var validParam = Enum.TryParse(param.ToString(), out SupportedAcadEntity entityType);
            if (validParam == false) return;

            //IsBusy = true;
            //System.Threading.Thread.Sleep(2 * 1000);
            //IsBusy = false;

            // run the corresponding cmd
            switch (entityType)
            {
                case SupportedAcadEntity.Line:
                    ExecuteCommand(AcadCommands.ADD_LINE_COMMAND);
                    break;

                case SupportedAcadEntity.Circle:
                    ExecuteCommand(AcadCommands.ADD_CIRCLE_COMMAND);
                    break;

                //case SupportedAcadEntity.Polyline:
                //    NotifyMessage += $"\n {entityType} Not Implemented";
                //    break;

                default:
                    NotifyMessage += $"\n {entityType} Not Implemented";
                    break;
            }

        }
        #endregion

        #region Acad Dialog
        private readonly DocumentCollection _dwgManager = null;
        private string _currentCommand = "";
        public IntPtr DocumentPointer { private set; get; }
        public void RefreshEntityData(Document acDoc)
        {
            try
            {
                IsBusy = true;

                AcadEntities.Clear();
                CadUtil
                    .CollectEntitiesInModelSpace(acDoc)
                    .ToList()
                    .ForEach(AcadEntities.Add);
            }
            finally
            {
                IsBusy = false;
            }

            DocumentPointer = acDoc.UnmanagedObject;
        }
        private void ExecuteCommand(string commandName)
        {
            _dwgManager.MdiActiveDocument.CommandEnded += MdiActiveDocument_CommandEnded;
            _currentCommand = commandName;
            CadApp.MainWindow.Focus();
            _dwgManager.MdiActiveDocument.SendStringToExecute(commandName + "\n", true, false, false);
        }
        private void MdiActiveDocument_CommandEnded(object sender, CommandEventArgs e)
        {
            if (e.GlobalCommandName.ToUpper() == _currentCommand.ToUpper())
            {
                _dwgManager.MdiActiveDocument.CommandEnded -= MdiActiveDocument_CommandEnded;

                RefreshEntityData(_dwgManager.MdiActiveDocument);
            }
        }
        #endregion

        #region Cmd
        public ICommand AddEntityCmd { get; set; }
        public ICommand RefreshCmd { get; set; }
        #endregion
    }

}
