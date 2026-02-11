using Azure.Storage.Files.Shares.Models;
using FaceLook.ViewModels;

namespace FaceLook.Services
{
    public interface IFileShareService
    {
        Task<IList<string>> GetCurrentUserPictureUrls(int page);
        Task<IList<ShareFileItemViewModel>> GetCurrentUserPictures();
        Task<ShareFileDownloadInfo> GetPictureByNameAsync(string fileName);
        Task UploadFile(IFormFile file);
        Task<bool> DeleteFile(string fileName);
    }
}
