using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;

namespace OnlineJournal.MVVM
{
    public class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public void CloseApplication(object sender, CancelEventArgs e)
        {
            Application.Current.Shutdown();
        }

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetProperty<T>(ref T prop, T value, [CallerMemberName] string propertyName = null)
        {
            if (Equals(prop, value))
                return false;
            
            prop = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }
}
