using Microsoft.EntityFrameworkCore;
using VehicleRegistration.Infrastructure.DataBaseModels;
using VehicleRegistration.Infrastructure.DataBaseModels.RBAC_Model;

namespace VehicleRegistration.Infrastructure
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<UserModel> Users { get; set; }
        public DbSet<VehicleModel> VehiclesDetails { get; set; }

        public DbSet<Permission> Permissions { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }

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