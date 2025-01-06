namespace MyPaySlipLive.Models.AdminModel
{
    public class SalaryListViewModel
    {

        public string CompanyName { get; set; } = null!;
        public string User { get; set; } = null!;
        public string SapCode { get; set; } = null!;
        public string Designation { get; set; } = null!;
        public string PfNumber { get; set; } = null!;
        public string EsiNumber { get; set; } = null!;
        public string Location { get; set; } = null!;
        public decimal Basic { get; set; }
        public decimal Ttl { get; set; }
        public int Days { get; set; }
        public decimal NhLv { get; set; }
        public decimal Reimb { get; set; }
        public decimal Hra { get; set; }
        public decimal Conv { get; set; }
        public decimal Others { get; set; }
        public decimal Total { get; set; }
        public string Month { get; set; } = null!;
        public decimal Pf { get; set; }
        public decimal Esi { get; set; }
        public decimal Adv { get; set; }
        public decimal Tds { get; set; }
        public decimal TransferredToAc { get; set; }
        public decimal VideCheck { get; set; }
        public decimal NetInr { get; set; }

    }
}
