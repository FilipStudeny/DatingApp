
using System.Security.Cryptography;
using System.Text;
using API.Data;
using API.DTO;
using API.LIB.INTERFACES;
using API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;


public class AccountController : BaseApiController
{
    private readonly DataContext dataContext;
    private readonly ITokenService tokenService;

    public AccountController(DataContext dataContext, ITokenService tokenService){
        this.dataContext = dataContext;
        this.tokenService = tokenService;
    
    }

    [HttpPost("login")]
    public async Task<ActionResult<UserDTO>> Login(LoginDTO loginDTO){

        var user = await dataContext.Users.SingleOrDefaultAsync(user => user.Username == loginDTO.Username.ToLower());
        if(user == null) return Unauthorized("Invalid username");

        using var hmac = new HMACSHA256(user.PasswordSalt);
        var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDTO.Password));
        
        if(Enumerable.SequenceEqual(computedHash, user.PasswordHash))
            return new UserDTO{
                Username = user.Username,
                Token = tokenService.CreateToken(user)
            };

        return Unauthorized("Invalid password");

    }

    

    [HttpPost("register")]
    public async Task<ActionResult<User>> Register(RegisterDTO registerDTO){
        
        if(await UserExists(registerDTO.Username)) return BadRequest("Username is already taken");
        

        using var hmac = new HMACSHA256();
        var newUser = new User{
            Username = registerDTO.Username.ToLower(),
            PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDTO.Password)),
            PasswordSalt = hmac.Key
        };

        dataContext.Add(newUser);
        await dataContext.SaveChangesAsync();

        return newUser;

    }

    private async Task<bool> UserExists(string username){

        return await dataContext.Users.AnyAsync(user => user.Username == username.ToLower());
    }

}