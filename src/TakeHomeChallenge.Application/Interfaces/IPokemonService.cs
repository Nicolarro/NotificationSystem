using TakeHomeChallenge.Application.DTOs;

namespace TakeHomeChallenge.Application.Interfaces;

/// <summary>
/// Service for consuming the Pokemon external API and retrieving Pokemon data.
/// </summary>
public interface IPokemonService
{
    /// <summary>
    /// Gets a Pokemon by its ID from the external PokeAPI.
    /// </summary>
    /// <param name="pokemonId">The Pokemon ID (e.g., 1 for Bulbasaur).</param>
    /// <returns>Pokemon details if found, null otherwise.</returns>
    Task<PokemonDetailDTO?> GetPokemonByIdAsync(int pokemonId);

    /// <summary>
    /// Gets multiple Pokemon by their IDs.
    /// </summary>
    /// <param name="pokemonIds">Collection of Pokemon IDs.</param>
    /// <returns>List of Pokemon details that were successfully retrieved.</returns>
    Task<ICollection<PokemonDetailDTO>> GetPokemonsByIdsAsync(IEnumerable<int> pokemonIds);
}
