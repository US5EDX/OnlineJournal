using OnlineJournal.API;
using OnlineJournal.Model;
using OnlineJournal.MVVM;
using OnlineJournal.Processings;
using System;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Web.Script.Serialization;

namespace OnlineJournal.ViewModel
{
    class TasksPageViewModel : ViewModelBase
    {
        private Task selectedTask;
        private string newTask;
        private bool isRegisterVisible;
        private string message;
        private string courseCode;
        private bool isUserResponsible;

        public ObservableCollection<Task> Tasks { get; set; }

        public Func<object, string, bool> Filter { get; set; }

        public Task SelectedTask
        {
            get { return selectedTask; }
            set
            {
                SetProperty(ref selectedTask, value);
            }
        }

        public string NewTask
        {
            get { return newTask; }
            set
            {
                SetProperty(ref newTask, value);
            }
        }

        public bool IsRegisterVisible
        {
            private get { return isRegisterVisible; }
            set
            {
                SetProperty(ref isRegisterVisible, value);
            }
        }

        public string Message
        {
            get { return message; }
            set
            {
                SetProperty(ref message, value);
            }
        }

        public string CourseName { get; set; }

        public GlobalViewModel Global { get; } = GlobalViewModel.Instance;

        public RelayCommand RegisterCommand => new RelayCommand(execute => OpenRegister());
        public RelayCommand ReturnCommand => new RelayCommand(execute => OpenJournal());

        public RelayCommand AddCommand => new RelayCommand(execute => Add(), canExecute => CanAdd());
        public RelayCommand ChangeCommand => new RelayCommand(execute => Update(), canExecute => CanUpadate());
        public RelayCommand DeleteCommand => new RelayCommand(execute => Delete(), canExecute => CanDelete());

        public TasksPageViewModel(string courseCode, string courseName, bool isUserResponsible)
        {
            this.courseCode = courseCode;
            CourseName = courseName;
            this.isUserResponsible = isUserResponsible;

            Filter = FilterTasks;
            IsRegisterVisible = false;

            var groups = WebAPI.GetCall(ConfigurationManager.AppSettings["taskUrl"] + "?courseCode=" + courseCode);

            if (groups.Result.StatusCode == System.Net.HttpStatusCode.OK)
            {
                Tasks = new JavaScriptSerializer()
                    .Deserialize<ObservableCollection<Task>>(groups.Result.Content.ReadAsStringAsync().Result);
            }
        }

        private bool FilterTasks(object obj, string searchText)
        {
            if (obj is Task task)
            {
                return task.Name.ToLower().Contains(searchText.ToLower());
            }
            return false;
        }

        private bool CanAdd()
        {
            Validation validation = new Validation();
            return validation.ValidateText(NewTask);
        }

        private void Add()
        {
            var serverResponse = WebAPI.PostCall(ConfigurationManager.AppSettings["taskUrl"], (NewTask, courseCode));
            if (serverResponse.Result.StatusCode == System.Net.HttpStatusCode.Created)
            {
                Task newTask = new JavaScriptSerializer()
                    .Deserialize<Task>(serverResponse.Result.Content.ReadAsStringAsync().Result);
                Tasks.Add(newTask);
                OpenRegister();
                Message = "Успішно додано нове завдання";
            }
            else
            {
                Message = "Сталася помилка додавання";
            }
        }

        private bool CanUpadate()
        {
            return !IsRegisterVisible && SelectedTask != null;
        }

        private void Update()
        {
            var serverResponse = WebAPI.PutCall(ConfigurationManager.AppSettings["taskUrl"]
                + "?id=" + SelectedTask.Id, SelectedTask.Name);
            if (serverResponse.Result.StatusCode == System.Net.HttpStatusCode.OK)
            {
                Message = "Успішно оновлено вибраний рядок";
            }
            else
            {
                Message = "Сталася помилка оновлення вибраного рядка";
            }
        }

        private bool CanDelete()
        {
            return !IsRegisterVisible && SelectedTask != null;
        }

        private void Delete()
        {
            var serverResponse = WebAPI.DeleteCall(ConfigurationManager.AppSettings["taskUrl"]
               + "?id=" + SelectedTask.Id);
            if (serverResponse.Result.StatusCode == System.Net.HttpStatusCode.OK)
            {
                Message = "Успішно видалено завдання";
                Tasks.Remove(SelectedTask);
                SelectedTask = null;
            }
            else
            {
                Message = "Сталася помилка видалення завдання";
            }
        }

        private void OpenRegister()
        {
            IsRegisterVisible = !IsRegisterVisible;
            NewTask = IsRegisterVisible ? string.Empty : null;
            SelectedTask = null;
        }

        private void OpenJournal()
        {
            Global.CurrentChildView = new JournalPageViewModel(courseCode, CourseName, isUserResponsible);
        }
    }
}
