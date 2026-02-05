namespace FaceLook.Services
{
    public interface IBlobService
    {
        Task UploadBlob(IFormFile picture);
    }
}
