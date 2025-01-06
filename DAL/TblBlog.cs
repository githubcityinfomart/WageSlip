using System;
using System.Collections.Generic;

namespace MyPaySlipLive.DAL
{
    public partial class TblBlog
    {
        public Guid Id { get; set; }
        public string? Details { get; set; }
        public DateTime? Date { get; set; }
    }
}
