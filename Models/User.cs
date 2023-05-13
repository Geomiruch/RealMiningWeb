using System.Collections.Generic;

namespace User_Story.Models
{
    public class User
    {
        public int ID { get; set; } 
        public string Login { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Avatar { get; set; }
        public int RoleID { get; set; }
        public Role Role { get; set; }
        public string FullName { get => FirstName + " " + LastName; }

        public ICollection<Topic> Topics { get; set; }
        public ICollection<Files> Files { get; set; }
        public ICollection<Message> Messages { get; set; }
        public ICollection<News> News { get; set; }

    }
}
