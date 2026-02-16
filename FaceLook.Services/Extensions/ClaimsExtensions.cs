using FaceLook.Services.Exceptions;
using System.Security.Claims;

namespace FaceLook.Services.Extensions
{
    public static class ClaimsExtensions
    {
        public static string GetUserId(this ClaimsPrincipal claimsPrincipal)
        {
            return claimsPrincipal.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? throw new ValidationException("Cannot extract the UserId from token");
        }

        public static string GetEmail(this ClaimsPrincipal claimsPrincipal)
        {
            return claimsPrincipal.FindFirst(ClaimTypes.Email)?.Value ?? throw new ValidationException("Cannot extract the Email from token");
        }
    }
}
