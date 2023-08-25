

using API.Models;

namespace API.LIB.INTERFACES;


public interface ITokenService
{
    Task<string> CreateToken(User user);

}