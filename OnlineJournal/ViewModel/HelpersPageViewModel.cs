using OnlineJournal.API;
using OnlineJournal.Model;
using OnlineJournal.MVVM;
using System;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Web.Script.Serialization;

namespace OnlineJournal.ViewModel
{
    class HelpersPageViewModel : ViewModelBase
    {
        private PairItemAsProperty selectedHelper;
        private string oldSelectedHelperEmail;
        private PairItemAsProperty newHelper;
        private bool isRegisterVisible;
        private string message;
        private string courseCode;

        public ObservableCollection<PairItemAsProperty> Helpers { get; set; }
        public ObservableCollection<PairItem> HelperOptions { get; set; }

        public Func<object, string, bool> Filter { get; set; }

        public PairItemAsProperty SelectedHelper
        {
            get { return selectedHelper; }
            set
            {
                SetProperty(ref selectedHelper, value);
                oldSelectedHelperEmail = selectedHelper != null ? selectedHelper.PairItem.Value : null;
            }
        }

        public PairItemAsProperty NewHelper
        {
            get { return newHelper; }
            set
            {
                SetProperty(ref newHelper, value);
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

        public string CourseCode { get => courseCode; set => SetProperty(ref courseCode, value); }

        public string CourseName { get; set; }

        public RelayCommand RegisterCommand => new RelayCommand(execute => OpenRegister());

        public RelayCommand AddCommand => new RelayCommand(execute => Add(), canExecute => CanAdd());
        public RelayCommand ChangeCommand => new RelayCommand(execute => Update(), canExecute => CanUpadate());
        public RelayCommand DeleteCommand => new RelayCommand(execute => Delete(), canExecute => CanDelete());

        public HelpersPageViewModel(string courseCode, string courseName, string responsible)
        {
            CourseCode = courseCode;
            CourseName = courseName;

            Filter = FilterHelpers;
            IsRegisterVisible = false;

            string url = ConfigurationManager.AppSettings["userFullNameUrl"] + "?email=" + responsible;

            var fullNames = WebAPI.GetCall(url);

            if (fullNames.Result.StatusCode == System.Net.HttpStatusCode.OK)
            {
                HelperOptions = new JavaScriptSerializer()
                    .Deserialize<ObservableCollection<PairItem>>(fullNames.Result.Content.ReadAsStringAsync().Result);
            }

            var courses = WebAPI.GetCall(ConfigurationManager.AppSettings["helperUrl"] + "?courseCode=" + CourseCode);

            if (courses.Result.StatusCode == System.Net.HttpStatusCode.OK)
            {
                Helpers = new JavaScriptSerializer()
                    .Deserialize<ObservableCollection<PairItemAsProperty>>(courses.Result.Content.ReadAsStringAsync().Result);
            }
        }

        private bool FilterHelpers(object obj, string searchText)
        {
            if (obj is PairItemAsProperty helper)
            {
                return helper.PairItem.Display.ToLower().Contains(searchText.ToLower())
                    || helper.PairItem.Value.ToLower().Contains(searchText.ToLower());
            }
            return false;
        }

        private bool CanAdd()
        {
            return NewHelper != null;
        }

        private void Add()
        {
            var serverResponse = WebAPI.PostCall(ConfigurationManager.AppSettings["helperUrl"],
                new
                {
                    CourseCode = this.CourseCode,
                    Helper = NewHelper.PairItem.Value
                });
            if (serverResponse.Result.StatusCode == System.Net.HttpStatusCode.Created)
            {
                Helpers.Add(NewHelper);
                OpenRegister();
                Message = "Успішно додано нового помічника";
            }
            else
            {
                Message = "Сталася помилка додавання";
            }
        }

        private bool CanUpadate()
        {
            return !IsRegisterVisible && SelectedHelper != null;
        }

        private void Update()
        {
            var serverResponse = WebAPI.PutCall(ConfigurationManager.AppSettings["helperUrl"]
                + "?courseCode=" + CourseCode, (oldSelectedHelperEmail, SelectedHelper.PairItem.Value));
            if (serverResponse.Result.StatusCode == System.Net.HttpStatusCode.OK)
            {
                Message = "Успішно оновлено вибраний рядок";
                oldSelectedHelperEmail = SelectedHelper.PairItem.Value;
            }
            else
            {
                Message = "Сталася помилка оновлення вибраного рядка";
            }
        }

        private bool CanDelete()
        {
            return !IsRegisterVisible && SelectedHelper != null;
        }

        private void Delete()
        {
            var serverResponse = WebAPI.DeleteCall(ConfigurationManager.AppSettings["helperUrl"]
               + "?courseCode=" + CourseCode + "&helper=" + SelectedHelper.PairItem.Value);
            if (serverResponse.Result.StatusCode == System.Net.HttpStatusCode.OK)
            {
                Message = "Успішно видалено помічника";
                Helpers.Remove(SelectedHelper);
                SelectedHelper = null;
            }
            else
            {
                Message = "Сталася помилка видалення помічника";
            }
        }

        private void OpenRegister()
        {
            IsRegisterVisible = !IsRegisterVisible;
            NewHelper = IsRegisterVisible ? new PairItemAsProperty() : null;
            SelectedHelper = null;
        }
    }
}
