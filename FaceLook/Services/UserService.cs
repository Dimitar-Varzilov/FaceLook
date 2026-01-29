using FaceLook.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace FaceLook.Services
{
    public class UserService(IHttpContextAccessor httpContextAccessor, ApplicationDbContext applicationDbContext) : IUserService
    {
        public async Task<IdentityUser> CreateUserAsync(IdentityUser userForCreation)
        {
            var createdUser = await applicationDbContext.Users.AddAsync(userForCreation);
            return createdUser.Entity;
        }

        public async Task<bool> DeleteUserAsync(Guid id)
        {
            var user = await GetUserByIdAsync(id);
            if (user is null) return false;

            applicationDbContext.Users.Remove(user);
            return await applicationDbContext.SaveChangesAsync() > 0;
        }

        public async Task<IdentityUser?> GetCurrentUserAsync()
        {
            string? userId = httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return userId is null
                ? null
                : applicationDbContext.Users
                .AsNoTracking()
                .FirstOrDefault(user => user.Id == userId);
        }

        public async Task<IdentityUser?> GetUserByEmailAsync(string email)
        {
            return applicationDbContext.Users
                .AsNoTracking()
                .Where(user => user.NormalizedEmail != null)
                .FirstOrDefault(user => user.NormalizedEmail == email);
        }

        public async Task<IdentityUser?> GetUserByIdAsync(Guid id)
        {
            return applicationDbContext.Users
                .AsNoTracking()
                .FirstOrDefault(user => user.Id == id.ToString());
        }

        public async Task<IList<IdentityUser>> GetUsersAsync()
        {
            return await applicationDbContext.Users
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IdentityUser> UpdateUserAsync(IdentityUser userForUpdate)
        {
            var entityEntry = applicationDbContext.Users.Update(userForUpdate);
            await applicationDbContext.SaveChangesAsync();
            return entityEntry.Entity;
        }
    }
}
