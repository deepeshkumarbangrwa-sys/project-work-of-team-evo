using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http; // Required for Session
using System.Diagnostics;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        // --- NEW HELPER METHOD FOR TESTING ---
        // This simulates a login by setting the Session variables manually.
        public IActionResult LoginSim(string role)
        {
            // 1. Set the Role
            HttpContext.Session.SetString("UserRole", role);

            // 2. Set the ID based on the role (Mock Data)
            if (role == "Patient")
            {
                HttpContext.Session.SetInt32("UserID", 123); // Patient John
                // Redirect Patients straight to their chat
                return RedirectToAction("ViewComments", "Feedback");
            }
            else
            {
                HttpContext.Session.SetInt32("UserID", 456); // Dr. Smith
                // Redirect Clinicians to their Dashboard
                return RedirectToAction("ClinicianDashboard", "Feedback");
            }
        }
        // -------------------------------------

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}