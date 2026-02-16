using FaceLook.Web.ViewModels;

namespace FaceLook.Services
{
    public interface IMessageService
    {
        Task<IEnumerable<MessageViewModel>> GetUserMessagesAsync(string userId);
        Task<MessageViewModel?> GetMessageById(Guid messageId);
        Task<MessageViewModel> SendMessageAsync(SendMessageRequest sendMessageRequest);
        Task<MessageViewModel> UpdateMessageAsync(string userId, MessageViewModel messageForUpdating);
        Task<bool> DeleteMessageAsync(string userId, Guid messageId);
    }
}
