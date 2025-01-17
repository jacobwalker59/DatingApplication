using System;
using System.Collections.Generic;
using DatingApp.API.Models;

namespace DatingApp.API.DTOs
{
    public class UserForDetailedDTO
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Gender { get;set;}
        public int Age {get;set;}
        public string KnownAs {get;set;}
        public DateTime CreatedProfile {get;set;} 
        public DateTime LastOnline {get;set;} 
        public string Introduction { get;set;}
        public string LookingFor { get;set;}
        public string Interests { get;set;}
        public string City {get;set;}
        public string Country {get;set;}
        public string PhotoURL {get;set;}
        public ICollection<PhotosForDetailedDTO> Photos { get; set; }
    }
}