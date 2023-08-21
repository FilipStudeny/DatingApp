
using System.Security.Claims;

namespace API.EXTENSIONS;

    public static class ClaimsPrincipleExtensions
    {
        public static string GetUsername(this ClaimsPrincipal user){
            return user.FindFirst(ClaimTypes.Name)?.Value;
        }
        public static string GetUserId(this ClaimsPrincipal user){
            return user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }
    }