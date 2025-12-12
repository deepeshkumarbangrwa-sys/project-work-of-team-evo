using WebApplication1.Models; 
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using System.Linq; 

namespace WebApplication1.Data
{
    public static class DbInitializer
    {
        private static string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(bytes);
            }
        }

        public static void Initialize(AppDbContext context)
        {
            // Ensures the physical database file and schema are created
            context.Database.EnsureCreated();

            // Only seed data if no users exist
            if (context.Users.Any())
            {
                return;   
            }

            var patient = new User
            {
                UserId = 1, 
                Email = "john@email.com", 
                PasswordHash = HashPassword("pass123"), // Hashed password for "password"
                Role = "Patient",
                FullName = "Sensore Patient User"
            };
            
            context.Users.Add(patient);
            
            context.SaveChanges();
        }
    }
}