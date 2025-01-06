namespace MyPaySlipLive.Models
{
    public class GetAllCompanyEmployeeDto
    {
        public string? SearchTerm { get; set; }
        public string CompanyCode { get; set; } = null!;
        public Pagination Pagination{ get; set; } = null!;
    }
}
