
using API.DTO;
using API.LIB.HELPERS;
using API.Models;
using CloudinaryDotNet.Actions;

namespace API.LIB.INTERFACES;


public interface ILikesRepository
{
    Task<UserLike> GetUserLike(int sourceUserId, int targetUserId);
    Task<User> GetUserWithLikes(int userId);
    Task<PagedList<LikeDTO>> GetUserLikes(LikesParams likesParams);

}