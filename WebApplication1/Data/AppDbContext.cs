using Microsoft.EntityFrameworkCore;
using WebApplication1.Models; 

namespace WebApplication1.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        // These DbSet properties correspond to the tables in your database
        public DbSet<User> Users { get; set; }
        public DbSet<SensorData> SensorData { get; set; }
        public DbSet<UserFeedback> UserFeedback { get; set; }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<SensorData>()
                .HasIndex(s => new { s.UserId, s.Timestamp });

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Role);
                
            base.OnModelCreating(modelBuilder);
        }
    }
}