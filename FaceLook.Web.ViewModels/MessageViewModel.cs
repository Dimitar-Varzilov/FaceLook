using FaceLook.Common.Enums;
using System.ComponentModel.DataAnnotations;

namespace FaceLook.Web.ViewModels
{
    public class MessageViewModel : BaseViewModel
    {
        [Required]
        public required Guid ChatId { get; set; }

        [Required]
        public required string SenderId { get; set; }

        public string SenderEmail { get; set; } = string.Empty;

        [MinLength(1)]
        [Required]
        public required string Content { get; set; }

        [Required]
        public required MessageStatus MessageStatus { get; set; }
    }
}
