namespace MyPaySlipLive.Models.AdminModel
{
    public class ManageCompanyViewModel
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public string AdminPassword { get; set; }
        public bool IsActivated { get; set; }

         
        public List<CompanyRecord> Companies { get; set; }

        public ManageCompanyViewModel()
        {
            Companies = new List<CompanyRecord>();
        }
    }
    public class CompanyRecord
    {
        public string Name { get; set; } = null!;
        public string Code { get; set; } = null!;
        public bool IsPaidCustomer { get; set; }
    }
}
