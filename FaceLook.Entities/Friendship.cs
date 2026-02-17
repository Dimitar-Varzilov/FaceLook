using FaceLook.Common.Enums;

namespace FaceLook.Data.Entities
{
    public class Friendship : BaseEntity
    {
        public required string RequesterId { get; set; }
        public virtual User Requester { get; set; } = null!;
        public required string AddresseeId { get; set; }
        public virtual User Addressee { get; set; } = null!;
        public required FriendshipStatus Status { get; set; }
    }
}