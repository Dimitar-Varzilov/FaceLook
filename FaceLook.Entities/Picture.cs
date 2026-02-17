namespace FaceLook.Data.Entities
{
    public class Picture : BaseNamedEntity
    {
        public required string SasUrl { get; set; }
        public required DateTimeOffset SasExpiry { get; set; }
        public required string UserId { get; set; }
        public required virtual User User { get; set; }

    }
}
