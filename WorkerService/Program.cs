using Infrastructure.Data;
using Infrastructure.Repos;
using Microsoft.EntityFrameworkCore;
using WorkerService.ExportPokemonWorker;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddScoped<IPokemonRepository, PokemonRepository>();
builder.Services.AddHostedService<ExportPokemonWorker>();
builder.Services.AddDbContext<PokemonDBContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("MyDb"))); // Grabs the connection string from appsettings.json
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("Redis");
});

var host = builder.Build();
host.Run();
