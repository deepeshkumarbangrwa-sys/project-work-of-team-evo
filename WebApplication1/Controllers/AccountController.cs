using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Security.Cryptography;
using System.Text;
using WebApplication1.Data; // <-- CORRECTED NAMESPACE
using System.Linq;
using Microsoft.AspNetCore.Http; // For Session management
using Microsoft.EntityFrameworkCore;

namespace WebApplication1.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _context;

        public AccountController(AppDbContext context)
        {
            _context = context;
        }

        private string HashPassword(string password)
        {
            // Must match the hashing method used in DbInitializer.cs
            using (var sha256 = SHA256.Create())
            {
                var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(bytes);
            }
        }

        // GET: /Account/Login
        public IActionResult Login()
        {
            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                ViewData["Error"] = "Email and password are required.";
                return View();
            }

            string hashedPassword = HashPassword(password);

            // --- USE EF CORE TO FIND USER (Fix for SqlException) ---
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == email && u.PasswordHash == hashedPassword);
            // ----------------------------------------------------

            if (user != null)
            {
                // Set Session for authentication
                HttpContext.Session.SetString("UserId", user.UserId.ToString());
                HttpContext.Session.SetString("UserEmail", user.Email);
                HttpContext.Session.SetString("UserRole", user.Role);

                if (user.Role == "Patient")
                {
                    // Redirect to the Dashboard after successful login
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    // Placeholder for future Clinician/Admin view
                    return RedirectToAction("Index", "Home"); 
                }
            }

            ViewData["Error"] = "Invalid login attempt.";
            return View();
        }

        // GET: /Account/Logout
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login", "Account");
        }
    }
}