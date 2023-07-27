using API.Data;
using API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

[Authorize]
public class UserController : BaseApiController
{

    private readonly DataContext dataContext;
    public UserController(DataContext dataContext){
        this.dataContext = dataContext;
    }

    [AllowAnonymous]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<User>>> GetUsers(){
        return await dataContext.Users.ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<User>> GetUser(int id){
        return await dataContext.Users.FindAsync(id);
    }


}
