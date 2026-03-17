using System.Net;
using System.Text;
using System.Text.Json;
using FrontEnd_Exam_2.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FrontEnd_Exam_2.Controllers
{
    public class AdminController : Controller
    {
        private const string BaseApiUrl = "https://cmsback.sampaarsh.cloud";

        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        private readonly IHttpClientFactory _httpClientFactory;

        public AdminController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        private bool IsAdmin() =>
            HttpContext.Session.GetString("UserRole") == "admin";

        private HttpClient GetAuthClient()
        {
            var client = _httpClientFactory.CreateClient();
            var token = HttpContext.Session.GetString("JwtToken");
            client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            return client;
        }

        [HttpGet("/Admin/Dashboard")]
        public async Task<IActionResult> Dashboard()
        {
            if (!IsAdmin())
            {
                return RedirectToAction("Login", "Login");
            }

            try
            {
                var client = GetAuthClient();
                using var response = await client.GetAsync($"{BaseApiUrl}/admin/clinic");
                var body = await response.Content.ReadAsStringAsync();

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var clinic = JsonSerializer.Deserialize<ClinicInfo>(body, JsonOptions);
                    if (clinic is not null)
                    {
                        ViewBag.ClinicId = clinic.Id;
                        ViewBag.ClinicName = clinic.Name;
                        ViewBag.ClinicCode = clinic.Code;
                        ViewBag.UserCount = clinic.UserCount;
                        ViewBag.AppointmentCount = clinic.AppointmentCount;
                        ViewBag.QueueCount = clinic.QueueCount;
                        ViewBag.CreatedAt = clinic.CreatedAt;
                    }

                    return View();
                }

                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    return RedirectToAction("Login", "Login");
                }

                if (response.StatusCode == HttpStatusCode.Forbidden)
                {
                    ViewBag.Error = "Access denied.";
                    return View();
                }

                if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    ViewBag.Error = "Clinic not found.";
                    return View();
                }

                return View();
            }
            catch (Exception)
            {
                ViewBag.Error = "Server error.";
                return View();
            }
        }

        [HttpGet("/Admin/Users")]
        public async Task<IActionResult> Users()
        {
            if (!IsAdmin())
            {
                return RedirectToAction("Login", "Login");
            }

            try
            {
                var client = GetAuthClient();
                using var response = await client.GetAsync($"{BaseApiUrl}/admin/users");
                var body = await response.Content.ReadAsStringAsync();

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var users = JsonSerializer.Deserialize<List<User>>(body, JsonOptions) ?? new List<User>();
                    ViewBag.Users = users;
                    return View();
                }

                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    return RedirectToAction("Login", "Login");
                }

                if (response.StatusCode == HttpStatusCode.Forbidden)
                {
                    ViewBag.Error = "Access denied.";
                    return View();
                }

                return View();
            }
            catch (Exception)
            {
                ViewBag.Error = "Server error.";
                return View();
            }
        }

        [HttpGet("/Admin/CreateUser")]
        public async Task<IActionResult> CreateUser()
        {
            await Task.CompletedTask;

            if (!IsAdmin())
            {
                return RedirectToAction("Login", "Login");
            }

            return View();
        }

        [HttpPost("/Admin/CreateUser")]
        public async Task<IActionResult> CreateUser(string name, string email, string password, string role, string phone)
        {
            if (!IsAdmin())
            {
                return RedirectToAction("Login", "Login");
            }

            if (string.IsNullOrWhiteSpace(name) || name.Trim().Length < 3)
            {
                ViewBag.Error = "Name must be at least 3 characters.";
                return View();
            }

            if (string.IsNullOrWhiteSpace(email))
            {
                ViewBag.Error = "Email is required.";
                return View();
            }

            if (string.IsNullOrWhiteSpace(password) || password.Length < 6)
            {
                ViewBag.Error = "Password must be at least 6 characters.";
                return View();
            }

            if (role != "doctor" && role != "receptionist" && role != "patient")
            {
                ViewBag.Error = "Invalid role selected.";
                return View();
            }

            try
            {
                var client = GetAuthClient();
                var request = new User
                {
                    Name = name,
                    Email = email,
                    Password = password,
                    Role = role,
                    Phone = phone
                };

                var json = JsonSerializer.Serialize(request);
                using var content = new StringContent(json, Encoding.UTF8, "application/json");
                using var response = await client.PostAsync($"{BaseApiUrl}/admin/users", content);
                var body = await response.Content.ReadAsStringAsync();

                if (response.StatusCode == HttpStatusCode.Created)
                {
                    TempData["Success"] = "User created successfully!";
                    return RedirectToAction("Users");
                }

                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    return RedirectToAction("Login", "Login");
                }

                if (response.StatusCode == HttpStatusCode.Forbidden)
                {
                    ViewBag.Error = "Access denied.";
                    return View();
                }

                var apiError = JsonSerializer.Deserialize<ErrorViewModel>(body, JsonOptions);
                ViewBag.Error = apiError?.Error ?? "Server error.";
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
