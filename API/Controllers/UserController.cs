using API.Data;
using API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")] // /api/user
public class UserController : ControllerBase
{

    private readonly DataContext dataContext;
    public UserController(DataContext dataContext){
        this.dataContext = dataContext;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<User>>> GetUsers(){
        return await dataContext.Users.ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<User>> GetUser(int id){
        return await dataContext.Users.FindAsync(id);
    }


}
