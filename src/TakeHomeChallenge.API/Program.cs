using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using TakeHomeChallenge.Infrastructure;
using TakeHomeChallenge.Infrastructure.Repositories;
using TakeHomeChallenge.Infrastructure.ExternalServices;
using TakeHomeChallenge.Application.Interfaces;
using TakeHomeChallenge.Application.Services;
using TakeHomeChallenge.Application.Mappings;
using TakeHomeChallenge.Domain.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Heroku provides DATABASE_URL, convert it to a proper connection string
var connectionString = GetConnectionString(builder.Configuration);

var secretKey = builder.Configuration.GetValue<string>("ApiSettings:SecretKey")
    ?? throw new InvalidOperationException("ApiSettings:SecretKey is not configured in appsettings.json");

static string GetConnectionString(IConfiguration configuration)
{
    // Try Heroku DATABASE_URL first
    var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");
    
    if (!string.IsNullOrEmpty(databaseUrl))
    {
        // Parse Heroku DATABASE_URL format: postgres://user:password@host:port/database
        var uri = new Uri(databaseUrl);
        var username = uri.UserInfo.Split(':')[0];
        var password = uri.UserInfo.Split(':')[1];
        var host = uri.Host;
        var port = uri.Port;
        var database = uri.LocalPath.TrimStart('/');
        
        return $"Host={host};Port={port};Database={database};Username={username};Password={password};SSL Mode=Require;Trust Server Certificate=true";
    }
    
    // Fallback to appsettings.json
    return configuration.GetConnectionString("DefaultConnection")
        ?? throw new InvalidOperationException("ConnectionString 'DefaultConnection' is not configured");
}


//Adding CORS to the project

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        // Get allowed origins from environment variable or use defaults
        var allowedOrigins = builder.Configuration.GetValue<string>("AllowedOrigins")
            ?? "http://localhost:5173,http://localhost:3000,http://localhost:5062";
        
        policy.WithOrigins(allowedOrigins.Split(','))
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// Add services to the container.

builder.Services.AddAuthentication(options =>
{

    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
        ValidateIssuer = false,
        ValidateAudience = false,
    };
}
);


builder.Services.AddControllers();
builder.Services.AddDbContext<AppDbContext>(opt =>
{
    opt.UseNpgsql(connectionString);
});

builder.Services.AddAutoMapper(typeof(UserProfile).Assembly);

// HttpClient for external API calls
builder.Services.AddHttpClient<IPokemonService, PokemonApiClient>();

// Dependency Injection
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "TakeHomeChallenge", Version = "v1" });

    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);
});

var app = builder.Build();

app.UseCors("AllowReactApp");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "TakeHomeChallenge v1"));
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.MapControllers();

using var scope = app.Services.CreateScope();
var services = scope.ServiceProvider;

try
{
    var context = services.GetRequiredService<AppDbContext>();
    await context.Database.MigrateAsync();
    await DBInitializer.SeedData(context);


}
catch (Exception exception)
{
    var logger = services.GetRequiredService<ILogger<Program>>();
    logger.LogError(exception, "An error occurred during migration");

    throw;

}

app.MapGet("/hello", () => "Hello Minimal API!");

app.Run();

// This partial class declaration makes Program accessible to WebApplicationFactory<Program>
// in integration tests. Without this, the test project can't reference the entry point.
// Top-level statements generate an implicit Program class that is internal by default.
public partial class Program { }
