namespace FaceLook.Web.ViewModels
{
    public record PictureViewModel : ShareFileItemViewModel
    {
        public required Guid Id { get; set; }
    }
}
