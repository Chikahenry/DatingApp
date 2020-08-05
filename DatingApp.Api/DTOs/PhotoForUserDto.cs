using System;


namespace DatingApp.Api.DTOs
{
    public class PhotoForUserDto
    {
        public int Id { get; set; }
        public string Url { get; set; }
        public DateTime DateAdded { get; set; }
        public string Description { get; set; }
        public bool IsMainPhoto { get; set; }
    }
}