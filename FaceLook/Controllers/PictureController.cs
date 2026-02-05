using FaceLook.Constants;
using FaceLook.Services;
using FaceLook.Services.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace FaceLook.Controllers
{
    public class PicturesController(IBlobService blobService) : Controller
    {
        public async Task<IActionResult> Upload(IFormFile picture)
        {
            if (picture == null || picture.Length == 0)
                return BadRequest("No file selected.");

            var allowedExtensions = PictureConstants.AllowedExtensions;
            var extension = Path.GetExtension(picture.FileName).ToLowerInvariant();

            if (!allowedExtensions.Contains(extension))
                return BadRequest("Invalid file type.");

            await blobService.UploadBlob(picture);

            TempData[PictureConstants.PictureMessageKey] = "Picture uploaded sucessfully";

            return RedirectToAction(nameof(HomeController.Index), ControllersExtensions.GetControllerName<HomeController>());
        }
    }
}
