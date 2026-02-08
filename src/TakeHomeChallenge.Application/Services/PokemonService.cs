using System.Text.Json;
using TakeHomeChallenge.Application.DTOs;
using TakeHomeChallenge.Application.Interfaces;

namespace TakeHomeChallenge.Application.Services;

/// <summary>
/// Service for consuming the Pokemon external API (PokeAPI).
/// Handles HTTP requests and response parsing.
/// </summary>
public class PokemonService : IPokemonService
{
    private readonly HttpClient _httpClient;
    private const string PokeApiBaseUrl = "https://pokeapi.co/api/v2/pokemon";

    public PokemonService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    /// <summary>
    /// Gets a Pokemon by its ID from PokeAPI.
    /// </summary>
    public async Task<PokemonDetailDTO?> GetPokemonByIdAsync(int pokemonId)
    {
        try
        {
            var response = await _httpClient.GetAsync($"{PokeApiBaseUrl}/{pokemonId}");
            
            if (!response.IsSuccessStatusCode)
            {
                // Pokemon not found or API error
                return null;
            }

            var content = await response.Content.ReadAsStringAsync();
            var pokemon = ParsePokemonResponse(content);
            
            return pokemon;
        }
        catch (Exception)
        {
            // Pokemon service is optional - if PokeAPI fails, return null
            // and let the user service continue without Pokemon details
            return null;
        }
    }

    /// <summary>
    /// Gets multiple Pokemon by their IDs.
    /// Makes parallel requests for efficiency.
    /// </summary>
    public async Task<ICollection<PokemonDetailDTO>> GetPokemonsByIdsAsync(IEnumerable<int> pokemonIds)
    {
        if (pokemonIds == null || !pokemonIds.Any())
        {
            return new List<PokemonDetailDTO>();
        }

        // Make parallel requests for all Pokemon
        var tasks = pokemonIds.Select(id => GetPokemonByIdAsync(id)).ToList();
        var results = await Task.WhenAll(tasks);

        // Filter out nulls (failed requests) and return only successful results
        return results
            .Where(p => p is not null)
            .Cast<PokemonDetailDTO>()
            .ToList();
    }

    /// <summary>
    /// Parses the PokeAPI JSON response into a PokemonDetailDTO.
    /// </summary>
    private static PokemonDetailDTO ParsePokemonResponse(string jsonContent)
    {
        using var doc = JsonDocument.Parse(jsonContent);
        var root = doc.RootElement;

        // Extract basic properties
        var id = root.GetProperty("id").GetInt32();
        var name = root.GetProperty("name").GetString() ?? string.Empty;

        // Extract image URL from sprites
        var imageUrl = root
            .GetProperty("sprites")
            .GetProperty("front_default")
            .GetString();

        // Extract types
        var types = new List<string>();
        if (root.TryGetProperty("types", out var typesElement))
        {
            foreach (var typeItem in typesElement.EnumerateArray())
            {
                if (typeItem.TryGetProperty("type", out var typeObj) &&
                    typeObj.TryGetProperty("name", out var typeName))
                {
                    types.Add(typeName.GetString() ?? string.Empty);
                }
            }
        }

        // Extract height and weight
        int? height = root.TryGetProperty("height", out var heightElement) 
            ? heightElement.GetInt32() 
            : null;

        int? weight = root.TryGetProperty("weight", out var weightElement) 
            ? weightElement.GetInt32() 
            : null;

        return new PokemonDetailDTO
        {
            Id = id,
            Name = name,
            ImageUrl = imageUrl,
            Types = types,
            Height = height,
            Weight = weight
        };
    }
}
