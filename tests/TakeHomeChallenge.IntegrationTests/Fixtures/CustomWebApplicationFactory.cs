using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TakeHomeChallenge.Application.Interfaces;
using TakeHomeChallenge.Infrastructure;
using TakeHomeChallenge.Infrastructure.ExternalServices;
using Xunit;

namespace TakeHomeChallenge.IntegrationTests.Fixtures;

/// <summary>
/// Custom factory that bootstraps the ENTIRE real ASP.NET Core pipeline for testing.
/// 
/// WHAT IS WebApplicationFactory?
/// ===============================
/// Think of it as a "fake server" that runs your real application in-memory.
/// Instead of doing `dotnet run` and hitting localhost:5062, your test code
/// gets an HttpClient that talks to the app directly in the same process.
/// 
/// It reads your Program.cs (that's why we needed `public partial class Program { }`)
/// and recreates the full DI container, middleware pipeline, and controllers.
/// 
/// WHAT DO WE OVERRIDE?
/// =====================
/// 1. DATABASE → We swap the connection string to point to a SEPARATE test database.
///    This way tests never touch your development data.
///    
/// 2. HTTP HANDLER → We replace the HttpMessageHandler that PokemonApiClient uses.
///    The real PokemonApiClient stays registered and is fully exercised (URL building,
///    JSON parsing, error handling). Only the network transport is faked — HTTP requests
///    are intercepted by MockPokemonHttpHandler and return canned JSON responses.
/// 
/// 3. ENVIRONMENT → We set it to "Testing" so any environment-specific config applies.
/// 
/// WHY MOCK HTTP INSTEAD OF THE SERVICE?
/// =======================================
/// Replacing IPokemonService entirely skips testing PokemonApiClient's code.
/// By mocking at the HTTP level, everything is real except the network call:
///   Controller → UserService → PokemonApiClient → HttpClient → [MockHandler]
/// This catches bugs in URL construction, JSON parsing, and error handling.
/// </summary>
public class CustomWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    // Separate test database — never touches your dev DB
    private const string TestConnectionString =
        "Host=127.0.0.1;Port=5432;Database=TakeHomeChallengeDB_Test;User Id=postgres;Password=nilariver;TrustServerCertificate=True;";

    /// <summary>
    /// ConfigureWebHost is the key override — it lets us modify the DI container
    /// BEFORE the app starts. This is where we swap services.
    /// </summary>
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureServices(services =>
        {
            // ─── 1. REPLACE THE DATABASE ───────────────────────────────
            // Remove the original DbContext registration (the one that points to your dev DB)
            var dbContextDescriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));

            if (dbContextDescriptor != null)
                services.Remove(dbContextDescriptor);

            // Add a new DbContext pointing to the test database
            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseNpgsql(TestConnectionString);
            });

            // ─── 2. REPLACE THE HTTP TRANSPORT (NOT THE SERVICE) ───────
            // Remove the original HttpClient + PokemonApiClient typed-client registration
            var pokemonServiceDescriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(IPokemonService));

            if (pokemonServiceDescriptor != null)
                services.Remove(pokemonServiceDescriptor);

            // Remove any IHttpClientFactory registrations for PokemonApiClient
            var httpClientDescriptors = services
                .Where(d => d.ServiceType.FullName?.Contains("PokemonApiClient") == true ||
                            d.ImplementationType?.FullName?.Contains("PokemonApiClient") == true)
                .ToList();

            foreach (var descriptor in httpClientDescriptors)
                services.Remove(descriptor);

            // Re-register PokemonApiClient with a FAKE HttpMessageHandler.
            // The real PokemonApiClient code runs — it builds URLs, calls HttpClient,
            // parses JSON — but the HttpClient uses MockPokemonHttpHandler instead of
            // actually connecting to pokeapi.co.
            services.AddScoped<IPokemonService>(sp =>
            {
                var httpClient = new HttpClient(new MockPokemonHttpHandler());
                return new PokemonApiClient(httpClient);
            });
        });
    }

    /// <summary>
    /// Called once when the factory is created.
    /// Creates the test database and applies all EF Core migrations.
    /// </summary>
    public async Task InitializeAsync()
    {
        using var scope = Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        // EnsureDeleted → drops the test DB if it exists from a previous run
        // Migrate → applies all migrations to create a fresh schema
        await dbContext.Database.EnsureDeletedAsync();
        await dbContext.Database.MigrateAsync();
    }

    /// <summary>
    /// Called once when all tests in the class are done.
    /// Drops the test database so it doesn't linger.
    /// </summary>
    public new async Task DisposeAsync()
    {
        using var scope = Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await dbContext.Database.EnsureDeletedAsync();

        await base.DisposeAsync();
    }

    /// <summary>
    /// Helper: clears all data from the Users table between tests.
    /// This ensures each test starts with a clean state without
    /// the overhead of dropping/recreating the entire database.
    /// 
    /// Called in test class's Initialize method or in individual tests.
    /// Uses raw SQL with RESTART IDENTITY to reset auto-increment IDs.
    /// </summary>
    public async Task ResetDatabaseAsync()
    {
        using var scope = Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await dbContext.Database.ExecuteSqlRawAsync("TRUNCATE TABLE \"Users\" RESTART IDENTITY CASCADE;");
    }
}
