using OnlineJournal.API;
using OnlineJournal.Model;
using OnlineJournal.MVVM;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Web.Script.Serialization;

namespace OnlineJournal.ViewModel
{
    class StudentsOnCourseViewModel : ViewModelBase
    {
        private GroupWithStudents selectedGroup;
        private bool isAllChecked;
        private string message;
        private readonly string courseCode;
        private string courseName;
        private static Dictionary<string, bool> toUpdate;

        public ObservableCollection<GroupWithStudents> StudentsOnCourse { get; set; }

        public Func<object, string, bool> Filter { get; set; }

        public GroupWithStudents SelectedGroup
        {
            get { return selectedGroup; }
            set
            {
                SetProperty(ref selectedGroup, value);
                isAllChecked = false;
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

        public string CourseName { get => courseName; set => SetProperty(ref courseName, value); }

        public RelayCommand UpdateCommand => new RelayCommand(execute => Update(), canExecute => CanUpadate());
        public RelayCommand CheckCommand => new RelayCommand(execute => Check(), canExecute => CanCheck());

        public StudentsOnCourseViewModel(string courseCode, string courseName)
        {
            toUpdate = new Dictionary<string, bool>();
            this.courseCode = courseCode;
            CourseName = courseName;

            Filter = FilterGroups;

            var groups = WebAPI.GetCall(ConfigurationManager.AppSettings["groupWithStudentsUrl"] + "?courseCode=" + courseCode);

            if (groups.Result.StatusCode == System.Net.HttpStatusCode.OK)
            {
                StudentsOnCourse = new JavaScriptSerializer()
                    .Deserialize<ObservableCollection<GroupWithStudents>>(groups.Result.Content.ReadAsStringAsync().Result);
            }
        }

        public static void UpdateSelection(StudentForCourseSubscribing student)
        {
            if (toUpdate.ContainsKey(student.Code))
            {
                toUpdate.Remove(student.Code);
            }
            else
            {
                toUpdate.Add(student.Code, student.IsCheked);
            }
        }

        private bool FilterGroups(object obj, string searchText)
        {
            if (obj is GroupWithStudents group)
            {
                return group.Code.ToLower().Contains(searchText.ToLower());
            }
            return false;
        }

        private bool CanUpadate()
        {
            return SelectedGroup != null;
        }

        private void Update()
        {
            var serverResponse = WebAPI.PostCall(ConfigurationManager.AppSettings["courseSubscribeUrl"]
                + "?courseCode=" + courseCode, toUpdate);
            if (serverResponse.Result.StatusCode == System.Net.HttpStatusCode.OK)
            {
                SelectedGroup = null;
                toUpdate = new Dictionary<string, bool>();
                Message = "Успішно оновлено вибраний рядок";
            }
            else
            {
                Message = "Сталася помилка оновлення вибраного рядка";
            }
        }

        private void Check()
        {
            isAllChecked = !isAllChecked;

            foreach (var student in SelectedGroup.Students)
                student.IsCheked = isAllChecked;
        }

        private bool CanCheck()
        {
            return SelectedGroup != null;
        }
    }
}
