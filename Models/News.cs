using System;

namespace User_Story.Models
{
    public class News
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }
        public string Topic { get; set; }
        public DateTime CreationDate { get; set; }
        public int UserID { get; set; }
        public User User { get; set; }
    }
}
