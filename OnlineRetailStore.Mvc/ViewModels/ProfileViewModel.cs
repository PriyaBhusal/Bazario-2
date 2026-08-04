using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace OnlineRetailStore.Mvc.ViewModels
{
    public class ChangePasswordViewModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Current password")]
        public string OldPassword { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "Password must be at least 8 characters long")]
        public string NewPassword { get; set; }
    }

    public class ProfileViewModel
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Username { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(255)]
        public string Email { get; set; }

        [StringLength(30)]
        public string Phone { get; set; }

        [StringLength(500)]
        public string Address { get; set; }

        public string Role { get; set; }
        public string MemberSince { get; set; }
        public List<BadgeViewModel> Badges { get; set; } = new List<BadgeViewModel>();
    }
}
