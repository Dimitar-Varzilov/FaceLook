namespace FaceLook.Data.Entities
{
    public class BaseModifiedEntity : BaseEntity
    {
        public required Guid ModifiedBy { get; set; }
    }
}
