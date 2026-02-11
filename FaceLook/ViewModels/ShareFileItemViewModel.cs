namespace FaceLook.ViewModels
{
    public record ShareFileItemViewModel
    {
        public required string Name { get; set; }
        public required string SasUrl { get; set; }
    }
}
