namespace TakeHomeChallenge.Application.DTOs;

public class UserResponseDTO
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public ICollection<int>? PokemonIds { get; set; }
    public ICollection<PokemonDetailDTO>? PokemonDetails { get; set; }
}
