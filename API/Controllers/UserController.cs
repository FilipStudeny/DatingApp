using System.Security.Claims;
using API.Data;
using API.DTO;
using API.EXTENSIONS;
using API.LIB.HELPERS;
using API.LIB.INTERFACES;
using API.Models;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

[Authorize]
public class UserController : BaseApiController
{
    private readonly IUnitOfWork unitOfWork;
    private readonly IMapper mapper;
    private readonly IPhotoService photoService;


    public UserController(IUnitOfWork unitOfWork, IMapper mapper, IPhotoService photoService){
        this.unitOfWork = unitOfWork;
        this.mapper = mapper;
        this.photoService = photoService;
    }
    [HttpGet]
    public async Task<ActionResult<PagedList<MemberDTO>>> GetUsers([FromQuery]UserParams userParams){

        var gender = await unitOfWork.UserRepository.GetUserGender(User.GetUsername());
        userParams.CurrentUsername = User.GetUsername();

            if (string.IsNullOrEmpty(userParams.Gender))
                userParams.Gender = gender == "male" ? "female" : "male";

        var users = await unitOfWork.UserRepository.GetMembersAsync(userParams);
        Response.AddPaginationHeader(new PaginationHeader(users.CurrentPage, users.PageSize, users.TotalCount, users.TotalPages));
        return Ok(users);
    }

    [HttpGet("{username}")]
    public async Task<ActionResult<MemberDTO>> GetUser(string username){
        return await unitOfWork.UserRepository.GetMemberAsync(username);
    }

    [HttpPut]
    public async Task<ActionResult> UpdateUser(MemberUpdateDTO memberUpdateDTO){
        var username = User.GetUsername();
        var user = await unitOfWork.UserRepository.GetUserByUsernameAsync(username);

        if(user == null) return NotFound();

        mapper.Map(memberUpdateDTO, user);
        if(await unitOfWork.Complete()) return NoContent();

        return BadRequest("Failed to update user");
    }

    [HttpPost("add-photo")]
    public async Task<ActionResult<PhotoDTO>> AddPhoto(IFormFile file){

        var user = await unitOfWork.UserRepository.GetUserByUsernameAsync(User.GetUsername());
        if(user == null) return NotFound();

        var result = await photoService.AddPhotoAsync(file);
        if(result.Error != null) return BadRequest(result.Error.Message);
        var photo = new Photo{
            Url = result.SecureUrl.AbsoluteUri,
            PublicId = result.PublicId
        };

        if(user.Photos.Count == 0) photo.IsMain = true;
        user.Photos.Add(photo);

        if(await unitOfWork.Complete()) {
            return CreatedAtAction(nameof(GetUser), new {username = user.UserName}, mapper.Map<PhotoDTO>(photo));
        }

        return BadRequest("Failed to upload photo.");
    }

    [HttpPut("set-main-photo/{photoId}")]
    public async Task<ActionResult> SetMainPhoto(int photoId){

        var user = await unitOfWork.UserRepository.GetUserByUsernameAsync(User.GetUsername());
        if(user == null) return NotFound();

        var photo = user.Photos.FirstOrDefault(photo => photo.Id == photoId);
        if(photo == null) return NotFound();
        if(photo.IsMain) return BadRequest("This is already your profile picture");

        var mainPhoto = user.Photos.FirstOrDefault(photo => photo.IsMain);
        if(mainPhoto != null) mainPhoto.IsMain = false;
        photo.IsMain = true;

        if(await unitOfWork.Complete()) return NoContent();
        return BadRequest("Problem setting profile picture");

    }

    
    [HttpDelete("delete-photo/{photoId}")]
    public async Task<ActionResult> DeletePhoto(int photoId){

        var user = await unitOfWork.UserRepository.GetUserByUsernameAsync(User.GetUsername());
        var photo = user.Photos.FirstOrDefault(x => x.Id == photoId);

        if (photo == null) return NotFound();
        if (photo.IsMain) return BadRequest("You cannot delete your main photo");

        if (photo.PublicId != null)
        {
            var result = await photoService.DeletePhotoAsync(photo.PublicId);
            if (result.Error != null) return BadRequest(result.Error.Message);
        }

        user.Photos.Remove(photo);

        if (await unitOfWork.Complete()) return Ok();

        return BadRequest("Failed to delete the photo");
    }
}
