using System.Text.Json.Serialization;

namespace FrontEnd_Exam_2.Models
{
    public class QueueEntry
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("tokenNumber")]
        public int TokenNumber { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; } = string.Empty;

        [JsonPropertyName("queueDate")]
        public string QueueDate { get; set; } = string.Empty;

        [JsonPropertyName("appointmentId")]
        public int AppointmentId { get; set; }

        [JsonPropertyName("appointment")]
        public QueueAppointment? Appointment { get; set; }
    }

    public class QueueAppointment
    {
        [JsonPropertyName("patient")]
        public PatientInfo? Patient { get; set; }
    }
}
