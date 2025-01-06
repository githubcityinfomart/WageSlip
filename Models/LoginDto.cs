using System.ComponentModel.DataAnnotations;

namespace MyPaySlipLive.Models
{
    public class LoginDto
    {
        [Required(ErrorMessage = "Username is required")]
        public string Username { get; set; } = null!;
        public string? CompanyCode { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = null!;




        //----------------------------------
        public bool RememberMe { get; set; }
    }
}
