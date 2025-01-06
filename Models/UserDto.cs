namespace MyPaySlipLive.Models
{
    public class UserDto
    {
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string Role { get; set; } = null!;
        public Guid CompanyId { get; set; }
        public string Code { get; set; } = null!;
        public decimal Salary { get; set; }
        public DateTime Date { get; set; }
    }
}
