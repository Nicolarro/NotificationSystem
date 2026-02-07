using System.ComponentModel.DataAnnotations;

namespace TakeHomeChallenge.Application.DTOs;

public class UpdateUserDTO
{
    [MaxLength(100)]
    public string? Name { get; set; }

    [EmailAddress]
    [MaxLength(200)]
    public string? Email { get; set; }

    [MinLength(6)]
    [MaxLength(200)]
    public string? Password { get; set; }

    public List<int>? PokemonIds { get; set; }
}