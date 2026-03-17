namespace FrontEnd_Exam_2.Models
{
    public class ErrorViewModel
    {
        public string? RequestId { get; set; }

        public string? Error { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}
