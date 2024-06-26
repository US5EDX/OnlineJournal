using OnlineJournal.API;
using OnlineJournal.Model;
using OnlineJournal.MVVM;
using OnlineJournal.Processings.FileProcessing;
using System;
using System.Configuration;
using System.Web.Script.Serialization;

namespace OnlineJournal.ViewModel
{
    class HomePageViewModel : ViewModelBase
    {
        private bool isUpdatePasswordVisible;
        private bool isTokenExist;
        private bool isLogoutVisible;

        private ViewModelBase updatePasswordPage;

        public string HelloMessage { get; private set; }

        public bool IsUpdatePasswordVisible
        {
            private get { return isUpdatePasswordVisible; }
            set
            {
                SetProperty(ref isUpdatePasswordVisible, value);
            }
        }

        public bool IsLogoutVisible
        {
            private get { return isLogoutVisible; }
            set
            {
                SetProperty(ref isLogoutVisible, value);
            }
        }

        public ViewModelBase UpdatePasswordPage
        {
            get { return updatePasswordPage; }
            set
            {
                updatePasswordPage = value;
                OnPropertyChanged();
            }
        }

        public GlobalViewModel Global { get; } = GlobalViewModel.Instance;

        public RelayCommand UpdatePasswordCommand => new RelayCommand(execute => OpenUpdatePassword());
        public RelayCommand LogoutCommand => new RelayCommand(execute => Logout());

        public HomePageViewModel(bool isTokenUsed)
        {
            this.isTokenExist = isTokenUsed;
            ChangeogoutVisiblity();

            HelloMessage = "Добрий день, " + Global.Account.FullName + "!";
        }

        private void OpenUpdatePassword()
        {
            IsUpdatePasswordVisible = !IsUpdatePasswordVisible;
            ChangeogoutVisiblity();
            UpdatePasswordPage = IsUpdatePasswordVisible ? new UpdatePasswordPageViewModel(Global.Account.Email) : null;
        }

        private void Logout()
        {
            new TokenFileActing().DeleteTokenFile();
            isTokenExist = false;
            ChangeogoutVisiblity();
        }

        private void ChangeogoutVisiblity()
        {
            IsLogoutVisible = !IsUpdatePasswordVisible && isTokenExist;
        }
    }
}
