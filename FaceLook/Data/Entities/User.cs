using Microsoft.AspNetCore.Identity;

namespace FaceLook.Data.Entities
{
    public class User : IdentityUser
    {
        public virtual required ICollection<Message> SentMessages { get; set; }
        public virtual required ICollection<Message> ReceivedMessages { get; set; }
    }
}
