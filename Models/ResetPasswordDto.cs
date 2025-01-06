namespace MyPaySlipLive.Models
{
    public class ResetPasswordDto
    {
        public string Token { get; set; } = null!;
        public string NewPassword { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public string OldPassword { get; set; } = null!;
        public Guid EmployeeId { get; set; }  
    }
}
