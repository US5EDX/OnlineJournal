using OnlineJournal.API;
using OnlineJournal.Model;
using OnlineJournal.MVVM;
using System.Configuration;
using System.Web.Script.Serialization;

namespace OnlineJournal.ViewModel
{
    public class GlobalViewModel : ViewModelBase
    {
        public static GlobalViewModel Instance { get; }

        static GlobalViewModel()
        {
            Instance = new GlobalViewModel();
        }

        private ViewModelBase currentChildView;

        public ViewModelBase CurrentChildView
        {
            get { return currentChildView; }
            set
            {
                currentChildView = value;
                OnPropertyChanged();
            }
        }

        private User account;

        public User Account
        {
            get { return account; }
            set
            {
                SetProperty(ref account, value);
            }
        }

        private int role;

        public int Role
        {
            get { return role; }
            set
            {
                SetProperty(ref role, value);
            }
        }

        private string curatotGroup;

        public string CuratorGroup
        {
            get { return curatotGroup; }
            set
            {
                SetProperty(ref curatotGroup, value);
            }
        }

        public void DeifneUserInfo(string email)
        {
            GetUserInfo(email);

            if (Role != 3)
                CheckCurator();
        }

        private void GetUserInfo(string email)
        {
            string url = ConfigurationManager.AppSettings["userUrl"] + "?email=" + email;

            var result = WebAPI.GetCall(url);

            if (result.Result.StatusCode == System.Net.HttpStatusCode.OK)
            {
                Account = new JavaScriptSerializer()
                    .Deserialize<User>(result.Result.Content.ReadAsStringAsync().Result);
            }
        }

        private void CheckCurator()
        {
            string url = ConfigurationManager.AppSettings["groupCuratorUrl"] + "?curator=" + Account.Email;

            var result = WebAPI.GetCall(url);

            if (result.Result.StatusCode == System.Net.HttpStatusCode.OK)
            {
                CuratorGroup = new JavaScriptSerializer()
                    .Deserialize<string>(result.Result.Content.ReadAsStringAsync().Result);
            }
        }
    }
}
