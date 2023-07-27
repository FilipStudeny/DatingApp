

using API.Models;

namespace API.LIB.INTERFACES;


public interface ITokenService
{
    string CreateToken(User user);

}