using System;
using System.Collections.Generic;

namespace MyPaySlipLive.DAL
{
    public partial class TblRole
    {
        public TblRole()
        {
            TblUsers = new HashSet<TblUser>();
        }

        public int Id { get; set; }
        public string Role { get; set; } = null!;
        public DateTime Date { get; set; }

        public virtual ICollection<TblUser> TblUsers { get; set; }
    }
}
