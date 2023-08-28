

using API.DTO;
using API.LIB.HELPERS;
using API.Models;

namespace API.LIB.INTERFACES;


public interface IUserRepository
{
    void Update(User user);
    Task<IEnumerable<User>> GetUsersAsync();
    Task<User> GetUserByIdAsync(int id);
    Task<User> GetUserByUsernameAsync(string username);
    Task<PagedList<MemberDTO>> GetMembersAsync(UserParams userParams);
    Task<MemberDTO> GetMemberAsync(string username);
    Task<string> GetUserGender(string username);

}