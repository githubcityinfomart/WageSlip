using MyPaySlipLive.Models.AdminModel;

namespace MyPaySlipLive.Models
{
    public class ExportSlipViewModel
    {
        public List<GetComapanyCode> CompanyCodes { get; set; }
        public List<GetCompanyName> CompanyNames { get; set; }
        public List<UserDetailViewModel> UserDetail { get; set; }
    }
    public class GetComapanyCode
    {
        public string CompanyCodes { get; set; }
    }

    public class GetCompanyName
    {
        public string CompanyNames { get; set; }
    }
}
