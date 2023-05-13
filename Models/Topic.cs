using System;
using System.Collections;
using System.Collections.Generic;

namespace User_Story.Models
{
    public class Topic
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public DateTime CreationDate { get; set; }
        public int UserID { get; set; }
        public User User { get; set; }
        public ICollection<Message> Messages { get; set; }
    }
}
