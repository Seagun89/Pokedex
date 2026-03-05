using Microsoft.EntityFrameworkCore; // SQL injection protection and database connection management
using API.Models;

namespace API.Data
{
    public class PokemonDBContext : DbContext
    {
      public PokemonDBContext(DbContextOptions dbContextOptions) : base(dbContextOptions) {}
      public DbSet<Pokemon> Pokemon { get; set;} // Represents the pokemon table in the DB using the Pokemon model

      protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Pokemon>()
                .HasMany(p => p.Abilities);
        }
    }
}