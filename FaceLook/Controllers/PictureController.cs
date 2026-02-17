using FaceLook.Common.Constants;
using FaceLook.Services.Extensions;
using FaceLook.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FaceLook.Web.Controllers
{
    [Authorize]
    public class PicturesController(IPictureService pictureService) : Controller
    {
        public async Task<ViewResult> Index()
        {
            var userId = User.GetUserId();
            var pictures = await pictureService.GetUserPicturesAsync(userId);
            return View(pictures);
        }

        public async Task<IActionResult> Upload()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> UploadPicture(IFormFile picture)
        {
            if (picture == null || picture.Length == 0)
                return RedirectWithMessage("No file selected.");

            var extension = Path.GetExtension(picture.FileName).ToLowerInvariant();
            if (!PictureConstants.AllowedExtensions.Contains(extension))
                return RedirectWithMessage("Invalid file type.");

            var userId = User.GetUserId();
            await pictureService.UploadPictureAsync(userId, picture);

            return RedirectWithMessage("Picture uploaded successfully.", PictureConstants.PictureMessageKey);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(Guid pictureId)
        {
            if (pictureId == Guid.Empty)
            {
                return RedirectWithMessage("Picture id not found.");
            }
            var userId = User.GetUserId();
            await pictureService.DeletePictureAsync(userId, pictureId);
            return RedirectWithMessage("Picture deleted successfully.", PictureConstants.PictureMessageKey);
        }

        private RedirectToActionResult RedirectWithMessage(string message, string key = PictureConstants.PictureErrorKey, string action = nameof(Index))
        {
            TempData[key] = message;
            return RedirectToAction(action);
        }
    }
}
