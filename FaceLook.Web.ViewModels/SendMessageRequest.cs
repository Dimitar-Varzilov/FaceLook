using FaceLook.Common.Validation.Attributes;
using System.ComponentModel.DataAnnotations;

namespace FaceLook.Web.ViewModels
{
    public record SendMessageRequest
    {
        [EmailAddress]
        [Required]
        public required string ReceiverEmail { get; set; }

        [MinLength(1)]
        [Required]
        public required string Content { get; set; }

        [Required]
        [GuidString]
        public required string SenderId { get; set; }
    }
}
