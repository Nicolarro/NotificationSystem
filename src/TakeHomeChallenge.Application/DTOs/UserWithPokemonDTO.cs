using System.Collections.ObjectModel;
using TakeHomeChallenge.Application.DTOs;

namespace TakeHomeChallenge.Application.DTOs;

public class UserWithPokemonDTO
{
    public int? Id { get; set; }
    public string? Name { get; set; }
    public string? Email { get; set; }

    public string? PassWord {get;set;}

    public Collection<int>? PokemonIds {get;set;}

    public Collection<PokemonDetailDTO>? Pokemons {get;set;}


}