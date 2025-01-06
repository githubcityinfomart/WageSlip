using System;
using System.Collections.Generic;

namespace MyPaySlipLive.DAL
{
    public partial class TblCompany
    {
        public TblCompany()
        {
            TblUsers = new HashSet<TblUser>();
        }

        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string CompanyCode { get; set; } = null!;
        public bool IsActive { get; set; }
        public DateTime Date { get; set; }

        public virtual ICollection<TblUser> TblUsers { get; set; }
    }
}
