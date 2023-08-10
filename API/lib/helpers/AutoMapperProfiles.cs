

using API.DTO;
using API.EXTENSIONS;
using API.Models;
using AutoMapper;

namespace API.LIB.HELPERS;


public class AutoMapperProfiles: Profile
{
    public AutoMapperProfiles(){
        CreateMap<User, MemberDTO>()
        .ForMember(
            destination => destination.PhotoURL, 
            options => options.MapFrom(src => src.Photos.FirstOrDefault(photo => photo.IsMain).Url)
            )
        .ForMember(destination => destination.Age, options => options.MapFrom(src => src.DateOfBirth.CalculateAge()));
        CreateMap<Photo, PhotoDTO>();

    }

}