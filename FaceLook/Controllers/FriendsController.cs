using FaceLook.Common.Constants;
using FaceLook.Services.Extensions;
using FaceLook.Services.Interfaces;
using FaceLook.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FaceLook.Web.Controllers
{
    [Authorize]
    public class FriendsController(IFriendService friendService) : Controller
    {
        public async Task<IActionResult> Index()
        {
            try
            {
                var model = await friendService.GetFriendsPageAsync(User.GetUserId());
                return View(model);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendRequest(SendFriendRequestViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    TempData[ErrorConstants.ErrorKey] = "Please provide a valid email address";
                    return RedirectToAction(nameof(Index));
                }

                await friendService.SendFriendRequestAsync(User.GetUserId(), model.AddresseeEmail);
                TempData["SuccessMessage"] = $"Friend request sent to {model.AddresseeEmail}";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Accept(Guid id)
        {
            try
            {
                await friendService.AcceptFriendRequestAsync(User.GetUserId(), id);
                TempData["SuccessMessage"] = "Friend request accepted!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Decline(Guid id)
        {
            try
            {
                await friendService.DeclineFriendRequestAsync(User.GetUserId(), id);
                TempData["SuccessMessage"] = "Friend request declined";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Remove(Guid id)
        {
            try
            {
                await friendService.RemoveFriendAsync(User.GetUserId(), id);
                TempData["SuccessMessage"] = "Friend removed";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Block(Guid id)
        {
            try
            {
                await friendService.BlockUserAsync(User.GetUserId(), id);
                TempData["SuccessMessage"] = "User blocked";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        private void SetErrorMessage(string message)
        {
            TempData[ErrorConstants.ErrorKey] = message;
        }

        private RedirectToActionResult HandleException(Exception exception)
        {
            SetErrorMessage(exception.InnerException?.Message ?? exception.Message);
            return RedirectToAction(nameof(Index));
        }
    }
}
