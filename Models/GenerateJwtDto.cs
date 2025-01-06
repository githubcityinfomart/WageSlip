namespace MyPaySlipLive.Models
{
    public class GenerateJwtDto
    {
        public string Name { get; set; } = null!;
        public string Role { get; set; } = null!;
        public string CompanyCode { get; set; } = null!;
        public string CompanyName { get; set; } = null!;
        public string EmployeeCode { get; set; } = null!;
       
    }
}
