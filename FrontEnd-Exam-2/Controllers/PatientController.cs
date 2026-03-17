using System.Net;
using System.Text;
using System.Text.Json;
using FrontEnd_Exam_2.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FrontEnd_Exam_2.Controllers
{
    public class PatientController : Controller
    {
        private const string BaseApiUrl = "https://cmsback.sampaarsh.cloud";

        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        private readonly IHttpClientFactory _httpClientFactory;

        public PatientController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        private bool IsPatient() =>
            HttpContext.Session.GetString("UserRole") == "patient";

        private HttpClient GetAuthClient()
        {
            var client = _httpClientFactory.CreateClient();
            var token = HttpContext.Session.GetString("JwtToken");
            client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            return client;
        }

        // ── Book Appointment (GET form) ──
        [HttpGet("/Patient/BookAppointment")]
        public async Task<IActionResult> BookAppointment()
        {
            await Task.CompletedTask;
            if (!IsPatient()) return RedirectToAction("Login", "Login");
            return View();
        }

        // ── Book Appointment (POST) ──
        [HttpPost("/Patient/BookAppointment")]
        public async Task<IActionResult> BookAppointment(string appointmentDate, string timeSlot)
        {
            if (!IsPatient()) return RedirectToAction("Login", "Login");

            if (string.IsNullOrWhiteSpace(appointmentDate))
            {
                ViewBag.Error = "Appointment date is required.";
                return View();
            }

            if (string.IsNullOrWhiteSpace(timeSlot))
            {
                ViewBag.Error = "Time slot is required.";
                return View();
            }

            try
            {
                var client = GetAuthClient();
                var payload = new { appointmentDate, timeSlot };
                var json = JsonSerializer.Serialize(payload);
                using var content = new StringContent(json, Encoding.UTF8, "application/json");
                using var response = await client.PostAsync($"{BaseApiUrl}/appointments", content);
                var body = await response.Content.ReadAsStringAsync();

                if (response.StatusCode == HttpStatusCode.Created)
                {
                    TempData["Success"] = "Appointment booked successfully!";
                    return RedirectToAction("MyAppointments");
                }

                if (response.StatusCode == HttpStatusCode.Unauthorized)
                    return RedirectToAction("Login", "Login");

                if (response.StatusCode == HttpStatusCode.Forbidden)
                {
                    ViewBag.Error = "Access denied.";
                    return View();
                }

                var apiError = JsonSerializer.Deserialize<ErrorViewModel>(body, JsonOptions);
                ViewBag.Error = apiError?.Error ?? "Failed to book appointment.";
                ViewBag.AppointmentDate = appointmentDate;
                ViewBag.TimeSlot = timeSlot;
                return View();
            }
            catch (Exception)
            {
                ViewBag.Error = "Server error.";
                return View();
            }
        }

        // ── My Appointments ──
        [HttpGet("/Patient/MyAppointments")]
        public async Task<IActionResult> MyAppointments()
        {
            if (!IsPatient()) return RedirectToAction("Login", "Login");

            try
            {
                var client = GetAuthClient();
                using var response = await client.GetAsync($"{BaseApiUrl}/appointments/my");
                var body = await response.Content.ReadAsStringAsync();

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var appointments = JsonSerializer.Deserialize<List<Appointment>>(body, JsonOptions) ?? new List<Appointment>();
                    ViewBag.Appointments = appointments;
                    return View();
                }

                if (response.StatusCode == HttpStatusCode.Unauthorized)
                    return RedirectToAction("Login", "Login");

                ViewBag.Error = "Failed to load appointments.";
                return View();
            }
            catch (Exception)
            {
                ViewBag.Error = "Server error.";
                return View();
            }
        }

        // ── Appointment Details ──
        [HttpGet("/Patient/AppointmentDetails/{id}")]
        public async Task<IActionResult> AppointmentDetails(int id)
        {
            if (!IsPatient()) return RedirectToAction("Login", "Login");

            try
            {
                var client = GetAuthClient();
                using var response = await client.GetAsync($"{BaseApiUrl}/appointments/{id}");
                var body = await response.Content.ReadAsStringAsync();

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var appointment = JsonSerializer.Deserialize<Appointment>(body, JsonOptions);
                    ViewBag.Appointment = appointment;
                    return View();
                }

                if (response.StatusCode == HttpStatusCode.Unauthorized)
                    return RedirectToAction("Login", "Login");

                if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    ViewBag.Error = "Appointment not found.";
                    return View();
                }

                ViewBag.Error = "Failed to load appointment details.";
                return View();
            }
            catch (Exception)
            {
                ViewBag.Error = "Server error.";
                return View();
            }
        }

        // ── My Prescriptions ──
        [HttpGet("/Patient/MyPrescriptions")]
        public async Task<IActionResult> MyPrescriptions()
        {
            if (!IsPatient()) return RedirectToAction("Login", "Login");

            try
            {
                var client = GetAuthClient();
                using var response = await client.GetAsync($"{BaseApiUrl}/prescriptions/my");
                var body = await response.Content.ReadAsStringAsync();

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var prescriptions = JsonSerializer.Deserialize<List<Prescription>>(body, JsonOptions) ?? new List<Prescription>();
                    ViewBag.Prescriptions = prescriptions;
                    return View();
                }

                if (response.StatusCode == HttpStatusCode.Unauthorized)
                    return RedirectToAction("Login", "Login");

                ViewBag.Error = "Failed to load prescriptions.";
                return View();
            }
            catch (Exception)
            {
                ViewBag.Error = "Server error.";
                return View();
            }
        }

        // ── My Reports ──
        [HttpGet("/Patient/MyReports")]
        public async Task<IActionResult> MyReports()
        {
            if (!IsPatient()) return RedirectToAction("Login", "Login");

            try
            {
                var client = GetAuthClient();
                using var response = await client.GetAsync($"{BaseApiUrl}/reports/my");
                var body = await response.Content.ReadAsStringAsync();

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var reports = JsonSerializer.Deserialize<List<Report>>(body, JsonOptions) ?? new List<Report>();
                    ViewBag.Reports = reports;
                    return View();
                }

                if (response.StatusCode == HttpStatusCode.Unauthorized)
                    return RedirectToAction("Login", "Login");

                ViewBag.Error = "Failed to load reports.";
                return View();
            }
            catch (Exception)
            {
                ViewBag.Error = "Server error.";
                return View();
            }
        }
    }
}
