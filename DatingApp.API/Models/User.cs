using System.Collections;
using System;
using System.Collections.Generic;

namespace DatingApp.API.Models
{
    public class User
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public byte[] PasswordHash {get;set;}
        public byte [] PasswordSalt {get;set;}
        public string Gender { get;set;}
        public DateTime DateOfBirth {get;set;}
        public string KnownAs {get;set;}
        public DateTime CreatedProfile {get;set;}
        public DateTime LastOnline {get;set;}
        public string Introduction { get;set;}
        public string LookingFor { get;set;}
        public string Interests { get;set;}
        public string City {get;set;}
        public string Country {get;set;}
        public ICollection<Photo> Photos{get;set;}
        public ICollection<Like> Likers {get;set;}
        //  who have liked you
        public ICollection<Like> Likees {get;set;}
        // who have been liked by you
        public ICollection<Message> MessagesSent {get;set;}
        public ICollection<Message> MessagesReceived {get;set;}


    }
}