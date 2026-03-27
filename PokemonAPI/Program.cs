using PokemonAPI.Infrastructure.Data;
using PokemonAPI.ErrorHandling;
using PokemonAPI.MessageBroker;
using PokemonAPI.Infrastructure.Repos;
using PokemonAPI.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SharedDtos.HelperObjects;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddControllers(); // Adds services for controllers to the container
builder.Services.AddSingleton<IRabbitMQPublisher<ExportPokemonMessage>, RabbitMQPublisher<ExportPokemonMessage>>(sp =>
{
    var publisher = new RabbitMQPublisher<ExportPokemonMessage>();
    publisher.CreateAsync("Pokemon_Export_Worker").GetAwaiter().GetResult();
    return publisher;
}); // Registers the RabbitMQPublisher as a singleton service, ensuring that only one instance of the publisher is created and shared across the entire application, which is suitable for services that manage shared resources like message queues and can help improve performance and reduce resource usage by reusing the same instance.
builder.Services.AddScoped<IPokemonRepository, PokemonRepository>(); // Registers the PokemonRepository as the implementation for the IPokemonRepository interface why scoped? Because we want a new instance of the repository to be created for each request, ensuring that database contexts are not shared across requests and preventing potential issues with concurrent access.
builder.Services.AddScoped<IPokemonService, PokemonService>(); // Registers the PokemonService as the implementation for the IPokemonService interface why scoped? Because we want a new instance of the service for each requested
builder.Services.AddSwaggerGen(c => {
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "PokemonAPI", Version = "v1" });

    // 🔑 Add JWT Bearer definition
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter your JWT token"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            Array.Empty<string>()
        }
    });
}); // Adds services for generating Swagger/openAPI documents

//Configures the database connection using Entity framework and SQL server
builder.Services.AddDbContext<PokemonDBContext>(options => 
    options.UseSqlServer(builder.Configuration.GetConnectionString("MyDb"))); // Grabs the connection string from appsettings.json

// Configures distributed caching using Redis, allowing for improved performance and reducing db load by caching frequently
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("RedisCache");
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
.AddJwtBearer(options =>
{
    options.Authority = builder.Configuration["JWT:Authority"];
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuerSigningKey = true,
        ValidateIssuer = true,
        ValidateAudience = true,

        ValidIssuer = builder.Configuration["JWT:Issuer"],
        ValidAudience = builder.Configuration["JWT:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(builder.Configuration["JWT:Secret"] ??
        throw new ArgumentNullException("JWT:Secret configuration value is missing.")))
    };
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("CanViewAllPokemon", policy => policy.RequireRole("User"));
    options.AddPolicy("CanViewPokemon", policy => policy.RequireRole("User"));
    options.AddPolicy("CanAddPokemon", policy => policy.RequireRole("Admin"));
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options => options.SwaggerEndpoint("/swagger/v1/swagger.json", "My PokemonAPI V1")); //Configures swagger UI endpoint
}
app.UseMiddleware<CustomExceptionHandlerMiddleware>(); // Adds middleware for handling exceptions globally, allowing you to catch and handle exceptions in a centralized manner, improving error handling and providing consistent error responses across the application. This middleware can be configured to log exceptions, return custom error messages, or perform other actions when an unhandled exception occurs during the processing of a request.
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();  // Maps controller endpoints to the app request pipeline

app.Run(); 
