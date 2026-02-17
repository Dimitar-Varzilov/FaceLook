using Azure.Storage.Files.Shares;
using Azure.Storage.Files.Shares.Models;
using Azure.Storage.Sas;
using FaceLook.Services.Exceptions;
using FaceLook.Services.Interfaces;
using FaceLook.Services.Models;
using FaceLook.Web.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace FaceLook.Services.Core
{
    public class FileShareService(IOptions<FileShareOptions> fileShareOptions) : IFileShareService
    {
        public async Task<IList<ShareFileItem>> GetFilesAsync(string userId, string directoryName)
        {
            IList<ShareFileItem> files = [];
            var directoryClient = await GetUserDirectoryClientAsync(userId, directoryName);

            await foreach (var item in directoryClient.GetFilesAndDirectoriesAsync())
            {
                if (item.IsDirectory) continue;
                files.Add(item);
            }

            return files;
        }

        public async Task<ShareFileDownloadInfo> GetFileByNameAsync(string userId, string directoryName, string fileName)
        {
            var fileClient = await GetFileClientAsync(userId, directoryName, fileName);
            var downloadResponse = await fileClient.DownloadAsync();
            return downloadResponse.Value;
        }

        public async Task<FileUploadResult> UploadFileAsync(string userId, string directoryName, IFormFile file)
        {
            var directoryClient = await GetUserDirectoryClientAsync(userId, directoryName);

            var fileName = Guid.NewGuid() + file.FileName;
            var fileClientResponse = await directoryClient.CreateFileAsync(fileName, file.Length);

            using var stream = fileClientResponse.Value.OpenWrite(false, 0);
            await file.CopyToAsync(stream);

            return await GenerateSasUrlAsync(fileClientResponse.Value);
        }

        public async Task<bool> DeleteFileAsync(string userId, string directoryName, string fileName)
        {
            var fileClient = await GetFileClientAsync(userId, directoryName, fileName);
            var result = await fileClient.DeleteAsync();
            return !result.IsError;
        }

        public async Task<FileUploadResult> RefreshSasUrlAsync(string userId, string directoryName, string fileName)
        {
            var fileClient = await GetFileClientAsync(userId, directoryName, fileName);
            return await GenerateSasUrlAsync(fileClient);
        }

        private async Task<ShareDirectoryClient> GetUserDirectoryClientAsync(string userId, string directoryName)
        {
            var shareClient = new ShareClient(fileShareOptions.Value.ConnectionString, fileShareOptions.Value.FileShare);

            //Create User directory if does not exists
            await shareClient.GetDirectoryClient(userId).CreateIfNotExistsAsync();

            //Create directory based on directoryName
            var directoryPath = Path.Combine(userId, directoryName);
            var directoryClient = shareClient.GetDirectoryClient(directoryPath);
            await directoryClient.CreateIfNotExistsAsync();

            return directoryClient;
        }

        private async Task<ShareFileClient> GetFileClientAsync(string userId, string directoryName, string fileName)
        {
            var directoryClient = await GetUserDirectoryClientAsync(userId, directoryName);
            return directoryClient.GetFileClient(fileName);
        }

        private async Task<FileUploadResult> GenerateSasUrlAsync(ShareFileClient fileClient)
        {
            if (!fileClient.CanGenerateSasUri)
                throw new ValidationException("SAS generation is not supported for this file client");

            var expiresOn = new DateTimeOffset(DateTime.UtcNow.AddHours(fileShareOptions.Value.SasExpiryInHours));
            var sasUri = fileClient.GenerateSasUri(ShareFileSasPermissions.Read, expiresOn);

            return new FileUploadResult(fileClient.Name, sasUri.AbsoluteUri, expiresOn);
        }
    }
}
