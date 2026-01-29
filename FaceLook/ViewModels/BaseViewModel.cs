namespace FaceLook.ViewModels
{
    public class BaseViewModel
    {
        public required Guid Id { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? ModifiedAt { get; set; }
    }
}
