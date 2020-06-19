using System;

namespace DatingApp.API.DTOs
{
    public class UserForListDTO
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Gender { get;set;}
        public int Age {get;set;}
        public string KnownAs {get;set;}
        public DateTime CreatedProfile {get;set;}
        public DateTime LastOnline {get;set;}
        public string City {get;set;}
        public string Country {get;set;}
        public string MainPhoto {get;set;}
    }
}