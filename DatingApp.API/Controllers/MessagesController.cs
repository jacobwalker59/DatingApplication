using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using DatingApp.API.Data;
using DatingApp.API.DTOs;
using DatingApp.API.Helpers;
using DatingApp.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DatingApp.API.Controllers {
    [Authorize]
    [ServiceFilter (typeof (LogUserActivity))]
    [Route ("user/{userId}/[controller]")]
    public class MessagesController : ControllerBase {
        private readonly IDatingRepository repo;
        private readonly IMapper mapper;
        public MessagesController (IDatingRepository repo, IMapper mapper) {
            this.mapper = mapper;
            this.repo = repo;
        }

        [HttpGet("{id}", Name = "GetMessage")]
        public async Task<IActionResult> GetMessage(int userId, int id){
            
            if(userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            return Unauthorized();

            var messageFromRepo = await repo.GetMessage(id);

            if(messageFromRepo == null)
            {
                return NotFound();
            }

            return Ok(messageFromRepo);

        }

        [HttpGet]
        public async Task<IActionResult> GetMessagesForUser(int userId, [FromQuery] MessageParams messageParams)
        {
            if(userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            return Unauthorized();

            messageParams.UserId = userId;

            var messagesFromRepo = await repo.GetMessagesForUser(messageParams);

            var messages = Mapper.Map<IEnumerable<MessageToReturnDTO>>(messagesFromRepo);

            Response.AddPagination(messagesFromRepo.CurrentPage, messagesFromRepo.PageSize, messagesFromRepo.TotalCount, messagesFromRepo.TotalPages);

            return Ok(messages);
        }

        [HttpGet("thread/{recipientId}")]
        public async Task<IActionResult> GetMessagesThread(int userId, int recipientId)
        {
            if(userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            return Unauthorized();

            var messageFromRepo = await repo.GetMessageThread(userId, recipientId);

            var messageThread = mapper.Map<IEnumerable<MessageToReturnDTO>>(messageFromRepo);

            return Ok(messageThread);
        }

        [HttpPost]
        public async Task<IActionResult> CreateMessage(int userId,[FromBody] MessageFromCreationDTO messageFromCreation){
            
            var sender = await repo.GetUser(userId);

            if(sender.Id != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            return Unauthorized();

            messageFromCreation.SenderId = userId;

            var recipient  = await repo.GetUser(messageFromCreation.RecipientId);

            if(recipient == null)
            {
                return BadRequest("Could not find user");
            }

            var message = mapper.Map<Message>(messageFromCreation);

            //message from creation turned into message

            repo.Add(message);

            if(await repo.SaveAll())
            {
                var messageToReturn = mapper.Map<MessageToReturnDTO>(message);
                return CreatedAtRoute("GetMessage", new {userId, id = message.Id}, messageToReturn);
            }

            throw new Exception("Creating the message failed to save");

        }

        [HttpPost("{id}")]
        public async Task<IActionResult> DeleteMessage(int id, int userId){
           
            if(userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            return Unauthorized();

            var messageFromRepo = await repo.GetMessage(id);

            if(messageFromRepo.SenderId == userId){
                messageFromRepo.SenderDeleted = true;
            }

            if(messageFromRepo.RecipientId == userId){
                messageFromRepo.RecipientDeleted = true;
            }

            if(messageFromRepo.SenderDeleted && messageFromRepo.RecipientDeleted){
               repo.Delete(messageFromRepo);
            }

            if(await repo.SaveAll())
            {
                return NoContent();
            }

            throw new Exception ("Error deleting the message");

        }

        [HttpPost("{id}/read")]
         public async Task<IActionResult> MarkMessagesAsRead(int id, int userId){
            
            if(userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            return Unauthorized();

            var message = await repo.GetMessage(id);

            if(message.RecipientId !=userId){
                return Unauthorized();
            }

            message.IsRead = true;
            message.DateRead = DateTime.Now;

            await repo.SaveAll();

            return NoContent();

         }
        
    }
}