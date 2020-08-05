using System;

namespace DatingApp.Api.MOdels
{
    public class Photo
    {
        public int Id { get; set; }
        public string Url { get; set; }
        public DateTime DateAdded { get; set; }
        public string Description { get; set; }
        public bool IsMainPhoto { get; set; }
        public int UserID { get; set; }
        public User User { get; set; }
    }
}