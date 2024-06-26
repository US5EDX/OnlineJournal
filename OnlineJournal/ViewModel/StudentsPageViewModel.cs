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
    class StudentsPageViewModel : ViewModelBase
    {
        private Student selectedStudent;
        private Student newStudent;
        private bool isRegisterVisible;
        private string message;
        private string groupCode;

        public ObservableCollection<Student> Students { get; set; }

        public Func<object, string, bool> Filter { get; set; }

        public Student SelectedStudent
        {
            get { return selectedStudent; }
            set
            {
                SetProperty(ref selectedStudent, value);
            }
        }

        public Student NewStudent
        {
            get { return newStudent; }
            set
            {
                SetProperty(ref newStudent, value);
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

        public string GroupCode { get => groupCode; set => SetProperty(ref groupCode, value); }

        public GlobalViewModel Global { get; } = GlobalViewModel.Instance;

        public RelayCommand RegisterCommand => new RelayCommand(execute => OpenRegister());
        public RelayCommand OpenCommand => new RelayCommand(execute => OpenStudentResult(), canExecute => CanOpenStudentResult());

        public RelayCommand AddCommand => new RelayCommand(execute => Add(), canExecute => CanAdd());
        public RelayCommand ChangeCommand => new RelayCommand(execute => Update(), canExecute => CanUpadate());
        public RelayCommand DeleteCommand => new RelayCommand(execute => Delete(), canExecute => CanDelete());

        public StudentsPageViewModel(string groupCode)
        {
            GroupCode = groupCode;

            Filter = FilterStudents;
            IsRegisterVisible = false;

            var groups = WebAPI.GetCall(ConfigurationManager.AppSettings["studentUrl"] + "?groupCode=" + GroupCode);

            if (groups.Result.StatusCode == System.Net.HttpStatusCode.OK)
            {
                Students = new JavaScriptSerializer()
                    .Deserialize<ObservableCollection<Student>>(groups.Result.Content.ReadAsStringAsync().Result);
            }
        }

        private bool FilterStudents(object obj, string searchText)
        {
            if (obj is Student student)
            {
                return student.Code.ToLower().Contains(searchText.ToLower()) || student.Name.ToLower().Contains(searchText.ToLower())
                    || student.Surname.ToLower().Contains(searchText.ToLower()) || student.Patronymic.ToLower().Contains(searchText.ToLower());
            }
            return false;
        }

        private bool CanAdd()
        {
            Validation validation = new Validation();
            return NewStudent != null && !string.IsNullOrEmpty(NewStudent.Code) && validation.ValidateText(NewStudent.Name)
                && validation.ValidateText(NewStudent.Surname) && validation.ValidateText(NewStudent.Patronymic);
        }

        private void Add()
        {
            var serverResponse = WebAPI.PostCall(ConfigurationManager.AppSettings["studentUrl"], NewStudent.GetStudentWithGroupCode(GroupCode));
            if (serverResponse.Result.StatusCode == System.Net.HttpStatusCode.Created)
            {
                Students.Add(NewStudent);
                OpenRegister();
                Message = "Успішно додано нового студентаі";
            }
            else
            {
                Message = "Сталася помилка додавання";
            }
        }

        private bool CanUpadate()
        {
            return !IsRegisterVisible && SelectedStudent != null;
        }

        private void Update()
        {
            var serverResponse = WebAPI.PutCall(ConfigurationManager.AppSettings["studentUrl"]
                + "?code=" + SelectedStudent.Code, SelectedStudent);
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
            return !IsRegisterVisible && SelectedStudent != null;
        }

        private void Delete()
        {
            var serverResponse = WebAPI.DeleteCall(ConfigurationManager.AppSettings["studentUrl"]
               + "?code=" + SelectedStudent.Code);
            if (serverResponse.Result.StatusCode == System.Net.HttpStatusCode.OK)
            {
                Message = "Успішно видалено студента";
                Students.Remove(SelectedStudent);
                SelectedStudent = null;
            }
            else
            {
                Message = "Сталася помилка видалення групи";
            }
        }

        private void OpenRegister()
        {
            IsRegisterVisible = !IsRegisterVisible;
            NewStudent = IsRegisterVisible ? new Student() : null;
            SelectedStudent = null;
        }

        private bool CanOpenStudentResult()
        {
            return !IsRegisterVisible && SelectedStudent != null;
        }

        private void OpenStudentResult()
        {
            string studentFullName = SelectedStudent.Name + ' ' + SelectedStudent.Surname + ' ' + SelectedStudent.Patronymic;
            Global.CurrentChildView = new StudentResultPageViewModel(SelectedStudent.Code, studentFullName, GroupCode);
        }
    }
}
