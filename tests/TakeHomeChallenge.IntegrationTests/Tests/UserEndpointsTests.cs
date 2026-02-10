using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using TakeHomeChallenge.Application.DTOs;
using TakeHomeChallenge.IntegrationTests.Fixtures;
using Xunit;

namespace TakeHomeChallenge.IntegrationTests.Tests;

/// <summary>
/// Integration tests for the User API endpoints.
/// 
/// HOW THESE TESTS WORK:
/// =====================
/// 1. IClassFixture<CustomWebApplicationFactory> tells xUnit to create ONE factory
///    instance for ALL tests in this class. The factory spins up the real app pipeline.
///    
/// 2. _client is an HttpClient that sends requests DIRECTLY to the in-memory app —
///    no actual TCP port needed, no `dotnet run` required.
///    
/// 3. Before each test, ResetDatabaseAsync() truncates the Users table so every
///    test starts with a clean state (no leftover data from previous tests).
///    
/// 4. The Pokemon API is mocked (MockPokemonService), so when a user has
///    PokemonIds [1, 25], the response includes fake details for Bulbasaur & Pikachu.
///    
/// TEST NAMING CONVENTION:
/// =======================
/// MethodName_Scenario_ExpectedBehavior
/// Example: CreateUser_WithValidData_ReturnsCreatedWithUser
/// This makes it immediately clear WHAT is tested, UNDER WHAT conditions, 
/// and WHAT should happen.
/// </summary>
public class UserEndpointsTests : IClassFixture<CustomWebApplicationFactory>, IAsyncLifetime
{
    private readonly CustomWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public UserEndpointsTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    /// <summary>
    /// Runs before EACH test — ensures a clean database.
    /// </summary>
    public async Task InitializeAsync()
    {
        await _factory.ResetDatabaseAsync();
    }

    public Task DisposeAsync() => Task.CompletedTask;

    // ═══════════════════════════════════════════════════════════════
    //  HELPER METHODS
    //  Reduce duplication across tests. DRY principle.
    // ═══════════════════════════════════════════════════════════════

    private async Task<UserResponseDTO> CreateTestUserAsync(
        string name = "Test User",
        string email = "test@example.com",
        string password = "password123",
        ICollection<int>? pokemonIds = null)
    {
        var createDto = new CreateUserDTO
        {
            Name = name,
            Email = email,
            Password = password,
            PokemonIds = pokemonIds
        };

        var response = await _client.PostAsJsonAsync("/api/User", createDto);
        response.EnsureSuccessStatusCode();
        var user = await response.Content.ReadFromJsonAsync<UserResponseDTO>();
        return user!;
    }

    // ═══════════════════════════════════════════════════════════════
    //  POST /api/User — CREATE
    // ═══════════════════════════════════════════════════════════════

