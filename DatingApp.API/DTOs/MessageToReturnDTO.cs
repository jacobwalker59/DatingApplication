using System;
using DatingApp.API.Models;

namespace DatingApp.API.DTOs
{
    public class MessageToReturnDTO
    {
        public int Id { get; set; }
        public int SenderId { get; set; }
        public string SenderKnownAs {get;set;}
        public string SenderPhotoURL { get; set; }
        public int RecipientId { get; set; }
        public string RecipientKnownAs {get;set;}
        public string RecipientPhotoURL { get; set; }
        public string content {get;set;}
        public bool IsRead {get;set;}
        public DateTime DateRead {get;set;}
        public DateTime MessageSent {get;set;}
    }
}