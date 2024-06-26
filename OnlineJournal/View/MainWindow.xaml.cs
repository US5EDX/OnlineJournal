using OnlineJournal.ViewModel;
using System.Windows;

namespace OnlineJournal
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            LogInWindowViewModel vm = new LogInWindowViewModel();
            DataContext = vm;
        }
    }
}
