namespace MyPaySlipLive.Models
{
    public class CompanyViewModel
    {
        public CompanyDto CompanyOBJ { get; set; } = new CompanyDto();
        public List<CompanyDto> CompanyList { get; set; } = new List<CompanyDto>();
        public int TotalPages { get; set; }
        public int CurrentPage { get; set; }
        public int CurrentPageSize { get; set; }
    }
}
