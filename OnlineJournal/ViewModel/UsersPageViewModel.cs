using OnlineJournal.API;
using OnlineJournal.Model;
using OnlineJournal.MVVM;
using OnlineJournal.Processings;
using System;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Web.Script.Serialization;

namespace OnlineJournal.ViewModel
{
    class UsersPageViewModel : ViewModelBase
    {
        private User selectedUser;
        private UserWithPassword newUser;
        private bool isRegisterVisible;
        private bool isUpdatePasswordVisible;
        private string message;

        private ViewModelBase updatePasswordPage;

        public ObservableCollection<User> Users { get; set; }

        public ObservableCollection<string> RoleOptions { get; set; }

        public Func<object, string, bool> Filter { get; set; }

        public User SelectedUser
        {
            get { return selectedUser; }
            set
            {
                SetProperty(ref selectedUser, value);
            }
        }

        public UserWithPassword NewUser
        {
            get { return newUser; }
            set
            {
                SetProperty(ref newUser, value);
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

        public bool IsUpdatePasswordVisible
        {
            private get { return isUpdatePasswordVisible; }
            set
            {
                SetProperty(ref isUpdatePasswordVisible, value);
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

        public ViewModelBase UpdatePasswordPage
        {
            get { return updatePasswordPage; }
            set
            {
                updatePasswordPage = value;
                OnPropertyChanged();
            }
        }

        public RelayCommand RegisterCommand => new RelayCommand(execute => OpenRegister(), canExecute => CanOpenRegister());
        public RelayCommand UpdatePasswordCommand => new RelayCommand(execute => OpenUpdatePassword(), canExecute => CanOpenUpadatePassword());

        public RelayCommand AddCommand => new RelayCommand(execute => AddUser(), canExecute => CanAddUser());
        public RelayCommand ChangeCommand => new RelayCommand(execute => UpdateUser(), canExecute => CanUpadateUser());
        public RelayCommand DeleteCommand => new RelayCommand(execute => DeleteUser(), canExecute => CanDeleteUser());

        public UsersPageViewModel()
        {
            Filter = FilterUsers;
            IsRegisterVisible = false;
            IsUpdatePasswordVisible = false;

            RoleOptions = new ObservableCollection<string>() { "Викладач", "Аспірант", "Адміністратор" };

            var result = WebAPI.GetCall(ConfigurationManager.AppSettings["userUrl"]);

            if (result.Result.StatusCode == System.Net.HttpStatusCode.OK)
            {
                Users = new JavaScriptSerializer()
                    .Deserialize<ObservableCollection<User>>(result.Result.Content.ReadAsStringAsync().Result);
            }
        }

        private bool FilterUsers(object obj, string searchText)
        {
            if (obj is User user)
            {
                return user.Email.ToLower().Contains(searchText.ToLower()) || user.FullName.ToLower().Contains(searchText.ToLower());
            }
            return false;
        }

        private bool CanAddUser()
        {
            Validation validation = new Validation();
            return NewUser != null && validation.ValidateEmail(NewUser.Email) && validation.ValidateText(NewUser.FullName)
                && validation.ValidatePassword(NewUser.Password) && validation.ValidatePhone(NewUser.Phone) && NewUser.Role > 0;
        }

        private void AddUser()
        {
            var serverResponse = WebAPI.PostCall(ConfigurationManager.AppSettings["userUrl"], NewUser);
            if (serverResponse.Result.StatusCode == System.Net.HttpStatusCode.Created)
            {
                Users.Add(NewUser.GetUserWithoutPassword());
                OpenRegister();
                Message = "Успішно додано нового праціника";
            }
            else
            {
                Message = "Сталася помилка додавання";
            }
        }

        private bool CanUpadateUser()
        {
            Validation validation = new Validation();

            return !IsRegisterVisible && !IsUpdatePasswordVisible && SelectedUser != null
                && validation.ValidateText(SelectedUser.FullName)
                && validation.ValidatePhone(SelectedUser.Phone);
        }

        private void UpdateUser()
        {
            var serverResponse = WebAPI.PutCall(ConfigurationManager.AppSettings["userUrl"]
                + "?userEmail=" + SelectedUser.Email, SelectedUser.GetUserWithBlankPassword());
            if (serverResponse.Result.StatusCode == System.Net.HttpStatusCode.OK)
            {
                Message = "Успішно оновлено вибраний рядок";
            }
            else
            {
                Message = "Сталася помилка оновлення вибраного рядка";
            }
        }

        private bool CanDeleteUser()
        {
            return !IsRegisterVisible && !IsUpdatePasswordVisible && SelectedUser != null;
        }

        private void DeleteUser()
        {
            var serverResponse = WebAPI.DeleteCall(ConfigurationManager.AppSettings["userUrl"]
               + "?userEmail=" + SelectedUser.Email);
            if (serverResponse.Result.StatusCode == System.Net.HttpStatusCode.OK)
            {
                Message = "Успішно видалено робітника";
                Users.Remove(SelectedUser);
                SelectedUser = null;
            }
            else
            {
                Message = "Сталася помилка видалення робітника";
            }
        }

        private void OpenRegister()
        {
            IsRegisterVisible = !IsRegisterVisible;
            NewUser = IsRegisterVisible ? new UserWithPassword() : null;
            SelectedUser = null;
        }

        private bool CanOpenRegister()
        {
            return !IsUpdatePasswordVisible;
        }

        private void OpenUpdatePassword()
        {
            IsUpdatePasswordVisible = !IsUpdatePasswordVisible;
            UpdatePasswordPage = IsUpdatePasswordVisible ? new UpdatePasswordPageViewModel(SelectedUser.Email) : null;
        }

        private bool CanOpenUpadatePassword()
        {
            return SelectedUser != null && !IsRegisterVisible;
        }
    }
}
