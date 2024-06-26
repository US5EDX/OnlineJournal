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
    class CoursesPageViewModel : ViewModelBase
    {
        private Course selectedCourse;
        private Course newCourse;
        private bool isRegisterVisible;
        private bool isFullMenuVisible;
        private bool isUserLecturer;
        private string message;

        public ObservableCollection<Course> Courses { get; set; }

        public Func<object, string, bool> Filter { get; set; }

        public Course SelectedCourse
        {
            get { return selectedCourse; }
            set
            {
                SetProperty(ref selectedCourse, value);
                IsFullMenuVisible = selectedCourse == null ? false : selectedCourse.IsCurrnetUserResponsible;
            }
        }

        public Course NewCourse
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

        public bool IsFullMenuVisible
        {
            private get { return isFullMenuVisible; }
            set
            {
                SetProperty(ref isFullMenuVisible, value);
            }
        }

        public bool IsUserLecturer
        {
            private get { return isUserLecturer; }
            set
            {
                SetProperty(ref isUserLecturer, value);
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

        public CoursesPageViewModel()
        {
            Filter = FilterCourses;
            IsRegisterVisible = false;
            IsUserLecturer = Global.Role == 1;

            GetCourses();
        }

        private void GetCourses()
        {
            string url = ConfigurationManager.AppSettings["courseUrl"] + "?responsible=" + Global.Account.Email;

            var courses = WebAPI.GetCall(url);

            if (courses.Result.StatusCode == System.Net.HttpStatusCode.OK)
            {
                Courses = new JavaScriptSerializer()
                    .Deserialize<ObservableCollection<Course>>(courses.Result.Content.ReadAsStringAsync().Result);
            }
        }

        private bool FilterCourses(object obj, string searchText)
        {
            if (obj is Course course)
            {
                return course.Code.ToLower().Contains(searchText.ToLower()) || course.Name.ToLower().Contains(searchText.ToLower())
                    || course.Responsible.Display.ToLower().Contains(searchText.ToLower());
            }
            return false;
        }

        private bool CanAdd()
        {
            Validation validation = new Validation();
            return NewCourse != null && !string.IsNullOrEmpty(NewCourse.Code) && validation.ValidateText(NewCourse.Name);
        }

        private void Add()
        {
            NewCourse.Responsible = new PairItem() { Value = Global.Account.Email, Display = Global.Account.FullName };
            var serverResponse = WebAPI.PostCall(ConfigurationManager.AppSettings["courseUrl"], NewCourse.GetCourseWithoutLecturerFullName());
            if (serverResponse.Result.StatusCode == System.Net.HttpStatusCode.Created)
            {
                NewCourse.IsCurrnetUserResponsible = true;
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
            NewCourse = IsRegisterVisible ? new Course() : null;
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
            return !IsRegisterVisible && SelectedCourse != null;
        }

        private void OpenJournal()
        {
            Global.CurrentChildView = new JournalPageViewModel(SelectedCourse.Code, SelectedCourse.Name, SelectedCourse.IsCurrnetUserResponsible);
        }
    }
}
