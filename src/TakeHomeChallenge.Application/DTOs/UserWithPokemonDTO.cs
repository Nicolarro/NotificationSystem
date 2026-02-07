namespace TakeHomeChallenge.Application.DTOs;

// This DTO has been consolidated into UserResponseDTO.
// TODO: Remove this file and update all references to use UserResponseDTO.
public class UserWithPokemonDTO
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public List<int>? PokemonIds { get; set; }
}