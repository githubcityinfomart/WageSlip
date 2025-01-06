namespace MyPaySlipLive.Models.AdminModel
{
    public class SalaryUploadViewModel
    {
        public string SelectedMonth { get; set; }
        public List<UploadedFile> UploadedFiles { get; set; }

        public SalaryUploadViewModel()
        {
            UploadedFiles = new List<UploadedFile>();
        }
    }

    public class UploadedFile
    {
        public string FileName { get; set; }
        public DateTime UploadedDate { get; set; }
    }
}
