using FaceLook.Common.Enums;

namespace FaceLook.Web.ViewModels
{
    public class FriendViewModel
    {
        public Guid FriendshipId { get; set; }
        public string FriendId { get; set; } = string.Empty;
        public string FriendEmail { get; set; } = string.Empty;
        public string FriendUserName { get; set; } = string.Empty;
        public FriendshipStatus Status { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public bool IsRequester { get; set; }
    }
}
