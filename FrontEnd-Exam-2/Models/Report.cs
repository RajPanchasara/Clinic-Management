using System.Text.Json.Serialization;

namespace FrontEnd_Exam_2.Models
{
    public class Report
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("diagnosis")]
        public string Diagnosis { get; set; } = string.Empty;

        [JsonPropertyName("testRecommended")]
        public string TestRecommended { get; set; } = string.Empty;

        [JsonPropertyName("remarks")]
        public string Remarks { get; set; } = string.Empty;

        [JsonPropertyName("appointmentId")]
        public int AppointmentId { get; set; }

        [JsonPropertyName("createdAt")]
        public string CreatedAt { get; set; } = string.Empty;
    }
}
