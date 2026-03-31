using PokemonAPI.Infrastructure.Data;
using PokemonAPI.Infrastructure.Repos;
using Microsoft.EntityFrameworkCore;
using WorkerService.ExportPokemonWorker;

// if I'd want worker service to be it's fully owned microservice then I won't scope and use HttpClient fetch to pokemonAPI
var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddScoped<IPokemonRepository, PokemonRepository>();
builder.Services.AddHostedService<ExportPokemonWorker>();
builder.Services.AddDbContext<PokemonDBContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("MyDb"),
    sqlOptions => sqlOptions.EnableRetryOnFailure())); // Grabs the connection string from appsettings.json
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("Redis");
});

var host = builder.Build();

using (var scope = host.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<PokemonDBContext>();
    db.Database.Migrate(); // Creates DB if it doesn't exist & applies migrations
}

host.Run();
