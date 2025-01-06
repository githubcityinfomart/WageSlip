using System.ComponentModel.DataAnnotations;

namespace MyPaySlipLive.Models
{
    public class UserPasswordResetModel
    {
        [Required(ErrorMessage = "New Password is required.")]
        [MinLength(6, ErrorMessage = "New Password must be at least 6 characters long.")]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }

        [Required(ErrorMessage = "Confirm Password is required.")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }

        public string PhoneNumber { get; set; }
        public Guid EmployeeId { get; set; }
    }
}
