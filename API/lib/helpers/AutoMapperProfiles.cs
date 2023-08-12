

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
    }
}

