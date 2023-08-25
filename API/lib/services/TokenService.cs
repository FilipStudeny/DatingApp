

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using API.LIB.INTERFACES;
using API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace API.LIB.SERVICES;


public class TokenService : ITokenService
{
    private readonly SymmetricSecurityKey key;
    private readonly UserManager<User> userManager;

    public TokenService(IConfiguration configuration, UserManager<User> userManager){
        key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["token_key"]));
        this.userManager = userManager;
    }

    public async Task<string> CreateToken(User user)
    {
        var claims = new List<Claim>{
            new Claim(JwtRegisteredClaimNames.NameId, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName),
        };

        var roles = await userManager.GetRolesAsync(user);
        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
        var tokenDescriptor = new SecurityTokenDescriptor{
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.Now.AddDays(7),
            SigningCredentials = credentials
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}