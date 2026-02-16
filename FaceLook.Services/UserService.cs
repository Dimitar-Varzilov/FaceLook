using FaceLook.Data;
using FaceLook.Data.Entities;
using FaceLook.Services.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace FaceLook.Services
{
    public class UserService(IHttpContextAccessor httpContextAccessor, ApplicationDbContext applicationDbContext) : IUserService
    {
        public async Task<User> CreateUserAsync(User userForCreation)
        {
            var createdUser = await applicationDbContext.Users.AddAsync(userForCreation);
            await applicationDbContext.SaveChangesAsync();
            return createdUser.Entity;
        }

        public async Task<bool> DeleteUserAsync(string id)
        {
            var user = await GetUserByIdAsync(id);
            if (user is null) return false;

            applicationDbContext.Users.Remove(user);
            return await applicationDbContext.SaveChangesAsync() > 0;
        }

        public async Task<User?> GetCurrentUserAsync()
        {
            string? userId = httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId is null) return null;

            return await applicationDbContext.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(user => user.Id == userId);
        }

        public async Task<string> GetRequiredCurrentUserNameAsync()
        {
            User currentUser = await GetCurrentUserAsync() ?? throw new ResourceNotFoundException("User is not found");
            return currentUser.UserName ?? throw new ValidationException("UserName must be set");
        }

        public async Task<User?> GetUserByEmailAsync(string? email)
        {
            if (email is null) return null;

            return await applicationDbContext.Users
                .AsNoTracking()
                .Where(user => user.NormalizedEmail != null)
                .FirstOrDefaultAsync(user => user.NormalizedEmail == email);
        }

        public async Task<User?> GetUserByIdAsync(string id)
        {
            return await applicationDbContext.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(user => user.Id == id);
        }

        public async Task<IList<User>> GetUsersAsync()
        {
            return await applicationDbContext.Users
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<User> UpdateUserAsync(User userForUpdate)
        {
            var entityEntry = applicationDbContext.Users.Update(userForUpdate);
            await applicationDbContext.SaveChangesAsync();
            return entityEntry.Entity;
        }
    }
}
