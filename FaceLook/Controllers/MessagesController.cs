using FaceLook.Common.Constants;
using FaceLook.Services;
using FaceLook.Services.Extensions;
using FaceLook.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FaceLook.Web.Controllers
{
    [Authorize]
    public class MessagesController(IMessageService messageService) : Controller
    {
        public async Task<IActionResult> Index()
        {
            try
            {
                var messages = await messageService.GetUserMessagesAsync(User.GetUserId());
                return View(messages);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        public async Task<IActionResult> Create()
        {
            return View();
        }

        public async Task<IActionResult> CreateMessage(SendMessageRequest sendMessageRequest)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(sendMessageRequest);
                }

                var viewModel = await messageService.SendMessageAsync(sendMessageRequest);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        public async Task<IActionResult> Edit(Guid id)
        {
            try
            {
                var message = await messageService.GetMessageById(id);
                if (message is null)
                {
                    TempData[ErrorConstants.ErrorKey] = "Message not found";
                    return RedirectToAction(nameof(this.Index));
                }
                return View(message);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        public async Task<IActionResult> EditMessage(MessageViewModel messageToUpdate)
        {
            if (!ModelState.IsValid)
            {
                return View(messageToUpdate);
            }

            try
            {
                var viewModel = await messageService.UpdateMessageAsync(User.GetUserId(), messageToUpdate);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                var message = await messageService.GetMessageById(id);
                if (message is null)
                {
                    TempData[ErrorConstants.ErrorKey] = "Message not found";
                }
                var isDeleted = await messageService.DeleteMessageAsync(User.GetUserId(), id);

                return RedirectToAction(nameof(this.Index));
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
            return RedirectToAction(nameof(this.Index));
        }
    }
}
