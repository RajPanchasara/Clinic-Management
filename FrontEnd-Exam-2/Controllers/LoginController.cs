using System.Net;
using System.Text;
using System.Text.Json;
using FrontEnd_Exam_2.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FrontEnd_Exam_2.Controllers
{
    public class LoginController : Controller
    {
        private const string BaseApiUrl = "https://cmsback.sampaarsh.cloud";

        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        private readonly IHttpClientFactory _httpClientFactory;

        public LoginController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [HttpGet("/Login")]
        public async Task<IActionResult> Login()
        {
            await Task.CompletedTask;

            var jwtToken = HttpContext.Session.GetString("JwtToken");
            if (!string.IsNullOrWhiteSpace(jwtToken))
            {
                var role = HttpContext.Session.GetString("UserRole");
                return role switch
                {
                    "admin" => RedirectToAction("Dashboard", "Admin"),
                    "patient" => RedirectToAction("MyAppointments", "Patient"),
                    "receptionist" => RedirectToAction("Queue", "Receptionist"),
                    "doctor" => RedirectToAction("TodayQueue", "Doctor"),
                    _ => RedirectToAction("Dashboard", "Admin")
                };
            }

            return View();
        }

        [HttpPost("/Login")]
        public async Task<IActionResult> Login(string email, string password)
        {
            try
            {
                var client = _httpClientFactory.CreateClient();
                var request = new LoginRequest
                {
                    Email = email,
                    Password = password
                };

                var json = JsonSerializer.Serialize(request);
                using var content = new StringContent(json, Encoding.UTF8, "application/json");
                using var response = await client.PostAsync($"{BaseApiUrl}/auth/login", content);
                var responseBody = await response.Content.ReadAsStringAsync();

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var result = JsonSerializer.Deserialize<LoginResponse>(responseBody, JsonOptions);
                    if (result?.User is null || string.IsNullOrWhiteSpace(result.Token))
                    {
                        ViewBag.Email = email;
                        ViewBag.Error = "Login failed. Please try again later.";
                        return View();
                    }

                    HttpContext.Session.SetString("JwtToken", result.Token);
                    HttpContext.Session.SetString("UserId", result.User.Id.ToString());
                    HttpContext.Session.SetString("UserName", result.User.Name ?? string.Empty);
                    HttpContext.Session.SetString("UserRole", result.User.Role ?? string.Empty);
                    HttpContext.Session.SetString("ClinicId", result.User.ClinicId.ToString());
                    HttpContext.Session.SetString("ClinicName", result.User.ClinicName ?? string.Empty);
                    HttpContext.Session.SetString("ClinicCode", result.User.ClinicCode ?? string.Empty);

                    return (result.User.Role) switch
                    {
                        "admin" => RedirectToAction("Dashboard", "Admin"),
                        "patient" => RedirectToAction("MyAppointments", "Patient"),
                        "receptionist" => RedirectToAction("Queue", "Receptionist"),
                        "doctor" => RedirectToAction("TodayQueue", "Doctor"),
                        _ => RedirectToAction("Dashboard", "Admin")
                    };
                }

                if (response.StatusCode == HttpStatusCode.BadRequest)
                {
                    var apiError = JsonSerializer.Deserialize<ErrorViewModel>(responseBody, JsonOptions);
                    ViewBag.Email = email;
                    ViewBag.Error = "Validation error: " + (apiError?.Error ?? string.Empty);
                    return View();
                }

                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    _ = JsonSerializer.Deserialize<ErrorViewModel>(responseBody, JsonOptions);
                    ViewBag.Email = email;
                    ViewBag.Error = "Invalid email or password. Please try again.";
                    return View();
                }

                ViewBag.Email = email;
                ViewBag.Error = "Login failed. Please try again later.";
                return View();
            }
            catch (Exception)
            {
                ViewBag.Email = email;
                ViewBag.Error = "Server error. Please try again.";
                return View();
            }
        }

        [HttpGet("/Login/Logout")]
        public async Task<IActionResult> Logout()
        {
            await Task.CompletedTask;

            HttpContext.Session.Clear();
            return RedirectToAction("Login", "Login");
        }
    }
}
