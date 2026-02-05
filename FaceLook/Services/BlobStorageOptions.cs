namespace FaceLook.Services
{
    public record BlobStorageOptions
    {
        public required string ContainerName { get; set; }
        public required string ConnectionString { get; set; }
    }
}
