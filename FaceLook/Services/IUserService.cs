using FaceLook.Data.Entities;

namespace FaceLook.Services
{
    public interface IUserService
    {
        Task<IList<User>> GetUsersAsync();
        Task<User?> GetUserByIdAsync(Guid id);
        Task<User?> GetUserByEmailAsync(string email);
        Task<User> CreateUserAsync(User userForCreation);
        Task<User> UpdateUserAsync(User userForUpdate);
        Task<bool> DeleteUserAsync(Guid id);
        Task<User?> GetCurrentUserAsync();
        Task<string> GetRequiredCurrentUserNameAsync();
    }
}
