
using API.EXTENSIONS;
using API.LIB.INTERFACES;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
namespace API.LIB.HELPERS;

public class LogUserActivity : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var resultContext = await next();
        if(!resultContext.HttpContext.User.Identity.IsAuthenticated) return;
        
        var userId = resultContext.HttpContext.User.GetUserId();
        var uow = resultContext.HttpContext.RequestServices.GetRequiredService<IUnitOfWork>();
        var user = await uow.UserRepository.GetUserByIdAsync(userId);
        user.LastActive = DateTime.UtcNow;

        await uow.Complete();
    }
}

