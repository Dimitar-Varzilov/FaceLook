using FaceLook.Common.Enums;

namespace FaceLook.Data.Entities
{
    public class Message : BaseEntity
    {
        public required Guid ChatId { get; set; }
        public virtual Chat Chat { get; set; } = null!;

        public required string SenderId { get; set; }
        public virtual User Sender { get; set; } = null!;

        public required string Content { get; set; }
        public required MessageStatus MessageStatus { get; set; }
    }
}
