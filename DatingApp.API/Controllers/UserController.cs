using System.Data.Common;
using System.Security.Claims;
using System.Diagnostics;
using System.Reflection.Metadata;
using System.Net;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using DatingApp.API.Data;
using DatingApp.API.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using DatingApp.API.Helpers;
using DatingApp.API.Models;

namespace DatingApp.API.Controllers
{
    [Authorize]
    [ServiceFilter(typeof(LogUserActivity))]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IDatingRepository _repo;
        public IMapper _mapper { get; set; }
        public UserController(IDatingRepository repo, IMapper mapper)
        {
            this._mapper = mapper;
            this._repo = repo;
        }
        [HttpGet("GetUsers")]
        public async Task<IActionResult> GetUsers(UserParams userParams)
        {
            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var userFromRepo = await _repo.GetUser(currentUserId);
            userParams.UserId = currentUserId;
            if(string.IsNullOrEmpty(userParams.Gender))
            {
                userParams.Gender = userFromRepo.Gender == "male" ? "female":"male";
            }
            var users = await _repo.GetUsers(userParams);
            
            var usersToReturn = _mapper.Map<IEnumerable<UserForListDTO>>(users);
            Response.AddPagination(users.CurrentPage, users.PageSize, users.TotalCount, users.TotalPages);
            return Ok(usersToReturn);
        }

        [HttpGet("{id}", Name = "GetUser")]
        [Route("getUser/{id}")]
        public async Task<IActionResult> GetUser(int id)
        {
            var user = await _repo.GetUser(id);
            var userToReturn = _mapper.Map<UserForDetailedDTO>(user);
            return Ok(userToReturn);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UserForUpdateDTO userForUpdate)
        {
            if(id != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            return Unauthorized();

            var userfromRepo = await _repo.GetUser(id);
            _mapper.Map(userForUpdate, userfromRepo);
            if(await _repo.SaveAll())
            {
                return NoContent();
            }
            else{
                throw new Exception($"Updating User With Id Failed on Save");
            }
        }

        [HttpPost("{id}/like/{recipientId}")]
        public async Task<IActionResult> LikeUser(int id, int recipientID)
        {
            if(id != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            return Unauthorized();

            var like = await _repo.GetLikes(id, recipientID);

            if(like !=null){
                return BadRequest("You already liked this user");
            }

            if(await _repo.GetUser(recipientID)==null){
                return NotFound();
            }

            like = new API.Models.Like{
                LikerId = id,
                LikeeId = recipientID
            };

            _repo.Add<Like>(like);

            if(await _repo.SaveAll())
            {
                return Ok();
            }
            return BadRequest();
        }
    }
}