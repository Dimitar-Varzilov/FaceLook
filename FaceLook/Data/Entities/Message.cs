using FaceLook.Enums;

namespace FaceLook.Data.Entities
{
    public class Message : BaseEntity
    {
        public required string SenderId { get; set; }
        public virtual User Sender { get; set; } = null!;
        public required string ReceiverId { get; set; }
        public virtual User Receiver { get; set; } = null!;
        public required string Content { get; set; }
        public required MessageStatus MessageStatus { get; set; }
    }
}
