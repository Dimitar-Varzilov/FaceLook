namespace FaceLook.ViewModels
{
    public class BaseViewModel
    {
        public required Guid Id { get; set; }
        public required DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? ModifiedAt { get; set; }
        public required string ModifiedBy { get; set; }
    }
}
