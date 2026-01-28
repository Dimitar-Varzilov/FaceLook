using Microsoft.AspNetCore.Identity;

namespace FaceLook.Services
{
    public interface IUserService
    {
        Task<IList<IdentityUser<Guid>>> GetUsersAsync();
        Task<IdentityUser<Guid>?> GetUserByIdAsync(Guid id);
        Task<IdentityUser<Guid>> CreateUserAsync(IdentityUser<Guid> userForCreation);
        Task<IdentityUser<Guid>> UpdateUserAsync(IdentityUser<Guid> userForUpdate);
        Task<bool> DeleteUserAsync(Guid id);
        Task<IdentityUser<Guid>?> GetCurrentUserAsync();
    }
}
