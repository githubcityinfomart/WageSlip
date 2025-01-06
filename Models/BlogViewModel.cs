namespace MyPaySlipLive.Models
{
    public class BlogViewModel
    {
        public BlogDto BlogObJ { get; set; } = new BlogDto();
        public List<BlogDto> BlogList { get; set; } = new List<BlogDto>();
		public int TotalPages { get; set; }
		public int CurrentPage { get; set; }
		public int CurrentPageSize { get; set; }
        public Guid? BlogId { get; set; }
    }

    public class BlogDto
    {
        public Guid Id { get; set; }
        public string? Details { get; set; }
        public DateTime? Date { get; set; }
    }
}
