using OnlineJournal.API;
using OnlineJournal.Model;
using OnlineJournal.MVVM;
using OnlineJournal.Processings;
using System;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Linq;
using System.Web.Script.Serialization;

namespace OnlineJournal.ViewModel
{
    class StudentResultPageViewModel : ViewModelBase
    {
        private StudentCourse selectedCourse;
        private string groupCode;

        public ObservableCollection<StudentCourse> Courses { get; set; }

        public Func<object, string, bool> Filter { get; set; }

        public StudentCourse SelectedCourse
        {
            get { return selectedCourse; }
            set
            {
                SetProperty(ref selectedCourse, value);
            }
        }

        public string StudentFullName { get; set; }

        public GlobalViewModel Global { get; } = GlobalViewModel.Instance;

        public RelayCommand ReturnCommand => new RelayCommand(execute => OpenJournal());

        public StudentResultPageViewModel(string studentCode, string studentFullName, string groupCode)
        {
            this.groupCode = groupCode;
            StudentFullName = studentFullName;

            Filter = FilterCourses;

            var courses = WebAPI.GetCall(ConfigurationManager.AppSettings["studentCoursesUrl"] + "?studentCode=" + studentCode);

            if (courses.Result.StatusCode == System.Net.HttpStatusCode.OK)
            {
                Courses = new JavaScriptSerializer()
                    .Deserialize<ObservableCollection<StudentCourse>>(courses.Result.Content.ReadAsStringAsync().Result);

                foreach (var course in Courses)
                    course.Grade = course.Tasks.Sum(ta => ta.Mark);
            }
        }

        private bool FilterCourses(object obj, string searchText)
        {
            if (obj is StudentCourse course)
            {
                return course.Name.ToLower().Contains(searchText.ToLower());
            }
            return false;
        }

        private void OpenJournal()
        {
            Global.CurrentChildView = new StudentsPageViewModel(groupCode);
        }
    }
}
