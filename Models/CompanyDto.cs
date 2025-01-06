namespace MyPaySlipLive.Models
{
    public class CompanyDto
    {
        public Guid Id { get; set; }
        public int Sno { get; set; }
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string Code { get; set; } = null!;
        public string UserCode { get; set; } = null!;   
        public string CompanyCode { get; set; } = null!;
        public string AdminPassword { get; set; } = null!;
        public string IsPaidCustomer { get; set; } = null!;
        public bool IsActive { get; set; }
        public bool IsPayed { get; set; }
    }
}
