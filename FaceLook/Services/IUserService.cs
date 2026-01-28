using Microsoft.AspNetCore.Identity;

namespace FaceLook.Services
{
    public interface IUserService
    {
        Task<IList<IdentityUser>> GetUsersAsync();
        Task<IdentityUser?> GetUserByIdAsync(Guid id);
        Task<IdentityUser> CreateUserAsync(IdentityUser userForCreation);
        Task<IdentityUser> UpdateUserAsync(IdentityUser userForUpdate);
        Task<bool> DeleteUserAsync(Guid id);
        Task<IdentityUser?> GetCurrentUserAsync();
    }
}
