using FaceLook.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace FaceLook.Services
{
    public class UserService(IHttpContextAccessor httpContextAccessor, ApplicationDbContext applicationDbContext) : IUserService
    {
        public async Task<IdentityUser<Guid>> CreateUserAsync(IdentityUser<Guid> userForCreation)
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

        public async Task<IdentityUser<Guid>?> GetCurrentUserAsync()
        {
            string? userId = httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId is null) return null;

            return applicationDbContext.Users
                .AsNoTracking()
                .FirstOrDefault(user => user.Id == Guid.Parse(userId));
        }

        public async Task<IdentityUser<Guid>?> GetUserByIdAsync(Guid id)
        {
            return applicationDbContext.Users
                .AsNoTracking()
                .FirstOrDefault(user => user.Id == id);
        }

        public async Task<IList<IdentityUser<Guid>>> GetUsersAsync()
        {
            return await applicationDbContext.Users
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IdentityUser<Guid>> UpdateUserAsync(IdentityUser<Guid> userForUpdate)
        {
            var entityEntry = applicationDbContext.Users.Update(userForUpdate);
            await applicationDbContext.SaveChangesAsync();
            return entityEntry.Entity;
        }
    }
}
