namespace OnlineJournal.Model
{
    public class UserWithPassword
    {
        public string Email { get; set; }
        public string FullName { get; set; }
        public string Password { get; set; }
        public string Phone { get; set; }
        public int Role { get; set; }

        public User GetUserWithoutPassword()
        {
            return new User() { Email = this.Email, FullName = this.FullName, Phone = this.Phone, Role = this.Role };
        }
    }
}