    [Fact]
    public async Task CreateUser_WithValidData_ReturnsCreatedWithUser()
    {
        // Arrange
        var createDto = new CreateUserDTO
        {
            Name = "Nicolas",
            Email = "nicolas@test.com",
            Password = "securepass123",
            PokemonIds = new List<int> { 1, 25 }
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/User", createDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var user = await response.Content.ReadFromJsonAsync<UserResponseDTO>();
        user.Should().NotBeNull();
        user!.Name.Should().Be("Nicolas");
        user.Email.Should().Be("nicolas@test.com");
        user.PokemonIds.Should().BeEquivalentTo(new[] { 1, 25 });

        // Note: CreateUser returns the mapped DTO without Pokemon enrichment.
        // Pokemon details are only populated on GET endpoints (GetUsers, GetUserById).
        // This is by design — the create response confirms what was saved, not enriched data.

        // Verify Location header points to the new resource
        response.Headers.Location.Should().NotBeNull();
        response.Headers.Location!.PathAndQuery.Should().Contain($"/api/User/{user.Id}");

        // Verify enrichment works when we GET the same user
        var getResponse = await _client.GetAsync($"/api/User/{user.Id}");
        var fetchedUser = await getResponse.Content.ReadFromJsonAsync<UserResponseDTO>();
        fetchedUser!.PokemonDetails.Should().NotBeNull();
        fetchedUser.PokemonDetails.Should().HaveCount(2);
        fetchedUser.PokemonDetails!.Select(p => p.Name).Should().Contain(new[] { "bulbasaur", "pikachu" });
    }

    [Fact]
    public async Task CreateUser_WithoutPokemonIds_ReturnsCreatedWithEmptyPokemon()
    {
        // Arrange
        var createDto = new CreateUserDTO
        {
            Name = "No Pokemon User",
            Email = "nopokemon@test.com",
            Password = "password123"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/User", createDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var user = await response.Content.ReadFromJsonAsync<UserResponseDTO>();
        user.Should().NotBeNull();
        user!.Name.Should().Be("No Pokemon User");
    }

    [Fact]
    public async Task CreateUser_WithMissingName_ReturnsBadRequest()
    {
        // Arrange — Name is [Required], sending empty should fail validation
        var createDto = new CreateUserDTO
        {
            Name = "",
            Email = "test@test.com",
            Password = "password123"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/User", createDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateUser_WithInvalidEmail_ReturnsBadRequest()
    {
        // Arrange — Email must be a valid format due to [EmailAddress]
        var createDto = new CreateUserDTO
        {
            Name = "Bad Email",
            Email = "not-an-email",
            Password = "password123"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/User", createDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateUser_WithShortPassword_ReturnsBadRequest()
    {
        // Arrange — Password has [MinLength(6)]
        var createDto = new CreateUserDTO
        {
            Name = "Short Pass",
            Email = "short@test.com",
            Password = "abc"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/User", createDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    // ═══════════════════════════════════════════════════════════════
    //  GET /api/User — GET ALL
    // ═══════════════════════════════════════════════════════════════

    [Fact]
    public async Task GetUsers_WhenEmpty_ReturnsEmptyList()
    {
        // Act
        var response = await _client.GetAsync("/api/User");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var users = await response.Content.ReadFromJsonAsync<List<UserResponseDTO>>();
        users.Should().NotBeNull();
        users.Should().BeEmpty();
    }

    [Fact]
    public async Task GetUsers_WithMultipleUsers_ReturnsAllUsers()
    {
        // Arrange
        await CreateTestUserAsync("User One", "one@test.com", "password123", new List<int> { 1 });
        await CreateTestUserAsync("User Two", "two@test.com", "password123", new List<int> { 25 });

        // Act
        var response = await _client.GetAsync("/api/User");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var users = await response.Content.ReadFromJsonAsync<List<UserResponseDTO>>();
        users.Should().HaveCount(2);
        users!.Select(u => u.Name).Should().Contain(new[] { "User One", "User Two" });
    }

    [Fact]
    public async Task GetUsers_WithPokemonIds_ReturnsPokemonDetails()
    {
        // Arrange — User with Pokemon IDs that exist in MockPokemonService
        await CreateTestUserAsync("Pokemon Trainer", "trainer@test.com", "password123", new List<int> { 1, 25, 150 });

        // Act
        var response = await _client.GetAsync("/api/User");

        // Assert
        var users = await response.Content.ReadFromJsonAsync<List<UserResponseDTO>>();
        var trainer = users!.First(u => u.Name == "Pokemon Trainer");

        trainer.PokemonDetails.Should().NotBeNull();
        trainer.PokemonDetails.Should().HaveCount(3);
        trainer.PokemonDetails!.Select(p => p.Name)
            .Should().BeEquivalentTo(new[] { "bulbasaur", "pikachu", "mewtwo" });
    }

    // ═══════════════════════════════════════════════════════════════
    //  GET /api/User/{id} — GET BY ID
    // ═══════════════════════════════════════════════════════════════

    [Fact]
    public async Task GetUserById_WhenExists_ReturnsUser()
    {
        // Arrange
        var created = await CreateTestUserAsync("Find Me", "findme@test.com", "password123", new List<int> { 25 });

        // Act
        var response = await _client.GetAsync($"/api/User/{created.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var user = await response.Content.ReadFromJsonAsync<UserResponseDTO>();
        user.Should().NotBeNull();
        user!.Id.Should().Be(created.Id);
        user.Name.Should().Be("Find Me");
        user.Email.Should().Be("findme@test.com");

        // Verify Pokemon enrichment happened
        user.PokemonDetails.Should().NotBeNull();
        user.PokemonDetails.Should().ContainSingle(p => p.Name == "pikachu");
    }

    [Fact]
    public async Task GetUserById_WhenNotExists_ReturnsNotFound()
    {
        // Act — ID 999 doesn't exist in a clean database
        var response = await _client.GetAsync("/api/User/999");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    // ═══════════════════════════════════════════════════════════════
    //  PUT /api/User/{id} — UPDATE
    // ═══════════════════════════════════════════════════════════════

    [Fact]
    public async Task UpdateUser_WithValidData_ReturnsNoContent()
    {
        // Arrange
        var created = await CreateTestUserAsync("Original Name", "original@test.com");

        var updateDto = new UpdateUserDTO
        {
            Name = "Updated Name"
        };

        // Act
        var response = await _client.PutAsJsonAsync($"/api/User/{created.Id}", updateDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Verify the update persisted
        var getResponse = await _client.GetAsync($"/api/User/{created.Id}");
        var updated = await getResponse.Content.ReadFromJsonAsync<UserResponseDTO>();
        updated!.Name.Should().Be("Updated Name");
        updated.Email.Should().Be("original@test.com"); // Unchanged field
    }

    [Fact]
    public async Task UpdateUser_PokemonIds_UpdatesSuccessfully()
    {
        // Arrange — start with Bulbasaur
        var created = await CreateTestUserAsync("Trainer", "trainer@test.com", "password123", new List<int> { 1 });

        var updateDto = new UpdateUserDTO
        {
            PokemonIds = new List<int> { 25, 150 } // Change to Pikachu + Mewtwo
        };

        // Act
        var response = await _client.PutAsJsonAsync($"/api/User/{created.Id}", updateDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Verify the Pokemon IDs were updated
        var getResponse = await _client.GetAsync($"/api/User/{created.Id}");
        var updated = await getResponse.Content.ReadFromJsonAsync<UserResponseDTO>();
        updated!.PokemonIds.Should().BeEquivalentTo(new[] { 25, 150 });
        updated.PokemonDetails!.Select(p => p.Name)
            .Should().BeEquivalentTo(new[] { "pikachu", "mewtwo" });
    }

    [Fact]
    public async Task UpdateUser_WhenNotExists_ReturnsNotFound()
    {
        // Arrange
        var updateDto = new UpdateUserDTO { Name = "Ghost" };

        // Act
        var response = await _client.PutAsJsonAsync("/api/User/999", updateDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task UpdateUser_WithInvalidEmail_ReturnsBadRequest()
    {
        // Arrange
        var created = await CreateTestUserAsync();

        var updateDto = new UpdateUserDTO
        {
            Email = "not-valid-email"
        };

        // Act
        var response = await _client.PutAsJsonAsync($"/api/User/{created.Id}", updateDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    // ═══════════════════════════════════════════════════════════════
    //  DELETE /api/User/{id} — DELETE
    // ═══════════════════════════════════════════════════════════════

    [Fact]
    public async Task DeleteUser_WhenExists_ReturnsNoContent()
    {
        // Arrange
        var created = await CreateTestUserAsync("To Delete", "delete@test.com");

        // Act
        var response = await _client.DeleteAsync($"/api/User/{created.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Verify it's actually gone
        var getResponse = await _client.GetAsync($"/api/User/{created.Id}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteUser_WhenNotExists_ReturnsNotFound()
    {
        // Act
        var response = await _client.DeleteAsync("/api/User/999");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    // ═══════════════════════════════════════════════════════════════
    //  FULL LIFECYCLE TEST
    //  Verifies the complete CRUD flow end-to-end.
    // ═══════════════════════════════════════════════════════════════

    [Fact]
    public async Task FullLifecycle_CreateReadUpdateDelete_WorksCorrectly()
    {
        // CREATE
        var created = await CreateTestUserAsync("Lifecycle User", "lifecycle@test.com", "password123", new List<int> { 1 });
        created.Id.Should().BeGreaterThan(0);

        // READ
        var getResponse = await _client.GetAsync($"/api/User/{created.Id}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var fetched = await getResponse.Content.ReadFromJsonAsync<UserResponseDTO>();
        fetched!.Name.Should().Be("Lifecycle User");

        // UPDATE
        var updateDto = new UpdateUserDTO { Name = "Updated Lifecycle", PokemonIds = new List<int> { 25 } };
        var updateResponse = await _client.PutAsJsonAsync($"/api/User/{created.Id}", updateDto);
        updateResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // VERIFY UPDATE
        var updatedResponse = await _client.GetAsync($"/api/User/{created.Id}");
        var updated = await updatedResponse.Content.ReadFromJsonAsync<UserResponseDTO>();
        updated!.Name.Should().Be("Updated Lifecycle");
        updated.PokemonIds.Should().BeEquivalentTo(new[] { 25 });

        // DELETE
        var deleteResponse = await _client.DeleteAsync($"/api/User/{created.Id}");
        deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // VERIFY DELETION
        var goneResponse = await _client.GetAsync($"/api/User/{created.Id}");
        goneResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
