using ExportPokemon;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<ExportPokemonWorker>();

var host = builder.Build();
host.Run();
