using FaceLook.Data.Entities;

namespace FaceLook.Services.Interfaces
{
    public interface IUserService
    {
        Task<IList<User>> GetUsersAsync();
        Task<User?> GetUserByIdAsync(string id);
        Task<User?> GetUserByEmailAsync(string? email);
        Task<User> CreateUserAsync(User userForCreation);
        Task<User> UpdateUserAsync(User userForUpdate);
        Task<bool> DeleteUserAsync(string id);
        Task<User?> GetCurrentUserAsync();
        Task<string> GetRequiredCurrentUserNameAsync();
    }
}
