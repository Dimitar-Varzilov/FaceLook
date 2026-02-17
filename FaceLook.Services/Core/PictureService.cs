using FaceLook.Common.Constants;
using FaceLook.Data;
using FaceLook.Data.Entities;
using FaceLook.Services.Exceptions;
using FaceLook.Services.Interfaces;
using FaceLook.Web.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

using Microsoft.EntityFrameworkCore;

namespace FaceLook.Services.Core
{
    internal class PictureService(
        ApplicationDbContext dbContext,
        UserManager<User> userManager,
        IFileShareService fileShareService) : IPictureService
    {
        public async Task<IEnumerable<PictureViewModel>> GetUserPicturesAsync(string userId)
        {
            await EnsureUserExistsAsync(userId);

            var pictures = await dbContext.Pictures
                .Where(p => p.UserId == userId && !p.IsDeleted)
                .ToListAsync();

            var refreshTasks = pictures
                .Where(p => p.SasExpiry <= DateTimeOffset.UtcNow)
                .Select(p => RefreshPictureSasAsync(userId, p));

            await Task.WhenAll(refreshTasks);
            await dbContext.SaveChangesAsync();

            return pictures.Select(p => new PictureViewModel()
            {
                Id = p.Id,
                Name = p.Name,
                SasUrl = p.SasUrl
            });
        }

        public async Task UploadPictureAsync(string userId, IFormFile file)
        {
            await EnsureUserExistsAsync(userId);

            var result = await fileShareService.UploadFileAsync(userId, FileDirectoryConstants.Pictures, file);

            var picture = new Picture()
            {
                Id = Guid.NewGuid(),
                Name = result.FileName,
                SasUrl = result.SasUrl,
                SasExpiry = result.SasExpiry,
                UserId = userId,
                User = null!,
                CreatedAt = DateTimeOffset.UtcNow,
                ModifiedBy = userId,
                IsDeleted = false
            };

            await dbContext.Pictures.AddAsync(picture);
            await dbContext.SaveChangesAsync();
        }

        public async Task DeletePictureAsync(string userId, Guid pictureId)
        {
            await EnsureUserExistsAsync(userId);

            var picture = await dbContext.Pictures
                .FirstOrDefaultAsync(p => p.Id == pictureId && p.UserId == userId && !p.IsDeleted)
                ?? throw new ResourceNotFoundException("Picture not found");

            await fileShareService.DeleteFileAsync(userId, FileDirectoryConstants.Pictures, picture.Name);

            picture.IsDeleted = true;
            picture.ModifiedAt = DateTimeOffset.UtcNow;
            picture.ModifiedBy = userId;

            await dbContext.SaveChangesAsync();
        }

        private async Task EnsureUserExistsAsync(string userId)
        {
            _ = await userManager.FindByIdAsync(userId)
                ?? throw new ResourceNotFoundException("User not found");
        }

        private async Task RefreshPictureSasAsync(string userId, Picture picture)
        {
            var result = await fileShareService.RefreshSasUrlAsync(userId, FileDirectoryConstants.Pictures, picture.Name);
            picture.SasUrl = result.SasUrl;
            picture.SasExpiry = result.SasExpiry;
            picture.ModifiedAt = DateTimeOffset.UtcNow;
            picture.ModifiedBy = userId;
        }
    }
}
