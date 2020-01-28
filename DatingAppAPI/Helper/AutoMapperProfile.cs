using AutoMapper;
using DatingApp.API.Dtos;
using DatingAppAPI.DTO;
using DatingAppAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DatingAppAPI.Helper
{
    public class AutoMapperProfile: Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<User, UserForListDto>()
                .ForMember(dest => dest.PhotoUrl, src => src.MapFrom(x => x.Photos.FirstOrDefault(p => p.IsMain).Url))
                .ForMember(dest => dest.Age, src => src.MapFrom(x => x.DateOfBirth.CalculateAge()));
            CreateMap<User, UserForDetailDto>()
                .ForMember(dest => dest.PhotoUrl, src => src.MapFrom(x => x.Photos.FirstOrDefault(p => p.IsMain).Url))
                .ForMember(dest => dest.Age, src => src.MapFrom(x => x.DateOfBirth.CalculateAge()));
            CreateMap<Photo, PhotoForDetailDto>();
            CreateMap<UserForUpdateDto, User>();
            CreateMap<Photo, PhotoForReturnDto>();
            CreateMap<PhotoForCreationDto, Photo>();
        }
    }
}
