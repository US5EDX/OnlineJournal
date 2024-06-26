using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace OnlineJournal.MVVM
{
    public class PropertyChangedRealization : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
