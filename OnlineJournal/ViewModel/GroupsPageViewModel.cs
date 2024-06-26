using OnlineJournal.API;
using OnlineJournal.Model;
using OnlineJournal.MVVM;
using System;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Web.Script.Serialization;

namespace OnlineJournal.ViewModel
{
    class GroupsPageViewModel : ViewModelBase
    {
        private Group selectedGroup;
        private Group newGroup;
        private bool isRegisterVisible;
        private string message;

        public ObservableCollection<Group> Groups { get; set; }

        public ObservableCollection<PairItem> CuratorOptions { get; set; }

        public Func<object, string, bool> Filter { get; set; }

        public Group SelectedGroup
        {
            get { return selectedGroup; }
            set
            {
                SetProperty(ref selectedGroup, value);
            }
        }

        public Group NewGroup
        {
            get { return newGroup; }
            set
            {
                SetProperty(ref newGroup, value);
            }
        }

        public bool IsRegisterVisible
        {
            private get { return isRegisterVisible; }
            set
            {
                SetProperty(ref isRegisterVisible, value);
            }
        }

        public string Message
        {
            get { return message; }
            set
            {
                SetProperty(ref message, value);
            }
        }

        public GlobalViewModel Global { get; } = GlobalViewModel.Instance;

        public RelayCommand RegisterCommand => new RelayCommand(execute => OpenRegister());
        public RelayCommand OpenStudentsCommand => new RelayCommand(execute => OpenStudents(), canExecute => CanOpenStudents());

        public RelayCommand AddCommand => new RelayCommand(execute => AddGroup(), canExecute => CanAddGroup());
        public RelayCommand ChangeCommand => new RelayCommand(execute => UpdateGroup(), canExecute => CanUpadateGroup());
        public RelayCommand DeleteCommand => new RelayCommand(execute => DeleteGroup(), canExecute => CanDeleteGroup());

        public GroupsPageViewModel()
        {
            Filter = FilterGroups;
            IsRegisterVisible = false;

            var fullNames = WebAPI.GetCall(ConfigurationManager.AppSettings["userFullNameUrl"]);

            if (fullNames.Result.StatusCode == System.Net.HttpStatusCode.OK)
            {
                CuratorOptions = new JavaScriptSerializer()
                    .Deserialize<ObservableCollection<PairItem>>(fullNames.Result.Content.ReadAsStringAsync().Result);
            }

            var groups = WebAPI.GetCall(ConfigurationManager.AppSettings["groupUrl"]);

            if (groups.Result.StatusCode == System.Net.HttpStatusCode.OK)
            {
                Groups = new JavaScriptSerializer()
                    .Deserialize<ObservableCollection<Group>>(groups.Result.Content.ReadAsStringAsync().Result);
            }
        }

        private bool FilterGroups(object obj, string searchText)
        {
            if (obj is Group group)
            {
                return group.Code.ToLower().Contains(searchText.ToLower()) || group.Curator.Display.ToLower().Contains(searchText.ToLower());
            }
            return false;
        }

        private bool CanAddGroup()
        {
            return NewGroup != null && !string.IsNullOrEmpty(NewGroup.Code) && NewGroup.Curator != null;
        }

        private void AddGroup()
        {
            var serverResponse = WebAPI.PostCall(ConfigurationManager.AppSettings["groupUrl"], NewGroup.GetGroupWithoutFullName());
            if (serverResponse.Result.StatusCode == System.Net.HttpStatusCode.Created)
            {
                Groups.Add(NewGroup);
                OpenRegister();
                Message = "Успішно додано нову групу";
            }
            else
            {
                Message = "Сталася помилка додавання";
            }
        }

        private bool CanUpadateGroup()
        {
            return !IsRegisterVisible && SelectedGroup != null;
        }

        private void UpdateGroup()
        {
            var serverResponse = WebAPI.PutCall(ConfigurationManager.AppSettings["groupUrl"]
                + "?code=" + SelectedGroup.Code, SelectedGroup.Curator.Value);
            if (serverResponse.Result.StatusCode == System.Net.HttpStatusCode.OK)
            {
                Message = "Успішно оновлено вибраний рядок";
            }
            else
            {
                Message = "Сталася помилка оновлення вибраного рядка";
            }
        }

        private bool CanDeleteGroup()
        {
            return !IsRegisterVisible && SelectedGroup != null;
        }

        private void DeleteGroup()
        {
            var serverResponse = WebAPI.DeleteCall(ConfigurationManager.AppSettings["groupUrl"]
               + "?code=" + SelectedGroup.Code);
            if (serverResponse.Result.StatusCode == System.Net.HttpStatusCode.OK)
            {
                Message = "Успішно видалено групу";
                Groups.Remove(SelectedGroup);
                SelectedGroup = null;
            }
            else
            {
                Message = "Сталася помилка видалення групи";
            }
        }

        private void OpenRegister()
        {
            IsRegisterVisible = !IsRegisterVisible;
            NewGroup = IsRegisterVisible ? new Group() : null;
            SelectedGroup = null;
        }

        private bool CanOpenStudents()
        {
            return !IsRegisterVisible && SelectedGroup != null;
        }

        private void OpenStudents()
        {
            Global.CurrentChildView = new StudentsPageViewModel(SelectedGroup.Code);
        }
    }
}
