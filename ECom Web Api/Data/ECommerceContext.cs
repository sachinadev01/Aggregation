using ECom_Web_Api.Models;
using Microsoft.EntityFrameworkCore;

namespace ECom_Web_Api.Data
{
    public class ECommerceContext: DbContext
    {
        public ECommerceContext(DbContextOptions<ECommerceContext> options): base(options) { }
        public DbSet<User> Users { get; set; }
        public DbSet<Service> Service { get; set; }
        public DbSet<Booking> Booking { get; set; }
        public DbSet<Payment> Payment { get; set; }
        public DbSet<Reviews> Reviews { get; set; }
        public DbSet<Notification> Notification { get; set; }
        public DbSet<FamilyDetail> FamilyDetails { get; set; }
        public DbSet<FamilyChild> FamilyChildren { get; set; }
        public DbSet<FamilyIdentity> FamilyIdentities { get; set; }
        public DbSet<AccessUser> AccessUser { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Menu> Menus { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Self relation for Menu
            modelBuilder.Entity<Menu>()
                .HasOne(m => m.Parent)
                .WithMany(m => m.Children)
                .HasForeignKey(m => m.ParentId)
                .OnDelete(DeleteBehavior.Restrict);
        }

    }
}
