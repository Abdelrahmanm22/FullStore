using System.ComponentModel.DataAnnotations;

namespace Store.Models
{
    public class PasswordViewModel
    {
        [Required(ErrorMessage = "The Current Password field is required"), MaxLength(100)]
        public string CurrentPassword { get; set; } = "";
        [Required(ErrorMessage = "The New Password field is required"), MaxLength(100)]
        public string NewPassword { get; set; } = "";
        [Required(ErrorMessage = "The Confirm Password field is required")]
        [Compare("NewPassword", ErrorMessage = "Confirm Password and Password don't match")]
        public string ConfirmPassword { get; set; } = "";
    }
}
