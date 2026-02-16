using FaceLook.Common.Constants;
using FaceLook.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FaceLook.Web.Controllers
{
    [Authorize]
    public class PicturesController(IFileShareService fileShareService) : Controller
    {
        public async Task<ViewResult> Index()
        {
            var pictureNames = await fileShareService.GetCurrentUserPictures();
            return View(pictureNames);
        }

        public async Task<FileStreamResult> GetPicture([FromQuery] string pictureName)
        {
            var picture = await fileShareService.GetPictureByNameAsync(pictureName);
            return File(picture.Content, picture.ContentType);
        }
        public async Task<IActionResult> Upload()
        {
            return View();
        }

        public async Task<IActionResult> UploadPicture(IFormFile picture)
        {
            if (picture == null || picture.Length == 0)
                return BadRequest("No file selected.");

            var extension = Path.GetExtension(picture.FileName).ToLowerInvariant();

            if (!PictureConstants.AllowedExtensions.Contains(extension))
                return BadRequest("Invalid file type.");

            await fileShareService.UploadFile(picture);

            TempData[PictureConstants.PictureMessageKey] = "Picture uploaded sucessfully";

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete([FromQuery] string pictureName)
        {
            await fileShareService.DeleteFile(pictureName);
            return RedirectToAction(nameof(Index));
        }
    }
}
