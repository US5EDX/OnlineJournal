using System;

namespace OnlineJournal.Model
{
    public class Log
    {
        public DateTime ActionDateTime { get; set; }
        public string UserEmail { get; set; }
        public string ActionType { get; set; }
        public string Description { get; set; }
        public string Changes { get; set; }
    }
}
