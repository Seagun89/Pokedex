using Microsoft.EntityFrameworkCore; // SQL injection protection and database connection management
using Infrastructure.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Data
{
    public class PokemonDBContext : IdentityDbContext<AppUser> // Represents the database context for the application, manages the connection to the database and provides access to the Pokemon table through the DbSet<Pokemon> property, also inherits from IdentityDbContext to integrate ASP.NET Core Identity for user authentication and authorization
    {
        public PokemonDBContext(DbContextOptions dbContextOptions) : base(dbContextOptions) {}
        public DbSet<Pokemon> Pokemon { get; set;} // Represents the pokemon table in the DB using the Pokemon model

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Pokemon>()
                .HasMany(p => p.Abilities); // Add a relationship between pokemon and ability models, makes a table for abilities and links it to pokemon table using pokemonid as FKey

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