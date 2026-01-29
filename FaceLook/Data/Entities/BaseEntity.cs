namespace FaceLook.Data.Entities
{
    public class BaseEntity
    {
        public required Guid Id { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? ModifiedAt { get; set; }
        public bool IsDeleted { get; set; }
    }
}
