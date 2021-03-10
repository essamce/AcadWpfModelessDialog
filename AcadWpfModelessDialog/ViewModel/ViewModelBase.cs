using System.ComponentModel;

namespace AcadWpfModelessDialog.ViewModel
{
    public class ViewModelBase : INotifyPropertyChanged
    {
        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propName)
           => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        public void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string propName = "", char _ = '.')
          => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        // we use (char _ = '.') just to change mehtod signature
        #endregion INotifyPropertyChanged
    }
}