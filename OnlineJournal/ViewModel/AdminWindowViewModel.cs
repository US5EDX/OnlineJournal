using OnlineJournal.MVVM;

namespace OnlineJournal.ViewModel
{
    class AdminWindowViewModel : ViewModelBase
    {
        public string Email { get; set; }

        public bool IsTokenUsed { get; set; }

        public GlobalViewModel Global { get; } = GlobalViewModel.Instance;

        public RelayCommand ShowHomePageCommand => new RelayCommand(execute => { Global.CurrentChildView = new HomePageViewModel(IsTokenUsed); });
        public RelayCommand ShowUsersPageCommand => new RelayCommand(execute => { Global.CurrentChildView = new UsersPageViewModel(); });
        public RelayCommand ShowGroupsPageCommand => new RelayCommand(execute => { Global.CurrentChildView = new GroupsPageViewModel(); });
        public RelayCommand ShowCoursesPageCommand => new RelayCommand(execute => { Global.CurrentChildView = new AdminCoursesPageViewModel(); });
        public RelayCommand ShowServicePageCommand => new RelayCommand(execute => { Global.CurrentChildView = new ServicePageViewModel(); });
        public RelayCommand ShowLogsPageCommand => new RelayCommand(execute => { Global.CurrentChildView = new LogsPageViewModel(); });

        public AdminWindowViewModel(string email, bool isTokenUsed)
        {
            Email = email;
            IsTokenUsed = isTokenUsed;
            Global.DeifneUserInfo(Email);
            ShowHomePageCommand.Execute(null);
        }
    }
}
