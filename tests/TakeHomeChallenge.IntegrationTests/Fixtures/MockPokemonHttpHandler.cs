using System.Net;
using System.Text;
using System.Text.Json;

namespace TakeHomeChallenge.IntegrationTests.Fixtures;

/// <summary>
/// A fake HttpMessageHandler that intercepts HTTP requests and returns
/// fake PokeAPI-shaped JSON responses — without making any network calls.
/// 
/// WHY MOCK AT THE HTTP LEVEL INSTEAD OF THE SERVICE LEVEL?
/// =========================================================
/// Mocking IPokemonService (the previous approach) replaces the ENTIRE client.
/// Your PokemonApiClient code is never exercised — you're testing nothing about:
///   - Whether it builds the correct URL
///   - Whether it correctly parses PokeAPI's JSON structure
///   - Whether it handles HTTP errors (404, 500) gracefully
/// 
/// By mocking at the HttpMessageHandler level:
///   - PokemonApiClient is REAL and fully tested
///   - HttpClient.GetAsync() is intercepted before it reaches the network
///   - The fake handler returns JSON that matches the real PokeAPI format
///   - You test the full code path: Service → ApiClient → HTTP → JSON parsing
/// 
/// HOW IT WORKS:
/// =============
/// HttpClient uses a pipeline of handlers (like middleware).
/// The final handler normally opens a TCP connection. We replace it with this
/// class, which inspects the URL and returns canned responses.
/// 
///   HttpClient → [MockPokemonHttpHandler] → returns fake response
///                (instead of)
///   HttpClient → [SocketsHttpHandler] → real network call to pokeapi.co
/// </summary>
public class MockPokemonHttpHandler : HttpMessageHandler
{
    // Fake Pokemon data keyed by ID — same data as before but as raw JSON
    // matching the real PokeAPI response structure
    private static readonly Dictionary<int, string> _pokemonResponses = new()
    {
        [1] = BuildPokeApiJson(1, "bulbasaur", new[] { "grass", "poison" }, 7, 69),
        [25] = BuildPokeApiJson(25, "pikachu", new[] { "electric" }, 4, 60),
        [150] = BuildPokeApiJson(150, "mewtwo", new[] { "psychic" }, 20, 1220),
    };

    /// <summary>
    /// This method is called by HttpClient for every request.
    /// We inspect the URL, extract the Pokemon ID, and return fake JSON.
    /// </summary>
    protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, CancellationToken cancellationToken)
    {
        // Extract Pokemon ID from URL like "https://pokeapi.co/api/v2/pokemon/25"
        var segments = request.RequestUri?.AbsolutePath.TrimEnd('/').Split('/');
        var lastSegment = segments?.LastOrDefault();

        if (int.TryParse(lastSegment, out var pokemonId) && _pokemonResponses.TryGetValue(pokemonId, out var json))
        {
            // Known Pokemon → return 200 OK with fake JSON
            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };
            return Task.FromResult(response);
        }

        // Unknown Pokemon → return 404, just like the real PokeAPI
        return Task.FromResult(new HttpResponseMessage(HttpStatusCode.NotFound));
    }

    /// <summary>
    /// Builds a JSON string that matches the real PokeAPI response structure.
    /// PokemonApiClient.ParsePokemonResponse() expects this exact shape:
    ///   - root.id, root.name, root.height, root.weight
    ///   - root.sprites.front_default
    ///   - root.types[].type.name
    /// </summary>
    private static string BuildPokeApiJson(int id, string name, string[] types, int height, int weight)
    {
        var typesArray = types.Select((t, i) => new
        {
            slot = i + 1,
            type = new { name = t, url = $"https://pokeapi.co/api/v2/type/{t}" }
        });

        var responseObj = new
        {
            id,
            name,
            height,
            weight,
            sprites = new
            {
                front_default = $"https://raw.githubusercontent.com/PokeAPI/sprites/master/sprites/pokemon/{id}.png"
            },
            types = typesArray
        };

        return JsonSerializer.Serialize(responseObj);
    }
}
