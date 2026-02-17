namespace FaceLook.Services.Models
{
    public record FileUploadResult(string FileName, string SasUrl, DateTimeOffset SasExpiry);
}
