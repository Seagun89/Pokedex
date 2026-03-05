using API.Data;
using API.ErrorHandling;
using API.Repos;
using API.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddControllers(); // Adds services for controllers to the container
builder.Services.AddScoped<IPokemonRepository, PokemonRepository>(); // Registers the PokemonRepository as the implementation for the IPokemonRepository interface why scoped? Because we want a new instance of the repository to be created for each request, ensuring that database contexts are not shared across requests and preventing potential issues with concurrent access.
builder.Services.AddTransient<IPokemonService, PokemonService>(); // Registers the PokemonService as the implementation for the IPokemonService interface why transient? Because we want a new instance of the service to be created each time it is requested, which is suitable for lightweight, stateless services that do not maintain any shared state and can be safely used across multiple requests without the risk of unintended side effects.
builder.Services.AddEndpointsApiExplorer(); 
builder.Services.AddSwaggerGen(); // Adds services for generating Swagger/openAPI documents

//Configures the database connection using Entity framework and SQL server
builder.Services.AddDbContext<PokemonDBContext>(options => 
    options.UseSqlServer(builder.Configuration.GetConnectionString("MyDb"))); // Grabs the connection string from appsettings.json

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options => options.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1")); //Configures swagger UI endpoint
    
}
app.UseMiddleware<CustomExceptionHandlerMiddleware>(); // Adds middleware for handling exceptions globally, allowing you to catch and handle exceptions in a centralized manner, improving error handling and providing consistent error responses across the application. This middleware can be configured to log exceptions, return custom error messages, or perform other actions when an unhandled exception occurs during the processing of a request.
app.UseHttpsRedirection();

app.MapControllers();  // Maps controller endpoints to the app request pipeline

app.Run(); 
