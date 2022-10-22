using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Backend.DTO;
using Backend.Entities;
using Backend.Extensions;

namespace Backend.Helpers;

public class AutoMapperProfiles : Profile
{
    public AutoMapperProfiles()
    {
        CreateMap<AppUser, MemberDTO>()
            .ForMember(dest => dest.PhotoUrl,
                opt => opt.MapFrom(src => src.Photos.FirstOrDefault(p => p.IsMain).Url))
            .ForMember(dest => dest.Age,
                opt => opt.MapFrom(src => src.DateOfBirth.CalculateAge()));

        CreateMap<Photo, PhotoDto>();
        CreateMap<MemberUpdateDTO, AppUser>();
        CreateMap<UserToRegisterDTO, AppUser>();
        CreateMap<Message, MessageDTO>()
            .ForMember(destinationMember => destinationMember.SenderPhotoUrl,
                        opt => opt.MapFrom(src => src.Sender.Photos.FirstOrDefault(p => p.IsMain).Url))
            .ForMember(destinationMember => destinationMember.RecipientPhotoUrl,
                        opt => opt.MapFrom(src => src.Recipient.Photos.FirstOrDefault(p => p.IsMain).Url));
    }

}