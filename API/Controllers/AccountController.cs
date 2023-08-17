
using System.Security.Cryptography;
using System.Text;
using API.Data;
using API.DTO;
using API.LIB.INTERFACES;
using API.Models;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;


public class AccountController : BaseApiController
{
    private readonly DataContext dataContext;
    private readonly ITokenService tokenService;
    private readonly IMapper mapper;

    public AccountController(DataContext dataContext, ITokenService tokenService, IMapper mapper){
        this.dataContext = dataContext;
        this.tokenService = tokenService;
        this.mapper = mapper;
    }

    [HttpPost("login")]
    public async Task<ActionResult<UserDTO>> Login(LoginDTO loginDTO){

        var user = await dataContext.Users.Include(photo => photo.Photos).SingleOrDefaultAsync(user => user.UserName == loginDTO.Username.ToLower());
        if(user == null) return Unauthorized("Invalid username");

        using var hmac = new HMACSHA256(user.PasswordSalt);
        var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDTO.Password));
        
        if(Enumerable.SequenceEqual(computedHash, user.PasswordHash))
            return new UserDTO{
                Username = user.UserName,
                Token = tokenService.CreateToken(user),
                PhotoUrl = user.Photos.FirstOrDefault(x => x.IsMain)?.Url,
                KnownAs = user.KnownAs,
                Gender = user.Gender
            };

        return Unauthorized("Invalid password");
    }

    [HttpPost("register")]
    public async Task<ActionResult<UserDTO>> Register(RegisterDTO registerDTO){
        
        if(await UserExists(registerDTO.Username)) return BadRequest("Username is already taken");

        var user = mapper.Map<User>(registerDTO);
        using var hmac = new HMACSHA256();
        user.UserName = registerDTO.Username.ToLower();
        user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDTO.Password));
        user.PasswordSalt = hmac.Key;

        dataContext.Users.Add(user);
        await dataContext.SaveChangesAsync();

        return new UserDTO{
            Username = user.UserName,
            Token = tokenService.CreateToken(user),
            KnownAs = user.KnownAs,
            Gender = user.Gender

        };
    }

    private async Task<bool> UserExists(string username){

        return await dataContext.Users.AnyAsync(user => user.UserName == username.ToLower());
    }

}