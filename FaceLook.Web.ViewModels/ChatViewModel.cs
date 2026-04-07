using System.ComponentModel.DataAnnotations;

namespace FaceLook.Web.ViewModels
{
    public class ChatViewModel: BaseNamedViewModel
    {
        [MinLength(2)]
        public virtual ICollection<string> ParticipantIds { get; set; } = null!;
        public virtual ICollection<MessageViewModel> Messages { get; set; } = null!;
    }
}
