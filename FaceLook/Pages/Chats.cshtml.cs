using FaceLook.Services.Extensions;
using FaceLook.Services.Interfaces;
using FaceLook.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FaceLook.Web.Pages
{
    [Authorize]
    public class ChatsModel(
        IFriendService friendService,
        IChatService chatService,
        IMessageService messageService
        ) : PageModel
    {
        public IEnumerable<FriendViewModel>? Friends;
        public IEnumerable<ChatViewModel>? Chats;
        public bool IsCreating;
        public string? ErrorMessage;
        public string UserId = string.Empty;

        public async Task OnGetAsync()
        {
            UserId = User.GetUserId();
            await LoadDataAsync();
        }

        [BindProperty]
        public List<string>? ParticipantIds { get; set; }

        [BindProperty]
        public string? ChatName { get; set; }

        [BindProperty]
        public Guid ChatId { get; set; }

        [BindProperty]
        public string? MessageContent { get; set; }

        [BindProperty]
        public Guid MessageId { get; set; }

        [BindProperty]
        public MessageViewModel? MessageToEdit { get; set; }

        public async Task<IActionResult> OnPostEditMessageAsync()
        {
            UserId = User.GetUserId();
            ErrorMessage = null;

            try
            {
                if (MessageToEdit is null)
                    throw new ArgumentException("Message data is required");

                if (MessageToEdit.Id == Guid.Empty)
                    throw new ArgumentException("MessageId is required");

                if (string.IsNullOrWhiteSpace(MessageToEdit.Content))
                    throw new ArgumentException("Message cannot be empty");

                var updatedMessage = await messageService.UpdateMessageAsync(UserId, MessageToEdit);
                await LoadDataAsync();
                return RedirectToPage();
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.InnerException?.Message ?? ex.Message;
                await LoadDataAsync();
                return Page();
            }
        }

        public async Task<IActionResult> OnPostDeleteMessageAsync()
        {
            UserId = User.GetUserId();
            ErrorMessage = null;

            try
            {
                if (MessageId == Guid.Empty)
                    throw new ArgumentException("MessageId is required");

                await messageService.DeleteMessageAsync(UserId, MessageId);
                await LoadDataAsync();
                return RedirectToPage();
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.InnerException?.Message ?? ex.Message;
                await LoadDataAsync();
                return Page();
            }
        }

        public async Task<IActionResult> OnPostCreateChatAsync()
        {
            UserId = User.GetUserId();
            ErrorMessage = null;
            IsCreating = true;

            try
            {
                var participantSet = new HashSet<string>(ParticipantIds ?? [])
                {
                    UserId
                };
                var chat = await chatService.CreateChatAsync(UserId, participantSet);

                if (!string.IsNullOrEmpty(ChatName) && ChatName.Length > 0)
                {
                    await chatService.UpdateChatNameAsync(UserId, chat.Id, ChatName);
                }

                await LoadDataAsync();
                ParticipantIds = null;
                ChatName = null;
                return RedirectToPage();
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.InnerException?.Message ?? ex.Message;
                await LoadDataAsync();
                return Page();
            }
            finally
            {
                IsCreating = false;
            }
        }

        public async Task<IActionResult> OnPostRenameChatAsync()
        {
            UserId = User.GetUserId();
            ErrorMessage = null;

            try
            {
                if (ChatId == Guid.Empty)
                    throw new ArgumentException("ChatId is required");

                await chatService.UpdateChatNameAsync(UserId, ChatId, ChatName ?? string.Empty);
                await LoadDataAsync();
                return RedirectToPage();
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.InnerException?.Message ?? ex.Message;
                await LoadDataAsync();
                return Page();
            }
        }

        public async Task<IActionResult> OnPostSendMessageAsync()
        {
            UserId = User.GetUserId();
            ErrorMessage = null;

            try
            {
                if (ChatId == Guid.Empty)
                    throw new ArgumentException("ChatId is required");

                if (string.IsNullOrWhiteSpace(MessageContent))
                    throw new ArgumentException("Message cannot be empty");

                var sendMessageRequest = new SendMessageRequest
                {
                    ChatId = ChatId,
                    Content = MessageContent,
                    SenderId = UserId
                };

                await messageService.SendMessageAsync(sendMessageRequest);

                await LoadDataAsync();
                MessageContent = null;
                return RedirectToPage();
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.InnerException?.Message ?? ex.Message;
                await LoadDataAsync();
                return Page();
            }
        }

        private async Task LoadDataAsync()
        {
            Friends = await friendService.GetAcceptedFriendsAsync(UserId);
            Chats = await chatService.GetUserChatsAsync(UserId);
        }
    }
}
