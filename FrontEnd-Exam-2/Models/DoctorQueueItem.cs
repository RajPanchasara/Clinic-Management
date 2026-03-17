using System.Text.Json.Serialization;

namespace FrontEnd_Exam_2.Models
{
    public class DoctorQueueItem
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("tokenNumber")]
        public int TokenNumber { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; } = string.Empty;

        [JsonPropertyName("patientName")]
        public string PatientName { get; set; } = string.Empty;

        [JsonPropertyName("patientId")]
        public int PatientId { get; set; }

        [JsonPropertyName("appointmentId")]
        public int AppointmentId { get; set; }
    }
}
