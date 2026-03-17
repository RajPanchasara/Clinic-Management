using System.Text.Json.Serialization;

namespace FrontEnd_Exam_2.Models
{
    public class User
    {
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("email")]
        public string Email { get; set; } = string.Empty;

        [JsonPropertyName("password")]
        public string Password { get; set; } = string.Empty;

        [JsonPropertyName("role")]
        public string Role { get; set; } = string.Empty;

        [JsonPropertyName("phone")]
        public string Phone { get; set; } = string.Empty;

        public string CreatedAt { get; set; } = string.Empty;

        public int ClinicId { get; set; }

        public string ClinicName { get; set; } = string.Empty;

        public string ClinicCode { get; set; } = string.Empty;
    }
}
