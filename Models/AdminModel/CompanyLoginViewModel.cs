using System.ComponentModel.DataAnnotations;

namespace MyPaySlipLive.Models.AdminModel
{
    public class CompanyLoginViewModel
    {
        [Required(ErrorMessage = "Company Code is required")]
        public string CompanyCode { get; set; }

        [Required(ErrorMessage = "Username is required")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
