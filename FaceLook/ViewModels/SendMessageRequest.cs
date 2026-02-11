using System.ComponentModel.DataAnnotations;

namespace FaceLook.ViewModels
{
    public record SendMessageRequest
    {
        [EmailAddress]
        [Required]
        public required string ReceiverEmail { get; set; }

        [MinLength(1)]
        [Required]
        public required string Content { get; set; }
    }
}
