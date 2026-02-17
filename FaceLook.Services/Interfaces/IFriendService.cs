using FaceLook.Web.ViewModels;

namespace FaceLook.Services.Interfaces
{
    public interface IFriendService
    {
        Task<FriendsPageViewModel> GetFriendsPageAsync(string currentUserId);
        Task<IEnumerable<FriendViewModel>> GetAcceptedFriendsAsync(string userId);
        Task<FriendViewModel> SendFriendRequestAsync(string requesterId, string addresseeEmail);
        Task<FriendViewModel> AcceptFriendRequestAsync(string currentUserId, Guid friendshipId);
        Task<FriendViewModel> DeclineFriendRequestAsync(string currentUserId, Guid friendshipId);
        Task<bool> RemoveFriendAsync(string currentUserId, Guid friendshipId);
        Task<bool> BlockUserAsync(string currentUserId, Guid friendshipId);
        Task<bool> AreFriendsAsync(string userId1, string userId2);
    }
}
