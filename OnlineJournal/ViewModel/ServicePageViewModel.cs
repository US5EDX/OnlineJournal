using OnlineJournal.API;
using OnlineJournal.Model;
using OnlineJournal.MVVM;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Linq;
using System.Web.Script.Serialization;

namespace OnlineJournal.ViewModel
{
    class ServicePageViewModel : ViewModelBase
    {
        private ObservableCollection<string> singleChooseOptions;
        private string message;
        private int actionType;
        private string selectedRestoreOption;
        private bool isAcceptButtonVisible;
        private bool isArchiveTableChosen;
        private bool isRestoreChosen;

        private ObservableCollection<string> RestoreDBOptions { get; set; }
        private ObservableCollection<string> RestoreTableDBOptions { get; set; }
        private ObservableCollection<string> RestoreLogsOptions { get; set; }

        public ObservableCollection<TableOption> TableOptions { get; set; }
        public ObservableCollection<string> SingleChooseOptions { get => singleChooseOptions; set => SetProperty(ref singleChooseOptions, value); }

        public string Message
        {
            get { return message; }
            set
            {
                SetProperty(ref message, value);
            }
        }

        public string SelectedRestoreOption
        {
            get { return selectedRestoreOption; }
            set
            {
                SetProperty(ref selectedRestoreOption, value);
            }
        }

        public bool IsAcceptButtonVisible { get => isAcceptButtonVisible; set => SetProperty(ref isAcceptButtonVisible, value); }
        public bool IsArchiveTableChosen { get => isArchiveTableChosen; set => SetProperty(ref isArchiveTableChosen, value); }
        public bool IsRestoreChosen { get => isRestoreChosen; set => SetProperty(ref isRestoreChosen, value); }

        public RelayCommand ArchiveJournalDBCommand => new RelayCommand(execute => ArchiveDB(), canExecute => CanArchiveDB());
        public RelayCommand ArchiveTablesJournalDBCommand => new RelayCommand(execute => ArchiveTables(), canExecute => CanArchiveTables());
        public RelayCommand RestoreJournalDBCommand => new RelayCommand(execute => RestoreDB(), canExecute => CanRestoreDB());
        public RelayCommand RestoreTableJournalDBCommand => new RelayCommand(execute => RestoreTable(), canExecute => CanRestoreTable());
        public RelayCommand ArchiveLogsCommand => new RelayCommand(execute => ArchiveLogs(), canExecute => CanArchiveLogs());
        public RelayCommand RestoreLogsCommand => new RelayCommand(execute => RestoreLogs(), canExecute => CanRestoreLogs());
        public RelayCommand ClearLogsCommand => new RelayCommand(execute => ClearLogs(), canExecute => CanClearLogs());

        public RelayCommand ExecuteCommand => new RelayCommand(execute => Execute(), canExecute => CanExecute());

        public ServicePageViewModel()
        {
            LoadTablesInfo();
            RestoreDBOptions = LoadRestoreInfo(1);
            RestoreTableDBOptions = LoadRestoreInfo(2);
            RestoreLogsOptions = LoadRestoreInfo(3);
        }

        private void LoadTablesInfo()
        {
            var tables = WebAPI.GetCall(ConfigurationManager.AppSettings["serviceUrl"] + "\\GetTableNames");

            if (tables.Result.StatusCode == System.Net.HttpStatusCode.OK)
            {
                TableOptions = new JavaScriptSerializer()
                    .Deserialize<ObservableCollection<TableOption>>(tables.Result.Content.ReadAsStringAsync().Result);
            }
        }

        private ObservableCollection<string> LoadRestoreInfo(int option)
        {
            var files = WebAPI.GetCall(ConfigurationManager.AppSettings["serviceUrl"] + "\\GetRestoreOptions?option=" + option);

            if (files.Result.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return new JavaScriptSerializer()
                    .Deserialize<ObservableCollection<string>>(files.Result.Content.ReadAsStringAsync().Result);
            }

            return new ObservableCollection<string>();
        }

        private bool CanArchiveDB()
        {
            return actionType == 0 || actionType == 1;
        }

