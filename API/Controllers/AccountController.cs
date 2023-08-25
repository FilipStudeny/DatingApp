
using System.Security.Cryptography;
using System.Text;
using API.Data;
using API.DTO;
using API.LIB.INTERFACES;
using API.Models;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;


public class AccountController : BaseApiController
{
    private readonly UserManager<User> userManager;
    private readonly ITokenService tokenService;
    private readonly IMapper mapper;

    public AccountController(UserManager<User> userManager, ITokenService tokenService, IMapper mapper){
        this.userManager = userManager;
        this.tokenService = tokenService;
        this.mapper = mapper;
    }

    [HttpPost("login")]
    public async Task<ActionResult<UserDTO>> Login(LoginDTO loginDTO){

        var user = await userManager.Users.Include(photo => photo.Photos).SingleOrDefaultAsync(user => user.UserName == loginDTO.Username.ToLower());
        if(user == null) return Unauthorized("Invalid username");
        var result = await userManager.CheckPasswordAsync(user, loginDTO.Password);
        if(!result) return Unauthorized("Invalid password");

        return new UserDTO{
            Username = user.UserName,
            Token = await tokenService.CreateToken(user),
            PhotoUrl = user.Photos.FirstOrDefault(x => x.IsMain)?.Url,
            KnownAs = user.KnownAs,
            Gender = user.Gender
        };
    }

    [HttpPost("register")]
    public async Task<ActionResult<UserDTO>> Register(RegisterDTO registerDTO){
        
        if(await UserExists(registerDTO.Username)) return BadRequest("Username is already taken");

        var user = mapper.Map<User>(registerDTO);
        user.UserName = registerDTO.Username.ToLower();

        var result = await userManager.CreateAsync(user, registerDTO.Password);
        if(!result.Succeeded) return BadRequest(result.Errors);

        var rolesResult = await userManager.AddToRoleAsync(user, "Member");
        if(!rolesResult.Succeeded) return BadRequest(rolesResult.Errors);


        return new UserDTO{
            Username = user.UserName,
            Token = await tokenService.CreateToken(user),
            KnownAs = user.KnownAs,
            Gender = user.Gender
        };
    }

    private async Task<bool> UserExists(string username){

        return await userManager.Users.AnyAsync(user => user.UserName == username.ToLower());
    }

}