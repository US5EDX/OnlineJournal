using OnlineJournal.MVVM;
using OnlineJournal.ViewModel;

namespace OnlineJournal.Model
{
    public class StudentForCourseSubscribing : PropertyChangedRealization
    {
        private bool isCheked;
        private bool isDefined;

        public string Code { get; set; }
        public string FullName { get; set; }
        public bool IsCheked
        {
            get => isCheked;
            set
            {
                if (isCheked == value)
                {
                    isDefined = true;
                    return;
                }

                if (!isDefined)
                {
                    isCheked = value;
                    isDefined = true;
                    return;
                }

                isCheked = value;
                StudentsOnCourseViewModel.UpdateSelection(this);
                OnPropertyChanged();
            }
        }
    }
}
