namespace FaceLook.Entities
{
    public class BaseEntity
    {
        public required Guid Id { get; set; }
        public required DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? ModifiedAt { get; set; }
        public required string ModifiedBy { get; set; }
        public required bool IsDeleted { get; set; }
    }
}
