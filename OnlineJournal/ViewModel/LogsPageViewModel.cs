using OnlineJournal.API;
using OnlineJournal.Model;
using OnlineJournal.MVVM;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace OnlineJournal.ViewModel
{
    class LogsPageViewModel : ViewModelBase
    {
        private ObservableCollection<Log> logs;
        private DateTime selectedDate;

        public Func<object, string, bool> Filter { get; set; }
        public ObservableCollection<Log> Logs { get => logs; set => SetProperty(ref logs, value); }
        public DateTime SelectedDate { get => selectedDate; set => SetProperty(ref selectedDate, value); }

        public RelayCommand UpdateCommand => new RelayCommand(execute => Update(), canExecute => CanUpadate());

        public LogsPageViewModel()
        {
            Filter = FilterLogs;
            SelectedDate = DateTime.Now.Date;
        }

        private bool FilterLogs(object obj, string searchText)
        {
            if (obj is Log log)
            {
                return log.ActionDateTime.ToString().ToLower().Contains(searchText.ToLower())
                    || log.UserEmail.ToLower().Contains(searchText.ToLower())
                    || log.ActionType.ToLower().Contains(searchText.ToLower())
                    || log.Description.ToLower().Contains(searchText.ToLower())
                    || (log.Changes != null && log.Changes.ToLower().Contains(searchText.ToLower()));
            }
            return false;
        }

        private bool CanUpadate()
        {
            return SelectedDate != null;
        }

        private void Update()
        {
            string url = ConfigurationManager.AppSettings["logUrl"] + "?date=" + selectedDate.ToString("yyyy-MM-dd");

            var logs = WebAPI.GetCall(url);

            if (logs.Result.StatusCode == System.Net.HttpStatusCode.OK)
            {
                Logs = new JavaScriptSerializer()
                    .Deserialize<ObservableCollection<Log>>(logs.Result.Content.ReadAsStringAsync().Result);
            }
        }
    }
}
