using Azure.Storage.Files.Shares.Models;
using FaceLook.Services.Models;
using Microsoft.AspNetCore.Http;

namespace FaceLook.Services.Interfaces
{
    public interface IFileShareService
    {
        Task<IList<ShareFileItem>> GetFilesAsync(string userId, string directoryName);
        Task<ShareFileDownloadInfo> GetFileByNameAsync(string userId, string directoryName, string fileName);
        Task<FileUploadResult> UploadFileAsync(string userId, string directoryName, IFormFile file);
        Task<bool> DeleteFileAsync(string userId, string directoryName, string fileName);
        Task<FileUploadResult> RefreshSasUrlAsync(string userId, string directoryName, string fileName);
    }
}
