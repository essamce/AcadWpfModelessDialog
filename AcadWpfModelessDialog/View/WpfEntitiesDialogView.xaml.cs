using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using AcadWpfModelessDialog.ViewModel;

namespace AcadWpfModelessDialog.View
{
    /// <summary>
    /// Interaction logic for WpfEntitiesDialogView.xaml
    /// </summary>
    public partial class WpfEntitiesDialogView : Window
    {
        WpfEntitiesDialogViewModel _viewModel;
        public WpfEntitiesDialogView(WpfEntitiesDialogViewModel viewModel)
        {
            #region initialize
            _viewModel = viewModel;
            DataContext = _viewModel;
            //if (_viewModel != null)
            //{
            //    _viewModel.Dialog = this;
            //}
            InitializeComponent();
            #endregion
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            e.Cancel = true;
            this.Hide();

            base.OnClosing(e);            
        }

    }
}
