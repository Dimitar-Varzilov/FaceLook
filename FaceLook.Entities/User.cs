using Microsoft.AspNetCore.Identity;

namespace FaceLook.Data.Entities
{
    public class User : IdentityUser
    {
        public virtual required ICollection<Message> SentMessages { get; set; }
        public virtual required ICollection<Message> ReceivedMessages { get; set; }
        public virtual required ICollection<Picture> Pictures { get; set; }
        public virtual required ICollection<Friendship> SentFriendRequests { get; set; }
        public virtual required ICollection<Friendship> ReceivedFriendRequests { get; set; }
    }
}
