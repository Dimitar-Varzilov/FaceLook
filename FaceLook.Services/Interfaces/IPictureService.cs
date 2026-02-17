using FaceLook.Web.ViewModels;
using Microsoft.AspNetCore.Http;

namespace FaceLook.Services.Interfaces
{
    public interface IPictureService
    {
        Task<IEnumerable<PictureViewModel>> GetUserPicturesAsync(string userId);
        Task UploadPictureAsync(string userId, IFormFile file);
        Task DeletePictureAsync(string userId, Guid pictureId);
    }
}
