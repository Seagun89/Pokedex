using System.Security.Claims;
using API.Data;
using API.ErrorHandling;
using API.Models;
using API.Repos;
using API.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddControllers(); // Adds services for controllers to the container
builder.Services.AddScoped<IPokemonRepository, PokemonRepository>(); // Registers the PokemonRepository as the implementation for the IPokemonRepository interface why scoped? Because we want a new instance of the repository to be created for each request, ensuring that database contexts are not shared across requests and preventing potential issues with concurrent access.
builder.Services.AddTransient<IPokemonService, PokemonService>(); // Registers the PokemonService as the implementation for the IPokemonService interface why transient? Because we want a new instance of the service to be created each time it is requested, which is suitable for lightweight, stateless services that do not maintain any shared state and can be safely used across multiple requests without the risk of unintended side effects.
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddEndpointsApiExplorer(); 
builder.Services.AddSwaggerGen(c => {
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Pokemon API", Version = "v1" });

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
    options.InstanceName = "PokemonApiCache_";
});

// Configures Identity Services for user authentication and authorization
builder.Services.AddIdentity<AppUser, IdentityRole>(options =>
{
    options.Password.RequireUppercase = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = default;

    options.User.RequireUniqueEmail = true;
    options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._";
}).AddEntityFrameworkStores<PokemonDBContext>();

// Configures JWT authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultForbidScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultSignInScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultSignOutScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
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
    options.AddPolicy("CanViewAllPokemon", policy => policy.RequireClaim(ClaimTypes.Role, "User"));
    options.AddPolicy("CanAddPokemon", policy => policy.RequireClaim(ClaimTypes.Role, "Admin"));
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options => options.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1")); //Configures swagger UI endpoint
}
app.UseMiddleware<CustomExceptionHandlerMiddleware>(); // Adds middleware for handling exceptions globally, allowing you to catch and handle exceptions in a centralized manner, improving error handling and providing consistent error responses across the application. This middleware can be configured to log exceptions, return custom error messages, or perform other actions when an unhandled exception occurs during the processing of a request.
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();  // Maps controller endpoints to the app request pipeline

app.Run(); 
