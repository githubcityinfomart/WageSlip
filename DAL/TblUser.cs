using System;
using System.Collections.Generic;

namespace MyPaySlipLive.DAL
{
    public partial class TblUser
    {
        public TblUser()
        {
            TblUserDetails = new HashSet<TblUserDetail>();
        }

        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Email { get; set; }
        public string Password { get; set; } = null!;
        public int Role { get; set; }
        public string? CompanyCode { get; set; }
        public DateTime Date { get; set; }
        public bool? IsActive { get; set; }
        public bool IsPayed { get; set; }
        public string? UserCode { get; set; }
        public string? PhoneNumber { get; set; }

        public virtual TblRole RoleNavigation { get; set; } = null!;
        public virtual ICollection<TblUserDetail> TblUserDetails { get; set; }
    }
}
