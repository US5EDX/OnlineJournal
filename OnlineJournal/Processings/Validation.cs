using System.Text.RegularExpressions;

namespace OnlineJournal.Processings
{
    public class Validation
    {
        public bool ValidateText(string text)
        {
            return text != null && Regex.IsMatch(text, @"[А-Яа-яЁёІіЇїЄє']{3,}");
        }

        public bool ValidatePhone(string phone)
        {
            return phone != null && Regex.IsMatch(phone, @"^\+380\d{3}\d{2}\d{2}\d{2}$");
        }

        public bool ValidateEmail(string email)
        {
            return email != null && Regex.IsMatch(email, @"^((([0-9A-Za-z]{1}[-0-9A-z\.]{1,}[0-9A-Za-z]{1})|([0-9А-Яа-я]{1}[-0-9А-я\.]{1,}[0-9А-Яа-я]{1}))@([-A-Za-z]{1,}\.){1,2}[-A-Za-z]{2,})$");
        }

        public bool ValidatePassword(string password)
        {
            return password != null && Regex.IsMatch(password, @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)[a-zA-Z\d]{8,}$");
        }
    }
}
