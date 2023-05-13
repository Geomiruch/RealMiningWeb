using System;

namespace User_Story.Models
{
    public class Message
    {
        public int ID { get; set; }
        public string MessageText { get; set; }
        public DateTime CreationDate { get; set; }
        public int TopicID { get; set; }
        public Topic Topic { get; set; }
        public int UserID { get; set; }
        public User User { get; set; }
    }
}
