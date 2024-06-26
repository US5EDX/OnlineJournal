using System.Collections.Generic;

namespace OnlineJournal.Model
{
    public class CourseLecturers
    {
        public string Responsible { get; set; }
        public List<string> Helpers { get; set; }

        public string GetHelpersAsString()
        {
            return string.Join(", ", Helpers);
        }
    }
}
