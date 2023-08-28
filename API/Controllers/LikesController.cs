
using System.Security.Cryptography;
using System.Text;
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

public class LikesController : BaseApiController
{
    private readonly IUnitOfWork unitOfWork;
    

    public LikesController(IUnitOfWork unitOfWork){
        this.unitOfWork = unitOfWork;
    }

    [HttpPost("{username}")]
    public async Task<ActionResult> AddLike(string username){

        var sourceUserId = User.GetUserId();
        var likedUser = await unitOfWork.UserRepository.GetUserByUsernameAsync(username);
        var sourceUser = await unitOfWork.LikesRepository.GetUserWithLikes(sourceUserId);

        if(likedUser == null) return NotFound();
        if(sourceUser.UserName == username) return BadRequest("You cannot like yourself");

        var userLike = await unitOfWork.LikesRepository.GetUserLike(sourceUserId, likedUser.Id);
        if(userLike != null) return BadRequest("You have already liked this user");

        userLike = new UserLike{
            SourceUserId = sourceUserId,
            TargetUserId = likedUser.Id
        };
        sourceUser.LikedUsers.Add(userLike);

        if(await unitOfWork.Complete()) return Ok();
        return BadRequest("Failed to like user"); 
    }

    [HttpGet]
    public async Task<ActionResult<PagedList<LikeDTO>>> GetUserLikes([FromQuery]LikesParams likesParams){

        likesParams.UserID = User.GetUserId();
        var users = await unitOfWork.LikesRepository.GetUserLikes(likesParams);
        Response.AddPaginationHeader(new PaginationHeader(users.CurrentPage, users.PageSize, users.TotalCount, users.TotalPages));
        return Ok(users);
    
    }
}