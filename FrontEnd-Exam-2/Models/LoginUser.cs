namespace FrontEnd_Exam_2.Models
{
    public class LoginUser
    {
        public int Id { get; set; }

        public string? Name { get; set; }

        public string? Email { get; set; }

        public string? Role { get; set; }

        public int ClinicId { get; set; }

        public string? ClinicName { get; set; }

        public string? ClinicCode { get; set; }
    }
}