        private void ArchiveDB()
        {
            IsAcceptButtonVisible = !IsAcceptButtonVisible;
            actionType = isAcceptButtonVisible ? 1 : 0;
        }

        private bool CanArchiveTables()
        {
            return actionType == 0 || actionType == 2;
        }

        private void ArchiveTables()
        {
            IsArchiveTableChosen = !IsArchiveTableChosen;
            IsAcceptButtonVisible = !IsAcceptButtonVisible;
            actionType = isAcceptButtonVisible ? 2 : 0;
        }

        private bool CanRestoreDB()
        {
            return actionType == 0 || actionType == 3;
        }

        private void RestoreDB()
        {
            IsRestoreChosen = !IsRestoreChosen;
            IsAcceptButtonVisible = !IsAcceptButtonVisible;
            SingleChooseOptions = RestoreDBOptions;
            actionType = isAcceptButtonVisible ? 3 : 0;
        }

        private bool CanRestoreTable()
        {
            return actionType == 0 || actionType == 4;
        }

        private void RestoreTable()
        {
            IsRestoreChosen = !IsRestoreChosen;
            IsAcceptButtonVisible = !IsAcceptButtonVisible;
            SingleChooseOptions = RestoreTableDBOptions;
            actionType = isAcceptButtonVisible ? 4 : 0;
        }

        private bool CanArchiveLogs()
        {
            return actionType == 0 || actionType == 5;
        }

        private void ArchiveLogs()
        {
            IsAcceptButtonVisible = !IsAcceptButtonVisible;
            actionType = isAcceptButtonVisible ? 5 : 0;
        }

        private bool CanRestoreLogs()
        {
            return actionType == 0 || actionType == 6;
        }

        private void RestoreLogs()
        {
            IsRestoreChosen = !IsRestoreChosen;
            IsAcceptButtonVisible = !IsAcceptButtonVisible;
            SingleChooseOptions = RestoreLogsOptions;
            actionType = isAcceptButtonVisible ? 6 : 0;
        }

        private bool CanClearLogs()
        {
            return actionType == 0 || actionType == 7;
        }

        private void ClearLogs()
        {
            IsAcceptButtonVisible = !IsAcceptButtonVisible;
            actionType = isAcceptButtonVisible ? 7 : 0;
        }

        private bool CanExecute()
        {
            return !(IsArchiveTableChosen && TableOptions.Count(ta => ta.IsCheked) == 0);
        }

        private void Execute()
        {
            switch (actionType)
            {
                case 1:
                    Message = ExecPostCall("\\ArchiveDB?option=" + 1) ? "Успішно" : "Неуспішно";
                    ArchiveDB();
                    return;
                case 2:
                    var selectedTables = TableOptions.Where(ta => ta.IsCheked).Select(ta => ta.Id).ToList();
                    Message = ExecPostCall("\\ArchiveOnlineJournalTables", selectedTables) ? "Успішно" : "Неуспішно";
                    ArchiveTables();
                    return;
                case 3:
                    Message = ExecPostCall("\\RestoreDB?option=" + 1, SelectedRestoreOption) ? "Успішно" : "Неуспішно";
                    RestoreDB();
                    return;
                case 4:
                    Message = ExecPostCall("\\RestoreOnlineJournalTable", SelectedRestoreOption) ? "Успішно" : "Неуспішно";
                    RestoreTable();
                    return;
                case 5:
                    Message = ExecPostCall("\\ArchiveDB?option=" + 2) ? "Успішно" : "Неуспішно";
                    ArchiveLogs();
                    return;
                case 6:
                    Message = ExecPostCall("\\RestoreDB?option=" + 2, SelectedRestoreOption) ? "Успішно" : "Неуспішно";
                    RestoreLogs();
                    return;
                case 7:
                    Message = ExecPostCall("\\ClearLogs") ? "Успішно" : "Неуспішно";
                    ClearLogs();
                    return;
            }
        }

        private bool ExecPostCall(string url, object newObject = null)
        {
            var response = WebAPI.PostCall(ConfigurationManager.AppSettings["serviceUrl"] + url, newObject);

            if (response.Result.StatusCode == System.Net.HttpStatusCode.OK)
                return true;

            return false;
        }
    }
}
