using System.ComponentModel.DataAnnotations;

namespace FaceLook.Web.ViewModels
{
    public class SendFriendRequestViewModel
    {
        [Required]
        [EmailAddress]
        public required string AddresseeEmail { get; set; }
    }

    public class FriendsPageViewModel
    {
        public IEnumerable<FriendViewModel> AcceptedFriends { get; set; } = [];
        public IEnumerable<FriendViewModel> PendingRequests { get; set; } = [];
        public IEnumerable<FriendViewModel> SentRequests { get; set; } = [];
        public SendFriendRequestViewModel SendRequest { get; set; } = new() { AddresseeEmail = string.Empty };
    }
}
