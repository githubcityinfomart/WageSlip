namespace MyPaySlipLive.Models
{
    public class Response
    {
        public Response(string status, string message, int totalRecords, string result)
        {
            TotalRecords = totalRecords;
            Status = status;
            Message = message;
            Result = result;
        }
        public Response(string status, string message, string result)
        {
            Status = status;
            Message = message;
            Result = result;
        }
        public Response(string status, string message)
        {
            Status = status;
            Message = message;
        }

        public Response()
        {
        }

        public int TotalRecords { get; set; }
        public string Status { get; set; }
        public string Message { get; set; }
        public string Result { get; set; }

    }

}
