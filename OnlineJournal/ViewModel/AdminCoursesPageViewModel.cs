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
    class AdminCoursesPageViewModel : ViewModelBase
    {
        private CourseForAdmin selectedCourse;
        private CourseForAdmin newCourse;
        private bool isRegisterVisible;
        private string message;

        public ObservableCollection<CourseForAdmin> Courses { get; set; }
        public ObservableCollection<PairItem> LecturerOptions { get; set; }

        public Func<object, string, bool> Filter { get; set; }

        public CourseForAdmin SelectedCourse
        {
            get { return selectedCourse; }
            set
            {
                SetProperty(ref selectedCourse, value);
            }
        }

        public CourseForAdmin NewCourse
        {
            get { return newCourse; }
            set
            {
                SetProperty(ref newCourse, value);
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

        public GlobalViewModel Global { get; } = GlobalViewModel.Instance;

        public RelayCommand RegisterCommand => new RelayCommand(execute => OpenRegister());
        public RelayCommand OpenHelpersCommand => new RelayCommand(execute => OpenHelpers(), canExecute => CanOpenHelpers());
        public RelayCommand OpenStudentsCommand => new RelayCommand(execute => OpenStudents(), canExecute => CanOpenStudents());
        public RelayCommand OpenJournalCommand => new RelayCommand(execute => OpenJournal(), canExecute => CanOpenJournal());

        public RelayCommand AddCommand => new RelayCommand(execute => Add(), canExecute => CanAdd());
        public RelayCommand ChangeCommand => new RelayCommand(execute => Update(), canExecute => CanUpadate());
        public RelayCommand DeleteCommand => new RelayCommand(execute => Delete(), canExecute => CanDelete());

        public AdminCoursesPageViewModel()
        {
            Filter = FilterCourses;
            IsRegisterVisible = false;

            GetLecuterOptions();
            GetCourses();
        }

        private void GetLecuterOptions()
        {
            var fullNames = WebAPI.GetCall(ConfigurationManager.AppSettings["userFullNameUrl"]);

            if (fullNames.Result.StatusCode == System.Net.HttpStatusCode.OK)
            {
                LecturerOptions = new JavaScriptSerializer()
                    .Deserialize<ObservableCollection<PairItem>>(fullNames.Result.Content.ReadAsStringAsync().Result);
            }
        }

        private void GetCourses()
        {
            var courses = WebAPI.GetCall(ConfigurationManager.AppSettings["courseForAdminUrl"] + "?email=" + Global.Account.Email);

            if (courses.Result.StatusCode == System.Net.HttpStatusCode.OK)
            {
                Courses = new JavaScriptSerializer()
                    .Deserialize<ObservableCollection<CourseForAdmin>>(courses.Result.Content.ReadAsStringAsync().Result);
            }
        }

        private bool FilterCourses(object obj, string searchText)
        {
            if (obj is CourseForAdmin course)
            {
                return course.Code.ToLower().Contains(searchText.ToLower()) || course.Name.ToLower().Contains(searchText.ToLower())
                    || course.Responsible.Display.ToLower().Contains(searchText.ToLower());
            }
            return false;
        }

        private bool CanAdd()
        {
            Validation validation = new Validation();
            return NewCourse != null && !string.IsNullOrEmpty(NewCourse.Code) && validation.ValidateText(NewCourse.Name)
                && NewCourse.Responsible != null;
        }

        private void Add()
        {
            var serverResponse = WebAPI.PostCall(ConfigurationManager.AppSettings["courseUrl"], NewCourse.GetCourseWithoutLecturerFullName());
            if (serverResponse.Result.StatusCode == System.Net.HttpStatusCode.Created)
            {
                NewCourse.AdminCourseRole = NewCourse.Responsible.Value == Global.Account.Email ? 1 : 0;
                Courses.Add(NewCourse);
                OpenRegister();
                Message = "Успішно додано новий курс";
            }
            else
            {
                Message = "Сталася помилка додавання";
            }
        }

        private bool CanUpadate()
        {
            return !IsRegisterVisible && SelectedCourse != null;
        }

        private void Update()
        {
            var serverResponse = WebAPI.PutCall(ConfigurationManager.AppSettings["courseUrl"]
                + "?code=" + SelectedCourse.Code, SelectedCourse.GetCourseWithoutLecturerFullName());
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
            return !IsRegisterVisible && SelectedCourse != null;
        }

        private void Delete()
        {
            var serverResponse = WebAPI.DeleteCall(ConfigurationManager.AppSettings["courseUrl"]
               + "?code=" + SelectedCourse.Code);
            if (serverResponse.Result.StatusCode == System.Net.HttpStatusCode.OK)
            {
                Message = "Успішно видалено курс";
                Courses.Remove(SelectedCourse);
                SelectedCourse = null;
            }
            else
            {
                Message = "Сталася помилка видалення курсу";
            }
        }

        private void OpenRegister()
        {
            IsRegisterVisible = !IsRegisterVisible;
            NewCourse = IsRegisterVisible ? new CourseForAdmin() : null;
            SelectedCourse = null;
        }

        private bool CanOpenHelpers()
        {
            return !IsRegisterVisible && SelectedCourse != null;
        }

        private void OpenHelpers()
        {
            Global.CurrentChildView = new HelpersPageViewModel(SelectedCourse.Code, SelectedCourse.Name, SelectedCourse.Responsible.Value);
        }

        private bool CanOpenStudents()
        {
            return !IsRegisterVisible && SelectedCourse != null;
        }

        private void OpenStudents()
        {
            Global.CurrentChildView = new StudentsOnCourseViewModel(SelectedCourse.Code, SelectedCourse.Name);
        }

        private bool CanOpenJournal()
        {
            return !IsRegisterVisible && SelectedCourse != null && SelectedCourse.AdminCourseRole != 0;
        }

        private void OpenJournal()
        {
            if (SelectedCourse.AdminCourseRole == 0)
                return;

            Global.CurrentChildView = new JournalPageViewModel(SelectedCourse.Code, SelectedCourse.Name, SelectedCourse.AdminCourseRole == 1);
        }
    }
}
