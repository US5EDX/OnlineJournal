using OnlineJournal.API;
using OnlineJournal.MVVM;
using OnlineJournal.Processings.FileProcessing;
using OnlineJournal.Servises;
using OnlineJournal.View;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Web.Script.Serialization;
using System.Windows;

namespace OnlineJournal.ViewModel
{
    class LogInWindowViewModel : ViewModelBase
    {
        private bool isVisible;
        private Visibility isUserUnlogged;

        public string Email { get; set; }

        public string Password { private get; set; }

        public bool IsChecked { private get; set; }

        public bool IsVisible
        {
            private get { return isVisible; }
            set
            {
                SetProperty(ref isVisible, value);
            }
        }

        public Visibility IsUserUnlogged
        {
            private get { return isUserUnlogged; }
            set
            {
                SetProperty(ref isUserUnlogged, value);
            }
        }

        public RelayCommand LogInCommand => new RelayCommand(execute => LogIn(), canExecute => CanLogIn());

        public LogInWindowViewModel()
        {
            IsUserUnlogged = Visibility.Visible;
            IsVisible = false;
            LogInWithToken();
        }

        private void LogInWithToken()
        {
            TokenFileActing tokenFileActing = new TokenFileActing();

            string token = tokenFileActing.GetToken();

            if (token == null)
                return;

            string url = ConfigurationManager.AppSettings["authUrl"] + $"?token={token}";

            var result = WebAPI.GetCall(url);

            if (result.Result.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var res = new JavaScriptSerializer().Deserialize<(string, int)>(result.Result.Content.ReadAsStringAsync().Result);

                OpenMainWindow(res.Item1, res.Item2, true);
            }
            else
            {
                tokenFileActing.DeleteTokenFile();
            }
        }

        private void LogIn()
        {
            string url = ConfigurationManager.AppSettings["authUrl"] + $"?email={Email}&password={Password}&IsNeedToken={IsChecked}";

            var result = WebAPI.GetCall(url);

            if (result.Result.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var res = new JavaScriptSerializer().Deserialize<List<object>>(result.Result.Content.ReadAsStringAsync().Result);

                if (res.Count > 1)
                    new TokenFileActing().SaveToken((string)res[1]);

                OpenMainWindow(Email, (int)res[0], res.Count > 1);
            }
            else
            {
                IsVisible = true;
            }
        }

        private bool CanLogIn()
        {
            return !string.IsNullOrEmpty(Email) && !string.IsNullOrEmpty(Password);
        }

        private void OpenMainWindow(string email, int role, bool isTokenUsed = false)
        {
            GlobalViewModel.Instance.Role = role;

            IsUserUnlogged = Visibility.Collapsed;

            var result = ChooseWindowToOpen(role, email, isTokenUsed);

            if (result.Item1 == null)
                Application.Current.Shutdown();

            GlobalViewModel.Instance.Role = role;

            IWindowService<ViewModelBase> windowService = new WindowService<ViewModelBase>();
            windowService.OpenWindowShutdownOnClosing(result.Item2, result.Item1);
        }

        private (ViewModelBase, Type) ChooseWindowToOpen(int role, string email, bool isTokenUsed)
        {
            switch (role)
            {
                case 1:
                    return (new LecturerWindowViewModel(email, isTokenUsed), typeof(LecturerWindow));
                case 2:
                    return (new PostgraduateWindowViewModel(email, isTokenUsed), typeof(PostgraduateStudentWindow));
                case 3:
                    return (new AdminWindowViewModel(email, isTokenUsed), typeof(AdminWindow));
                default:
                    return (null, null);
            }
        }
    }
}
