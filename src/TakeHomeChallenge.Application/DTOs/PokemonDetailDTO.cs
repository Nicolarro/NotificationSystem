namespace TakeHomeChallenge.Application.DTOs;

/// <summary>
/// Represents Pokemon data retrieved from the external PokeAPI.
/// </summary>
public class PokemonDetailDTO
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public List<string>? Types { get; set; }
    public int? Height { get; set; }
    public int? Weight { get; set; }
}
