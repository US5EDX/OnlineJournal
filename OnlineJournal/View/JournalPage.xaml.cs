using Microsoft.Win32;
using OnlineJournal.ViewModel;
using System.Windows;
using System.Windows.Controls;

namespace OnlineJournal.View
{
    public partial class JournalPage : Page
    {
        public JournalPage()
        {
            InitializeComponent();
            saveTableButton.Click += SaveTable;
            saveAcademicRecordButton.Click += SaveAcademicRecord;
        }

        private void SaveTable(object sender, RoutedEventArgs e)
        {
            var dialog = new SaveFileDialog();

            dialog.Filter = "Excel file (*.csv)|*.csv";

            if (dialog.ShowDialog() == true)
            {
                string result = (DataContext as JournalPageViewModel)?.SaveTable(dialog.FileName);

                if (Application.Current.Windows[1] != null)
                    MessageBox.Show(Application.Current.Windows[1], result);
            }
        }

        private void SaveAcademicRecord(object sender, RoutedEventArgs e)
        {
            var dialog = new SaveFileDialog();

            dialog.Filter = "Pdf files (*.pdf)|*.pdf";

            if (dialog.ShowDialog() == true)
            {
                if (Application.Current.Windows[1] == null)
                    return;

                var response = MessageBox.Show
                    (Application.Current.Windows[1], "Чи є форма семестрового контролю - Заліком?", "", MessageBoxButton.YesNo);

                string result = (DataContext as JournalPageViewModel)?.FromResult(dialog.FileName, response == MessageBoxResult.Yes);

                MessageBox.Show(Application.Current.Windows[1], result);
            }
        }
    }
}
