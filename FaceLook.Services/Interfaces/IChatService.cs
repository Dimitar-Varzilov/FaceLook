using FaceLook.Web.ViewModels;

namespace FaceLook.Services.Interfaces
{
    public interface IChatService
    {
        Task<ChatViewModel?> GetChatByIdAsync(Guid chatId);
        Task<IEnumerable<ChatViewModel>> GetUserChatsAsync(string userId);
        Task<ChatViewModel> CreateChatAsync(string userId, ISet<string> participants);
        Task<bool> UpdateChatNameAsync(string userId, Guid chatId, string name);
    }
}