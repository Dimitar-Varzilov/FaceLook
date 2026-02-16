namespace FaceLook.Services.Models
{
    public record FileShareOptions
    {
        public required string FileShare { get; set; }
        public required string ConnectionString { get; set; }
        public required int SasExpiryInHours { get; set; }
    }
}
