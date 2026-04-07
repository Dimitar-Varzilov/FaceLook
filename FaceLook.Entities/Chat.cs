namespace FaceLook.Data.Entities
{
    public class Chat : BaseNamedEntity
    {
        public virtual IList<string> ParticipantIds { get; set; } = null!;
        public virtual IList<User> Participants { get; set; } = null!;
        public virtual IList<Guid> MessageIds { get; set; } = null!;
        public virtual IList<Message> Messages { get; set; } = null!;
    }
}
