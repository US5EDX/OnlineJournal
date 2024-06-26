using System.Collections.Generic;

namespace OnlineJournal.Model
{
    public class GroupWithStudents
    {
        public string Code { get; set; }
        public List<StudentForCourseSubscribing> Students { get; set; }
    }
}
