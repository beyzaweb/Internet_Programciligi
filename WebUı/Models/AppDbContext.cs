using System.Reflection.Emit;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace WebUı.Models
{
    public class AppDbContext : IdentityDbContext<AppUser, AppRole,string>
    {
        public AppDbContext(DbContextOptions options) : base (options)
        {
            
        }

        public DbSet<Category> Categories { get; set; }
        public DbSet<News> News { get; set; }



        protected override void OnModelCreating(ModelBuilder builder)
        {
            // Sabit GUID'ler tanımlanır
            var adminRoleId = Guid.NewGuid().ToString();
            var adminUserId = Guid.NewGuid().ToString();

            // Role seed
            builder.Entity<AppRole>().HasData(
                                new AppRole
                                {
                                    Id = adminRoleId, // Statik Role ID
                                    Name = "Admin",
                                    NormalizedName = "ADMIN"
                                });

            builder.Entity<AppRole>().HasData(
                               new AppRole
                               {
                                   Id = Guid.NewGuid().ToString(),
                                   Name = "User",
                                   NormalizedName = "USER"
                               });

            builder.Entity<AppRole>().HasData(
                             new AppRole
                             {
                                 Id = Guid.NewGuid().ToString(),
                                 Name = "Gazetici",
                                 NormalizedName ="GAZETICI"
                             });

                    // İlişki konfigürasyonları
                    builder.Entity<News>()
                        .HasOne(n => n.Category)
                        .WithMany(c => c.News)
                        .HasForeignKey(n => n.CategoryId)
                        .OnDelete(DeleteBehavior.Restrict);

                    builder.Entity<News>()
                        .HasOne(n => n.User)
                        .WithMany()
                        .HasForeignKey(n => n.UserId)
                        .OnDelete(DeleteBehavior.Restrict);

            // User seed
            builder.Entity<AppUser>().HasData(
                new AppUser
                {
                    Id = adminUserId, // Statik User ID
                    UserName = "admin",
                    NormalizedUserName = "ADMIN",
                    Email = "admin@admin.com",
                    NormalizedEmail = "ADMIN@ADMIN.COM",
                    EmailConfirmed = true,
                    PasswordHash = new PasswordHasher<AppUser>().HashPassword(null, "admin..11")
                });

            // User-Role relationship seed
            builder.Entity<IdentityUserRole<string>>().HasData(
                new IdentityUserRole<string>
                {
                    UserId = adminUserId, // Yukarıda oluşturulan sabit User ID
                    RoleId = adminRoleId  // Yukarıda oluşturulan sabit Role ID
                });

                    builder.Entity<News>()
             .HasOne(n => n.User)
             .WithMany()
             .HasForeignKey(n => n.UserId)
             .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<News>()
                .HasOne(n => n.ApprovedByUser)
                .WithMany()
                .HasForeignKey(n => n.ApprovedByUserId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<News>()
                .HasOne(n => n.Category)
                .WithMany()
                .HasForeignKey(n => n.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);
            base.OnModelCreating(builder);

        }

    }
}
