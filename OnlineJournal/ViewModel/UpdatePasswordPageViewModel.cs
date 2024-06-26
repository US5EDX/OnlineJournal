using OnlineJournal.API;
using OnlineJournal.MVVM;
using System.Configuration;
using System.Windows;

namespace OnlineJournal.ViewModel
{
    class UpdatePasswordPageViewModel : ViewModelBase
    {
        private string message;
        private string oldPassword;
        private string newPassword;
        private string newPasswordRepeat;

        public string Email { get; set; }

        public string Message
        {
            get { return message; }
            set
            {
                SetProperty(ref message, value);
            }
        }

        public string OldPassword
        {
            get { return oldPassword; }
            set
            {
                SetProperty(ref oldPassword, value);
            }
        }

        public string NewPassword
        {
            get { return newPassword; }
            set
            {
                SetProperty(ref newPassword, value);
            }
        }

        public string NewPasswordRepeat
        {
            get { return newPasswordRepeat; }
            set
            {
                SetProperty(ref newPasswordRepeat, value);
            }
        }

        public RelayCommand ChangePasswordCommand => new RelayCommand(exectue => UpdateUserPassword(), canExecute => CanUpadateUserPassword());

        public UpdatePasswordPageViewModel(string email)
        {
            Email = email;
        }

        private bool CanUpadateUserPassword()
        {
            return !string.IsNullOrEmpty(OldPassword) && !string.IsNullOrEmpty(NewPassword)
                && !string.IsNullOrEmpty(NewPasswordRepeat) && new Processings.Validation().ValidatePassword(NewPassword)
                && NewPassword.Equals(NewPasswordRepeat);
        }

        private void UpdateUserPassword()
        {
            var serverResponse = WebAPI.PutCall(ConfigurationManager.AppSettings["userUrl"]
                + "?userEmail=" + Email + "&oldPassword=" + OldPassword, NewPassword);
            if (serverResponse.Result.StatusCode == System.Net.HttpStatusCode.OK)
            {
                Message = "Пароль успішно оновлено";
            }
            else
            {
                Message = "Сталася помилка оновлення паролю";
            }
        }
    }
}
