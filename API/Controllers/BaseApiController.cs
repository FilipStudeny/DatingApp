
using API.LIB.HELPERS;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers{

[ServiceFilter(typeof(LogUserActivity))]
[ApiController]
[Route("api/[controller]")] // /api/user
public class BaseApiController : ControllerBase
{

}
}