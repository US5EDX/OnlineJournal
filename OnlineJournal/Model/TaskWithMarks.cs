using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineJournal.Model
{
    public class TaskWithMarks
    {
        public int Code { get; set; }
        public string Name { get; set; }
        public List<Mark> Marks { get; set; }
    }
}
