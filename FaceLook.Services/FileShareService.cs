using Azure.Storage.Files.Shares;
using Azure.Storage.Files.Shares.Models;
using Azure.Storage.Sas;
using FaceLook.Common.Constants;
using FaceLook.Services.Exceptions;
using FaceLook.Services.Extensions;
using FaceLook.Web.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace FaceLook.Services
{
    public class FileShareService(IOptions<FileShareOptions> blobOptions, IHttpContextAccessor httpContextAccessor) : IFileShareService
    {
        private readonly string UserName = httpContextAccessor.HttpContext?.User.Identity?.Name ?? throw new ArgumentNullException(nameof(UserName));
        public async Task<IList<string>> GetCurrentUserPictureUrls(int page)
        {
            IList<string> pictures = [];
            var shareDirectoryClient = await GetUserDirectoryClient(FileDirectoryConstants.Pictures);
            await foreach (var fileSharee in shareDirectoryClient.GetFilesAndDirectoriesAsync())
            {
                if (fileSharee.IsDirectory) continue;

                var shareFileClient = shareDirectoryClient.GetFileClient(fileSharee.Name);
                pictures.Add(await GetSasUrl(shareFileClient));
            }
            return pictures;
        }

        public async Task<IList<ShareFileItemViewModel>> GetCurrentUserPictures()
        {
            IList<ShareFileItemViewModel> pictures = [];
            var shareDirectoryClient = await GetUserDirectoryClient(FileDirectoryConstants.Pictures);
            await foreach (var fileSharee in shareDirectoryClient.GetFilesAndDirectoriesAsync())
            {
                if (fileSharee.IsDirectory) continue;

                var shareFileClient = shareDirectoryClient.GetFileClient(fileSharee.Name);
                var sasUrl = await GetSasUrl(shareFileClient);
                pictures.Add(new() { Name = fileSharee.Name, SasUrl = sasUrl });
            }
            return pictures;
        }

        public async Task<ShareFileDownloadInfo> GetPictureByNameAsync(string name)
        {
            var fileClient = await GetUserShareFileClientAsync(FileDirectoryConstants.Pictures, name);
            var downloadResponse = await fileClient.DownloadAsync();
            return downloadResponse.Value;
        }

        public async Task UploadFile(IFormFile file)
        {
            var directoryClient = await GetUserDirectoryClient(FileDirectoryConstants.Pictures);

            string clientFileName = Guid.NewGuid().ToString() + file.FileName;
            var shareFileClientResponse = await directoryClient.CreateFileAsync(clientFileName, file.Length);

            using var fs = shareFileClientResponse.Value.OpenWrite(false, 0);
            await file.CopyToAsync(fs);

            await GenerateSasUrl(shareFileClientResponse.Value);
        }

        public async Task<bool> DeleteFile(string name)
        {
            var blobClient = await GetUserShareFileClientAsync(FileDirectoryConstants.Pictures, name);
            var result = await blobClient.DeleteAsync();
            return !result.IsError;
        }
        private async Task<ShareDirectoryClient> GetUserDirectoryClient(string directoryName)
        {
            ShareClient shareClient = new(blobOptions.Value.ConnectionString, blobOptions.Value.FileShare);
            await shareClient.GetDirectoryClient(Path.Combine(UserName, directoryName)).CreateIfNotExistsAsync();
            return shareClient.GetDirectoryClient(UserName);
        }

        private async Task<ShareFileClient> GetUserShareFileClientAsync(string directoryName, string name)
        {
            var directoryClient = await GetUserDirectoryClient(directoryName);
            return directoryClient.GetFileClient(name);
        }

        private async Task<string> GetSasUrl(ShareFileClient shareFileClient)
        {
            var filePropertiesResposne = await shareFileClient.GetPropertiesAsync();
            var metaData = filePropertiesResposne.Value.Metadata;
            var expiresOnValue = metaData[PictureConstants.PictureSasExpiresOnKey].FromBase64();
            if (DateTimeOffset.Parse(expiresOnValue) < DateTimeOffset.Now)
            {
                return await GenerateSasUrl(shareFileClient);
            }
            if (!metaData.TryGetValue(PictureConstants.PictureSasUriKey, out string? uri))
            {
                return await GenerateSasUrl(shareFileClient);
            }

            return uri.FromBase64();
        }


        private async Task<string> GenerateSasUrl(ShareFileClient shareFileClient)
        {
            if (!shareFileClient.CanGenerateSasUri)
            {
                throw new ValidationException("SAS generation is not allowed");
            }
            var expriresOnDate = DateTime.UtcNow.AddHours(blobOptions.Value.SasExpiryInHours);
            var expiresOnDateTimeOffset = new DateTimeOffset(expriresOnDate);
            var uri = shareFileClient.GenerateSasUri(ShareFileSasPermissions.Read, expiresOnDateTimeOffset);

            //We use base64 conversions, because metadata could not contains non-ASCII characters
            var absoluteUriBase64 = uri.AbsoluteUri.ToBase64();
            var metadata = new Dictionary<string, string>()
            {
                {
                    PictureConstants.PictureSasUriKey, absoluteUriBase64
                },
                {
                    PictureConstants.PictureSasExpiresOnKey, expiresOnDateTimeOffset.ToString().ToBase64()
                }
            };
            await shareFileClient.SetMetadataAsync(metadata);

            return absoluteUriBase64;
        }
    }
}
