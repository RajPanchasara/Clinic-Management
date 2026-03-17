using System.Text.Json.Serialization;

namespace FrontEnd_Exam_2.Models
{
    public class Appointment
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("appointmentDate")]
        public string AppointmentDate { get; set; } = string.Empty;

        [JsonPropertyName("timeSlot")]
        public string TimeSlot { get; set; } = string.Empty;

        [JsonPropertyName("status")]
        public string Status { get; set; } = string.Empty;

        [JsonPropertyName("patientId")]
        public int PatientId { get; set; }

        [JsonPropertyName("clinicId")]
        public int ClinicId { get; set; }

        [JsonPropertyName("createdAt")]
        public string CreatedAt { get; set; } = string.Empty;

        // Nested objects — list endpoint returns queueEntry; details endpoint returns prescription & report
        [JsonPropertyName("queueEntry")]
        public QueueInfo? QueueEntry { get; set; }

        [JsonPropertyName("patient")]
        public PatientInfo? Patient { get; set; }

        [JsonPropertyName("prescription")]
        public Prescription? Prescription { get; set; }

        [JsonPropertyName("report")]
        public Report? Report { get; set; }
    }

    public class PatientInfo
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("phone")]
        public string Phone { get; set; } = string.Empty;
    }

    public class QueueInfo
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
    }
}
