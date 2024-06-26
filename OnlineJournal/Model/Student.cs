namespace OnlineJournal.Model
{
    public class Student
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Patronymic { get; set; }

        public object GetStudentWithGroupCode(string groupCode)
        {
            return new { Code = this.Code, Name = this.Name, Surname = this.Surname, Patronymic = this.Patronymic, Class = groupCode };
        }
    }
}
