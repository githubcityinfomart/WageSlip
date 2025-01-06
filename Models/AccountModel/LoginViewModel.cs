using System.ComponentModel.DataAnnotations;

namespace MyPaySlipLive.Models.AccountModel
{
    public class LoginViewModel
    {

         
        public string? CompanyCode { get; set; }

        [Required(ErrorMessage = "Username is required")]
        public string Username { get; set; } = null!;

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = null!;

        public bool RememberMe { get; set; }
    }
}
