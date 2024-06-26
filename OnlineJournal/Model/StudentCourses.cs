using System.Collections.Generic;

namespace OnlineJournal.Model
{
    public class StudentCourse
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public int Grade { get; set; }
        public List<TaskWithMark> Tasks { get; set; }
    }
}
