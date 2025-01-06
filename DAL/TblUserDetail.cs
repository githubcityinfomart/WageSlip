using System;
using System.Collections.Generic;

namespace MyPaySlipLive.DAL
{
    public partial class TblUserDetail
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string Ecode { get; set; } = null!;
        public string? Name { get; set; }
        public string? Wdays { get; set; }
        public string? Leaves { get; set; }
        public string? Comm { get; set; }
        public string? Advance { get; set; }
        public string? Tax { get; set; }
        public string? Month { get; set; }
        public string? Year { get; set; }
        public string? Company { get; set; }
        public string? Location { get; set; }
        public string? Sex { get; set; }
        public string? Chq { get; set; }
        public string? Bank { get; set; }
        public string? Account { get; set; }
        public string? PfNumber { get; set; }
        public string? EsiNumber { get; set; }
        public string? Category { get; set; }
        public string? SalBasis { get; set; }
        public string? Basic { get; set; }
        public string? Hra { get; set; }
        public string? Ca { get; set; }
        public string? Allow { get; set; }
        public string? Washing { get; set; }
        public string? Total { get; set; }
        public string? Ebasic { get; set; }
        public string? Ehra { get; set; }
        public string? Eca { get; set; }
        public string? Eallow { get; set; }
        public string? ReImb { get; set; }
        public string? Etotal { get; set; }
        public string? Pf { get; set; }
        public string? Esi { get; set; }
        public string? Tded { get; set; }
        public string? Net { get; set; }
        public string? Fpf { get; set; }
        public string? Epf { get; set; }
        public string? PfEmp { get; set; }
        public string? Esie { get; set; }
        public string? Gross { get; set; }
        public string? Remark { get; set; }
        public string? SapCode { get; set; }
        public string? Ttl { get; set; }
        public string? Nhlv { get; set; }
        public string? Others { get; set; }
        public string? Conv { get; set; }
        public string? Tds { get; set; }
        public string? TransferredToAc { get; set; }
        public string? MobileNumber { get; set; }
        public string? TotalDeduction { get; set; }

        public virtual TblUser User { get; set; } = null!;
    }
}
