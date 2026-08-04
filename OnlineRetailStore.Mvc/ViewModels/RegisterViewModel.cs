using System.ComponentModel.DataAnnotations;

namespace OnlineRetailStore.Mvc.ViewModels
{
    public class RegisterViewModel
    {
        [Required]
        [StringLength(100)]
        public string Username { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(255)]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "Password must be at least 8 characters long")]
        public string Password { get; set; }

        [Display(Name = "Account type")]
        public string Role { get; set; } = "User";

        [StringLength(30)]
        public string Phone { get; set; }

        [StringLength(500)]
        public string Address { get; set; }
    }
}
