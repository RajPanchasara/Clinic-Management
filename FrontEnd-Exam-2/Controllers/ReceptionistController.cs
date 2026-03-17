using System.Net;
using System.Text;
using System.Text.Json;
using FrontEnd_Exam_2.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FrontEnd_Exam_2.Controllers
{
    public class ReceptionistController : Controller
    {
        private const string BaseApiUrl = "https://cmsback.sampaarsh.cloud";

        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        private readonly IHttpClientFactory _httpClientFactory;

        public ReceptionistController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        private bool IsReceptionist() =>
            HttpContext.Session.GetString("UserRole") == "receptionist";

        private HttpClient GetAuthClient()
        {
            var client = _httpClientFactory.CreateClient();
            var token = HttpContext.Session.GetString("JwtToken");
            client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            return client;
        }

        // ── Daily Queue ──
        [HttpGet("/Receptionist/Queue")]
        public async Task<IActionResult> Queue(string? date)
        {
            if (!IsReceptionist()) return RedirectToAction("Login", "Login");

            var queryDate = date ?? DateTime.Today.ToString("yyyy-MM-dd");
            ViewBag.SelectedDate = queryDate;

            try
            {
                var client = GetAuthClient();
                using var response = await client.GetAsync($"{BaseApiUrl}/queue?date={queryDate}");
                var body = await response.Content.ReadAsStringAsync();

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var queue = JsonSerializer.Deserialize<List<QueueEntry>>(body, JsonOptions) ?? new List<QueueEntry>();
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

        // ── Update Queue Status ──
        [HttpPost("/Receptionist/UpdateStatus")]
        public async Task<IActionResult> UpdateStatus(int id, string status, string date)
        {
            if (!IsReceptionist()) return RedirectToAction("Login", "Login");

            try
            {
                var client = GetAuthClient();
                var payload = new { status };
                var json = JsonSerializer.Serialize(payload);
                using var content = new StringContent(json, Encoding.UTF8, "application/json");

                var request = new HttpRequestMessage(new HttpMethod("PATCH"), $"{BaseApiUrl}/queue/{id}")
                {
                    Content = content
                };
                using var response = await client.SendAsync(request);
                var body = await response.Content.ReadAsStringAsync();

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    TempData["Success"] = "Status updated successfully!";
                }
                else if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    return RedirectToAction("Login", "Login");
                }
                else
                {
                    var apiError = JsonSerializer.Deserialize<ErrorViewModel>(body, JsonOptions);
                    TempData["Error"] = apiError?.Error ?? "Failed to update status.";
                }

                return RedirectToAction("Queue", new { date });
            }
            catch (Exception)
            {
                TempData["Error"] = "Server error.";
                return RedirectToAction("Queue", new { date });
            }
        }
    }
}
