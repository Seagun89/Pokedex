using Microsoft.EntityFrameworkCore; // SQL injection protection and database connection management
using API.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace API.Data
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
        }
    }
}