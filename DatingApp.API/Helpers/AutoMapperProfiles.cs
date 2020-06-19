using System.Linq;
using AutoMapper;
using DatingApp.API.DTOs;
using DatingApp.API.Models;

namespace DatingApp.API.Helpers
{
    public class AutoMapperProfiles: Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<User,UserForListDTO>().ForMember(dest => 
                dest.MainPhoto, opt => 
                opt.MapFrom(src => src.Photos.FirstOrDefault(p => p.IsMain).Url))
                .ForMember(dest => dest.Age, opt => opt.MapFrom(src => src.DateOfBirth.CalculateAge()));

            CreateMap<User,UserForDetailedDTO>().ForMember(dest => 
                dest.PhotoURL, opt => 
                opt.MapFrom(src => src.Photos.FirstOrDefault(p => p.IsMain).Url))
                .ForMember(dest => dest.Age, opt => opt.MapFrom(src => src.DateOfBirth.CalculateAge()));
            CreateMap<Photo, PhotosForDetailedDTO>();

            CreateMap<UserForUpdateDTO,User>();

            CreateMap<Photo, PhotoForReturnDTO>();
            //first one is what goes in, second one is what is returned.

            CreateMap<PhotoForCreationDTO, Photo>();

            CreateMap<UserForRegisterDTO, User>();

            CreateMap<MessageFromCreationDTO, Message>();

            CreateMap<Message, MessageFromCreationDTO>();

            CreateMap<Message, MessageToReturnDTO>().ForMember(m => m.SenderPhotoURL, opt => opt.MapFrom(
                u => u.Sender.Photos.FirstOrDefault(p => p.IsMain).Url))
                .ForMember(m => m.RecipientPhotoURL, opt => opt.MapFrom(
                u => u.Recipient.Photos.FirstOrDefault(p => p.IsMain).Url));

        }
    }
}