using System;

namespace DatingApp.API.DTOs
{
    public class MessageFromCreationDTO
    {
        public int SenderId {get;set;}
        public int RecipientId {get;set;}
        public DateTime MessageSent {get;set;}
        public string Content {get;set;}
        public MessageFromCreationDTO()
        {
            MessageSent = DateTime.Now;
        }
    }
}