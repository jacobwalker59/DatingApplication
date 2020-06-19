using System.Data.Common;
using System.Net.Mail;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using DatingApp.API.Data;
using DatingApp.API.DTOs;
using DatingApp.API.Helpers;
using DatingApp.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace DatingApp.API.Controllers {
    [Authorize]
    [Route ("users/{userId}/photos")]
    [ApiController]
    public class PhotoController : ControllerBase {
       
        public IDatingRepository _datingRepo { get; set; }
        public readonly IMapper _mapper;
        private readonly IOptions<CloudinarySettings> _cloudinaryOptions;
        private Cloudinary cloudinary;
        public PhotoController (IDatingRepository datingRepo, IMapper mapper, IOptions<CloudinarySettings> options) {
            
            this._cloudinaryOptions = options;
            this._mapper = mapper;
            this._datingRepo = datingRepo;

            Account account = new Account (
            _cloudinaryOptions.Value.CloudName,
             _cloudinaryOptions.Value.ApiKey,
            _cloudinaryOptions.Value.ApiSecret
            );

            cloudinary = new Cloudinary(account);
        }

        [HttpGet("{id}", Name = "GetPhoto")]
        public async Task<IActionResult> GetPhoto(int id){

            var photoFromRepo = await _datingRepo.GetPhoto(id);
            var photo = Mapper.Map<PhotoForReturnDTO>(photoFromRepo);
            return Ok(photo);
        }

        [HttpPost]
        public async Task<IActionResult> AddPhotoForUser(int userId, [FromForm] PhotoForCreationDTO dto
        ){

            if(userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            return Unauthorized();

            var userFromRepo = await _datingRepo.GetUser(userId);

            var file = dto.File;

            var uploadResult = new ImageUploadResult();
            if(file.Length>0){
                using(var stream = file.OpenReadStream()){

                    var uploadParams = new ImageUploadParams(){
                        File = new FileDescription(file.Name,stream),
                        Transformation = new Transformation().Width(500).Height(500).Crop("fill").Gravity("face")
                    };

                    uploadResult = cloudinary.Upload(uploadParams);
                }
            }
            dto.Url = uploadResult.Uri.ToString();
            dto.PublicId = uploadResult.PublicId;

            var photo = Mapper.Map<Photo>(dto);

           if(!userFromRepo.Photos.Any(u => u.IsMain))
           photo.IsMain = true;

           userFromRepo.Photos.Add(photo);

           if(await _datingRepo.SaveAll()) 
           {
               var photoToReturn = _mapper.Map<PhotoForReturnDTO>(photo);
               return CreatedAtRoute("GetPhoto", new {userId = userId, id = photo.Id}, photoToReturn);
           }
           return BadRequest("Could Not Add The Photo");
        }
        [HttpPost("{id}/setMain")]
        public async Task<IActionResult> SetMain(int userId,int id)
        {
            if(userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            return Unauthorized();

            var userFromRepo = await _datingRepo.GetUser(userId);

            if(!userFromRepo.Photos.Any(predicate=> predicate.Id == id))
            {
                return Unauthorized();
            }

            var photoFromRepo = await _datingRepo.GetPhoto(id);

            if(photoFromRepo.IsMain)
            return BadRequest("This is already the main photo");

            var currentMainPhoto = await _datingRepo.GetMainPhotoForUser(userId);

            currentMainPhoto.IsMain = false;

            photoFromRepo.IsMain = true;

            if( await _datingRepo.SaveAll())
            {
                return NoContent();
            }

            return BadRequest("Could Not Set Photo To Main");

        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePhoto(int userId, int id)
        {
            if(userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            return Unauthorized();

            var userFromRepo = await _datingRepo.GetUser(userId);

            if(!userFromRepo.Photos.Any(p=> p.Id == id))
            {
                return Unauthorized();
            }

            var photoFromRepo = await _datingRepo.GetPhoto(id);

            if(photoFromRepo.IsMain)
            return BadRequest("You cannot delete your main photo");

            if(photoFromRepo.PublicId!=null){

                    var deleteParams = new DeletionParams(photoFromRepo.PublicId);

            var result = cloudinary.Destroy(deleteParams);

            if(result.Result == "ok")
            {
                _datingRepo.Delete(photoFromRepo);
            }

            }

            if(photoFromRepo.PublicId==null)
            {
                _datingRepo.Delete(photoFromRepo);
            }

            if(await _datingRepo.SaveAll())
            {
                return Ok();
            }

            return BadRequest("Failed to delete the photo");

        }
        
}
}