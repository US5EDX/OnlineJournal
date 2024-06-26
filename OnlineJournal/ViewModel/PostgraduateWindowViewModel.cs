using OnlineJournal.MVVM;
using System;

namespace OnlineJournal.ViewModel
{
    class PostgraduateWindowViewModel : ViewModelBase
    {
        public string Email { get; set; }

        public bool IsTokenUsed { get; set; }

        public GlobalViewModel Global { get; } = GlobalViewModel.Instance;

        public RelayCommand ShowHomePageCommand => new RelayCommand(execute => { Global.CurrentChildView = new HomePageViewModel(IsTokenUsed); });
        public RelayCommand ShowGroupPageCommand => new RelayCommand(execute => { Global.CurrentChildView = new StudentsPageViewModel(Global.CuratorGroup); });
        public RelayCommand ShowCoursesPageCommand => new RelayCommand(execute => { Global.CurrentChildView = new CoursesPageViewModel(); });

        public PostgraduateWindowViewModel(string email, bool isTokenUsed)
        {
            Email = email;
            IsTokenUsed = isTokenUsed;
            Global.DeifneUserInfo(Email);
            ShowHomePageCommand.Execute(null);
        }
    }
}
