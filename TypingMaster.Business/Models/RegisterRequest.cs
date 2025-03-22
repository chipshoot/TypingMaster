using System.ComponentModel.DataAnnotations;

namespace TypingMaster.Business.Models
{
    public class RegisterRequest
    {
        [Required]
        public string AccountName { get; set; } = null!;
        
        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;
        
        [Required]
        [StringLength(100, MinimumLength = 8)]
        public string Password { get; set; } = null!;
        
        [Required]
        [Compare("Password")]
        public string ConfirmPassword { get; set; } = null!;
        
        public string? FirstName { get; set; }
        
        public string? LastName { get; set; }
    }
}
