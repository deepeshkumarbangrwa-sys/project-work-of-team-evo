using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using Microsoft.EntityFrameworkCore; // <-- NEW: Required for UseSqlite
using WebApplication1.Data;         // <-- NEW: Required for AppDbContext and DbInitializer
using Microsoft.Extensions.Logging; // <-- NEW: Recommended for error handling

var builder = WebApplication.CreateBuilder(args);

// --- NEW: Add Database Context Registration (SQLite) ---
builder.Services.AddDbContext<AppDbContext>(options =>
    // This tells the application to use the SQLite provider
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")) 
);
// ----------------------------------------------------

// Add services for MVC and Controllers
builder.Services.AddControllersWithViews();

// Configure Session Management
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// --- NEW: Database Initialization Block (Creates DB and Seeds Data) ---
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<AppDbContext>();
        // EnsureCreated() will create the physical DB file and schema if they don't exist
        context.Database.EnsureCreated(); 
        DbInitializer.Initialize(context); // Seeds the patient user
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while creating or seeding the database.");
    }
}
// ---------------------------------------------------------------------

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

// Activate Session (MUST come before routing)
app.UseSession();

// Final Route: Start at Account/Login
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.Run();