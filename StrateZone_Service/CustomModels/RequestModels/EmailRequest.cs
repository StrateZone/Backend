using System.ComponentModel.DataAnnotations;

namespace StrateZone_Service.CustomModels.RequestModels
{
    public class EmailRequest
    {
        [Required]
        [EmailAddress]
        public string? ToEmail { get; set; }

        [Required]
        public string? Subject { get; set; }

        [Required]
        public string? Content { get; set; }
    }
}
