namespace FrontEnd_Exam_2.Models
{
    public class ClinicInfo
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Code { get; set; } = string.Empty;

        public string CreatedAt { get; set; } = string.Empty;

        public int UserCount { get; set; }

        public int AppointmentCount { get; set; }

        public int QueueCount { get; set; }
    }
}
