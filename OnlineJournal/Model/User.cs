namespace OnlineJournal.Model
{
    public class User
    {
        public string Email { get; set; }
        public string FullName { get; set; }
        public string Phone { get; set; }
        public int Role { get; set; }

        public UserWithPassword GetUserWithBlankPassword()
        {
            return new UserWithPassword() { Email = this.Email, FullName = this.FullName, Phone = this.Phone, Role = this.Role, Password = null };
        }
    }
}
