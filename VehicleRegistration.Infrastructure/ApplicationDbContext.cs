using Microsoft.EntityFrameworkCore;
using VehicleRegistration.Infrastructure.DataBaseModels;

namespace VehicleRegistration.Infrastructure
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<UserModel> Users { get; set; }
        public DbSet<VehicleModel> VehiclesDetails { get; set; }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<UserModel>().HasKey(u => u.UserId);
            modelBuilder.Entity<VehicleModel>().HasKey(v => v.VehicleId);
            modelBuilder.Entity<VehicleModel>().HasOne(u => u.User).WithMany(v => v.Vehicles).HasForeignKey(i => i.UserId);
        }
    }
}