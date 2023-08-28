using API.Models;
using Microsoft.EntityFrameworkCore;
using API.DTO;
using AutoMapper;

namespace API.Data.REPOSITORY;

using API.EXTENSIONS;
using API.LIB.HELPERS;
using API.LIB.INTERFACES;

public class LikesRepository: ILikesRepository{

    private readonly DataContext _context;
    public LikesRepository(DataContext context)
    {
        _context = context;
    }

    public async Task<UserLike> GetUserLike(int sourceUserId, int targetUserId)
    {
        return await _context.Likes.FindAsync(sourceUserId, targetUserId);
    }

    public async Task<PagedList<LikeDTO>> GetUserLikes(LikesParams likesParams)
    {
        var users = _context.Users.OrderBy(user => user.UserName).AsQueryable();
        var likes = _context.Likes.AsQueryable();

        if(likesParams.predicate == "liked"){
            likes = likes.Where(like => like.SourceUserId == likesParams.UserID);
            users = likes.Select(like => like.TargetUser);
        }
        if(likesParams.predicate == "likedBy"){
            likes = likes.Where(like => like.TargetUserId == likesParams.UserID);
            users = likes.Select(like => like.SourceUser);
        }

        var likedUsers = users.Select(USER => new LikeDTO{
            UserName = USER.UserName,
            KnownAs = USER.KnownAs,
            Age = USER.DateOfBirth.CalculateAge(),
            PhotoUrl = USER.Photos.FirstOrDefault(PHOTO => PHOTO.IsMain).Url,
            City = USER.City,
            Id = USER.Id
        });

        return await PagedList<LikeDTO>.CreateAsync(likedUsers, likesParams.PageNumber, likesParams.PageSize);
    }

    public async Task<User> GetUserWithLikes(int userId)
    {
        return await _context.Users.Include(x => x.LikedUsers).FirstOrDefaultAsync(x => x.Id == userId);
    }
    
}
