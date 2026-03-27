using AuthAPI.Infrastructure.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AuthAPI.Infrastructure.Data
{
    public class AuthDBContext : IdentityDbContext<AppUser>
    {
        public AuthDBContext(DbContextOptions dbContextOptions) : base(dbContextOptions) { }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            var roles = new List<IdentityRole>
            {
                new IdentityRole
                {
                    Id = "1",
                    Name = "Admin", // ability to add, update, and delete pokemon data
                    NormalizedName = "ADMIN"
                },
                new IdentityRole
                {
                    Id = "2",
                    Name = "User", // ability to only view pokemon data
                    NormalizedName = "USER"
                }
            };
            builder.Entity<IdentityRole>().HasData(roles); // Seed the database with roles for user authentication and authorization, allows for role-based access control
        }
    }
}
