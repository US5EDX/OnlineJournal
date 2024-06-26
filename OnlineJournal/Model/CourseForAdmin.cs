namespace OnlineJournal.Model
{
    public class CourseForAdmin
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public PairItem Responsible { get; set; }
        public int AdminCourseRole { get; set; }

        public object GetCourseWithoutLecturerFullName()
        {
            return new { Code = this.Code, Name = this.Name, Responsible = this.Responsible.Value };
        }
    }
}
