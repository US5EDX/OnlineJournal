using OnlineJournal.API;
using OnlineJournal.Model;
using OnlineJournal.MVVM;
using OnlineJournal.Processings;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web.Script.Serialization;

namespace OnlineJournal.ViewModel
{
    class JournalPageViewModel : ViewModelBase
    {
        private readonly string courseCode;
        private bool isUserResponsible;
        private string message;
        private (int, int, int) updatedCell;

        private DataTable data;

        Dictionary<int, int> tasksId;
        Dictionary<int, string> studentsCode;

        public bool IsUserResponsible
        {
            get { return isUserResponsible; }
            set
            {
                SetProperty(ref isUserResponsible, value);
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

        public (int rowIndex, int columnIndex, int mark) UpdatedCell
        {
            get { return updatedCell; }
            set
            {
                if (value == (0, 0, 0))
                {
                    updatedCell = value;
                    return;
                }

                SetProperty(ref updatedCell, value);
                Update();
            }
        }

        public DataTable Data
        {
            get { return data; }
            set
            {
                SetProperty(ref data, value);
            }
        }

        public string CourseName { get; set; }

        public GlobalViewModel Global { get; } = GlobalViewModel.Instance;

        public RelayCommand OpenTasksCommand => new RelayCommand(execute => OpenTasks());

        public JournalPageViewModel(string courseCode, string courseName, bool isUserResponsible)
        {
            this.courseCode = courseCode;
            CourseName = courseName;
            IsUserResponsible = isUserResponsible;

            List<StudentWithFullName> students = LoadStudents(courseCode);
            List<TaskWithMarks> tasks = LoadTasks(courseCode);

            Data = new DataTable();
            tasksId = new Dictionary<int, int>();
            studentsCode = new Dictionary<int, string>();

            FillDataTable(ref students, ref tasks);
        }

        public string SaveTable(string destinationFilePath)
        {
            var result = new DataTableSaveToCSV().Save(destinationFilePath, ref data);
            return result ? "Успішно" : "Неуспішно";
        }

        public string FromResult(string destinationFilePath, bool isSetoff)
        {
            var resultTable = new FormResultDataTable().FormResult(ref data, isSetoff);
            try
            {
                var lecturersInfo = LoadCourseLecturesrsInfo(courseCode);
                new PDFFroming().FormAcademicRecord(destinationFilePath, ref resultTable, CourseName, lecturersInfo);
                return "Успішно";
            }
            catch
            {
                return "Неуспішно";
            }
        }

        private List<StudentWithFullName> LoadStudents(string courseCode)
        {
            var response = WebAPI.GetCall(ConfigurationManager.AppSettings["studentWithFullNameUrl"] + "?courseCode=" + courseCode);

            if (response.Result.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return new JavaScriptSerializer()
                    .Deserialize<List<StudentWithFullName>>(response.Result.Content.ReadAsStringAsync().Result);
            }

            return new List<StudentWithFullName>();
        }

        private List<TaskWithMarks> LoadTasks(string courseCode)
        {
            var response = WebAPI.GetCall(ConfigurationManager.AppSettings["markUrl"] + "?courseCode=" + courseCode);

            if (response.Result.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return new JavaScriptSerializer()
                    .Deserialize<List<TaskWithMarks>>(response.Result.Content.ReadAsStringAsync().Result);
            }

            return new List<TaskWithMarks>();
        }

        private void FormColumns(ref List<TaskWithMarks> tasks)
        {
            Data.Columns.Add("ПІБ");
            Data.Columns[0].ReadOnly = true;
            Data.Columns[0].DataType = typeof(string);

            foreach (var task in tasks)
            {
                var column = Data.Columns.Add(task.Name);
                column.DataType = typeof(int);

                //tasks id map creation
                tasksId.Add(column.Ordinal, task.Code);
            }
        }

        private void FillDataTable(ref List<StudentWithFullName> students, ref List<TaskWithMarks> tasks)
        {
            FormColumns(ref tasks);

            foreach (var student in students)
            {
                List<object> row = new List<object>();
                row.Add(student.FullName);

                foreach (var task in tasks)
                {
                    var mark = task.Marks.FirstOrDefault(ma => ma.StudentCode == student.Code);
                    if (mark != null)
                        row.Add(mark.Mark1);
                    else
                        row.Add(null);
                }

                Data.Rows.Add(row.ToArray());

                //students code map creation
                studentsCode.Add(Data.Rows.Count - 1, student.Code);
            }
        }

        private void OpenTasks()
        {
            Global.CurrentChildView = new TasksPageViewModel(courseCode, CourseName, IsUserResponsible);
        }

        private void Update()
        {
            Message = "Оновлюється";
            var serverResponse = UpdateCall();

            if (serverResponse.Result.StatusCode == System.Net.HttpStatusCode.OK || serverResponse.Result.StatusCode == System.Net.HttpStatusCode.Created)
            {
                Data.Rows[UpdatedCell.rowIndex][UpdatedCell.columnIndex] = UpdatedCell.mark == -1 ? (object)DBNull.Value : (object)UpdatedCell.mark;
                Message = "Успішно";
            }
            else
            {
                Message = "Сталася помилка оновлення";
            }
        }

        private System.Threading.Tasks.Task<System.Net.Http.HttpResponseMessage> UpdateCall()
        {
            string url = ConfigurationManager.AppSettings["markUrl"]
                + "?studentCode=" + studentsCode[UpdatedCell.rowIndex] + "&taskId=" + tasksId[UpdatedCell.columnIndex];

            if (UpdatedCell.mark == -1)
                return WebAPI.DeleteCall(url);

            if (Data.Rows[UpdatedCell.rowIndex][UpdatedCell.columnIndex] is DBNull)
                return WebAPI.PostCall(url, new
                {
                    StudentCode = studentsCode[UpdatedCell.rowIndex],
                    TaskId = tasksId[UpdatedCell.columnIndex],
                    Mark1 = UpdatedCell.mark
                });

            return WebAPI.PutCall(url, UpdatedCell.mark);
        }

        private CourseLecturers LoadCourseLecturesrsInfo(string courseCode)
        {
            var response = WebAPI.GetCall(ConfigurationManager.AppSettings["courseLecturersInfoUrl"] + "?code=" + courseCode);

            if (response.Result.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return new JavaScriptSerializer()
                    .Deserialize<CourseLecturers>(response.Result.Content.ReadAsStringAsync().Result);
            }

            return null;
        }
    }
}
