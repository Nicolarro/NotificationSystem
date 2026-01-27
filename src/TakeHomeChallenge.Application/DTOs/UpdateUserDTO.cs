using System.Collections.ObjectModel;

namespace TakeHomeChallenge.Application.DTOs;

public class UpdateUserDTO
{
    public string? Name { get; set; }
    public string? Email { get; set; }

    public string? PassWord {get;set;}

    public Collection<int>? PokemonIds {get;set;}

}