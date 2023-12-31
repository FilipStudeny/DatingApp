
using API.LIB.HELPERS;
using API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

public class AdminController : BaseApiController
{
    private readonly UserManager<User> userManager;
    public AdminController(UserManager<User> userManager){
        this.userManager = userManager;
    }

    [Authorize(Policy = "RequireAdminRole")]
    [HttpGet("users-with-roles")]
    public async Task<ActionResult> GetUserWithRoles(){

            var users = await userManager.Users
                .Include(r => r.UserRoles)
                .ThenInclude(r => r.Role)
                .OrderBy(u => u.UserName)
                .Select(u => new
                {
                    u.Id,
                    Username = u.UserName,
                    Roles = u.UserRoles.Select(r => r.Role.Name).ToList()
                })
                .ToListAsync();

            return Ok(users);
    }

    [Authorize(Policy = "RequireAdminRole")]
    [HttpPost("edit-roles/{username}")]
    public async Task<ActionResult> EditRoles(string username, [FromQuery]string roles){

        if(string.IsNullOrEmpty(roles)) return BadRequest("You must select at least one role");
        var selectedRoles = roles.Split(",").ToArray();
        var user = await userManager.FindByNameAsync(username);
        if(user == null) return NotFound();

        var userRoles = await userManager.GetRolesAsync(user);
        var result = await userManager.AddToRolesAsync(user, selectedRoles.Except(userRoles));
        if(!result.Succeeded) return BadRequest("Failed to add to roles");
        
        result = await userManager.RemoveFromRolesAsync(user, userRoles.Except(selectedRoles));
        if(!result.Succeeded) return BadRequest("Failed to remove roles");

        return Ok(await userManager.GetRolesAsync(user));
    }

    [Authorize(Policy = "ModeratePhotoRole")]
    [HttpGet("photos-to-moderate")]
    public ActionResult GetPhotosForModeration(){
        return Ok("Admins or moderators can see this");
    }

}
