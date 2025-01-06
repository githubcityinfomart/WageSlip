using System.ComponentModel.DataAnnotations;

namespace MyPaySlipLive.Models.AdminModel
{
    public class ResetPasswordViewModel
    {

        [Required(ErrorMessage = "Company Code is required.")]
        public string CompanyCode { get; set; } = null!;

        [Required(ErrorMessage = "Employee Code is required.")]
        public string EmployeeCode { get; set; } = null!;

        [Required(ErrorMessage = "Current Password is required.")]
        [DataType(DataType.Password)]
        public string CurrentPassword { get; set; } = null!;

        [Required(ErrorMessage = "New Password is required.")]
        [DataType(DataType.Password)]
        [MinLength(6, ErrorMessage = "New Password must be at least 6 characters long.")]
        public string NewPassword { get; set; } = null!;

        [Required(ErrorMessage = "Confirm Password is required.")]
        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; } = null!;
    }
}
