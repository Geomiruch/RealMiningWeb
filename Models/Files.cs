using System;

namespace User_Story.Models
{
    public class Files
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public string Path { get; set; }
        public string ModeVesrion { get; set; }
        public string GameVersion { get; set; }
        public DateTime UploadDate { get; set; }
        public int UserID { get; set; }
        public User User { get; set; }

    }
}
