using FaceLook.Common.Enums;
using System.ComponentModel.DataAnnotations;

namespace FaceLook.Web.ViewModels
{
    public class MessageViewModel : BaseViewModel
    {
        [Required]
        public required string SenderId { get; set; }

        [MinLength(1)]
        [Required]
        public required string ReceiverId { get; set; }

        [MinLength(1)]
        [Required]
        public required string Content { get; set; }

        [Required]
        public required MessageStatus MessageStatus { get; set; }
    }
}
