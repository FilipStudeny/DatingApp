

using API.DTO;
using API.Models;
using API.EXTENSIONS;
using AutoMapper;

namespace API.LIB.HELPERS;


public class AutoMapperProfiles: Profile
{
    public AutoMapperProfiles()
    {
        CreateMap<User, MemberDTO>()
            .ForMember(dest => dest.PhotoUrl, opt => opt.MapFrom(src => 
                src.Photos.FirstOrDefault(x => x.IsMain).Url))
            .ForMember(dest => dest.Age, opt => opt.MapFrom(src => src.DateOfBirth.CalculateAge()));
        CreateMap<Photo, PhotoDTO>();
        CreateMap<MemberUpdateDTO, User>();
        CreateMap<RegisterDTO, User>();
        CreateMap<Message, MessageDTO>()
            .ForMember(d => d.SenderPhotoUrl, o => o.MapFrom(s => s.Sender.Photos.FirstOrDefault(p => p.IsMain).Url))
            .ForMember(d => d.RecipientPhotoUrl, o => o.MapFrom(s => s.Recipient.Photos.FirstOrDefault(p => p.IsMain).Url));


    }
}

