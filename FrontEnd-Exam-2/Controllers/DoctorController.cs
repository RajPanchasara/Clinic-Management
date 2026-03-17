using System.Net;
using System.Text;
using System.Text.Json;
using FrontEnd_Exam_2.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FrontEnd_Exam_2.Controllers
{
    public class DoctorController : Controller
    {
        private const string BaseApiUrl = "https://cmsback.sampaarsh.cloud";

        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        private readonly IHttpClientFactory _httpClientFactory;

        public DoctorController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        private bool IsDoctor() =>
            HttpContext.Session.GetString("UserRole") == "doctor";

        private HttpClient GetAuthClient()
        {
            var client = _httpClientFactory.CreateClient();
            var token = HttpContext.Session.GetString("JwtToken");
            client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            return client;
        }

        // ── Today's Queue ──
        [HttpGet("/Doctor/TodayQueue")]
        public async Task<IActionResult> TodayQueue()
        {
            if (!IsDoctor()) return RedirectToAction("Login", "Login");

            try
            {
                var client = GetAuthClient();
                using var response = await client.GetAsync($"{BaseApiUrl}/doctor/queue");
                var body = await response.Content.ReadAsStringAsync();

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var queue = JsonSerializer.Deserialize<List<DoctorQueueItem>>(body, JsonOptions) ?? new List<DoctorQueueItem>();
                    ViewBag.Queue = queue;
                    return View();
                }

                if (response.StatusCode == HttpStatusCode.Unauthorized)
                    return RedirectToAction("Login", "Login");

                if (response.StatusCode == HttpStatusCode.Forbidden)
                {
                    ViewBag.Error = "Access denied.";
                    return View();
                }

                ViewBag.Error = "Failed to load queue.";
                return View();
            }
            catch (Exception)
            {
                ViewBag.Error = "Server error.";
                return View();
            }
        }

        // ── Add Prescription (GET form) ──
        [HttpGet("/Doctor/AddPrescription/{appointmentId}")]
        public async Task<IActionResult> AddPrescription(int appointmentId)
        {
            await Task.CompletedTask;
            if (!IsDoctor()) return RedirectToAction("Login", "Login");
            ViewBag.AppointmentId = appointmentId;
            return View();
        }

        // ── Add Prescription (POST) ──
        [HttpPost("/Doctor/AddPrescription/{appointmentId}")]
        public async Task<IActionResult> AddPrescription(int appointmentId, string medicinesJson, string notes)
        {
            if (!IsDoctor()) return RedirectToAction("Login", "Login");

            ViewBag.AppointmentId = appointmentId;

            if (string.IsNullOrWhiteSpace(medicinesJson))
            {
                ViewBag.Error = "At least one medicine is required.";
                return View();
            }

            try
            {
                var medicines = JsonSerializer.Deserialize<List<PrescriptionMedicine>>(medicinesJson, JsonOptions);
                if (medicines == null || medicines.Count == 0)
                {
                    ViewBag.Error = "At least one medicine is required.";
                    return View();
                }

                var client = GetAuthClient();
                var payload = new { medicines, notes = notes ?? string.Empty };
                var json = JsonSerializer.Serialize(payload);
                using var content = new StringContent(json, Encoding.UTF8, "application/json");
                using var response = await client.PostAsync($"{BaseApiUrl}/prescriptions/{appointmentId}", content);
                var body = await response.Content.ReadAsStringAsync();

                if (response.StatusCode == HttpStatusCode.Created)
                {
                    TempData["Success"] = "Prescription added successfully!";
                    return RedirectToAction("TodayQueue");
                }

                if (response.StatusCode == HttpStatusCode.Unauthorized)
                    return RedirectToAction("Login", "Login");

                var apiError = JsonSerializer.Deserialize<ErrorViewModel>(body, JsonOptions);
                ViewBag.Error = apiError?.Error ?? "Failed to add prescription.";
                return View();
            }
            catch (Exception)
            {
                ViewBag.Error = "Server error.";
                return View();
            }
        }

        // ── Add Report (GET form) ──
        [HttpGet("/Doctor/AddReport/{appointmentId}")]
        public async Task<IActionResult> AddReport(int appointmentId)
        {
            await Task.CompletedTask;
            if (!IsDoctor()) return RedirectToAction("Login", "Login");
            ViewBag.AppointmentId = appointmentId;
            return View();
        }

        // ── Add Report (POST) ──
        [HttpPost("/Doctor/AddReport/{appointmentId}")]
        public async Task<IActionResult> AddReport(int appointmentId, string diagnosis, string testRecommended, string remarks)
        {
            if (!IsDoctor()) return RedirectToAction("Login", "Login");

            ViewBag.AppointmentId = appointmentId;

            if (string.IsNullOrWhiteSpace(diagnosis))
            {
                ViewBag.Error = "Diagnosis is required.";
                return View();
            }

            try
            {
                var client = GetAuthClient();
                // Ensure optional fields are sent as empty strings rather than nulls if not provided
                var payload = new { 
                    diagnosis, 
                    testRecommended = testRecommended ?? string.Empty, 
                    remarks = remarks ?? string.Empty 
                };
                var json = JsonSerializer.Serialize(payload);
                using var content = new StringContent(json, Encoding.UTF8, "application/json");
                using var response = await client.PostAsync($"{BaseApiUrl}/reports/{appointmentId}", content);
                var body = await response.Content.ReadAsStringAsync();

                if (response.StatusCode == HttpStatusCode.Created)
                {
                    TempData["Success"] = "Report added successfully!";
                    return RedirectToAction("TodayQueue");
                }

                if (response.StatusCode == HttpStatusCode.Unauthorized)
                    return RedirectToAction("Login", "Login");

                var apiError = JsonSerializer.Deserialize<ErrorViewModel>(body, JsonOptions);
                ViewBag.Error = apiError?.Error ?? "Failed to add report.";
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
