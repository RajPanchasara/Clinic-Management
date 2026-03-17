using System.Text.Json.Serialization;

namespace FrontEnd_Exam_2.Models
{
    public class Prescription
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("medicines")]
        public List<PrescriptionMedicine> Medicines { get; set; } = new();

        [JsonPropertyName("notes")]
        public string Notes { get; set; } = string.Empty;

        [JsonPropertyName("appointmentId")]
        public int AppointmentId { get; set; }

        [JsonPropertyName("createdAt")]
        public string CreatedAt { get; set; } = string.Empty;
    }

    public class PrescriptionMedicine
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("dosage")]
        public string Dosage { get; set; } = string.Empty;

        [JsonPropertyName("duration")]
        public string Duration { get; set; } = string.Empty;
    }
}
