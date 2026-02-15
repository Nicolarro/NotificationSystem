import axios from "axios";
import { ConfigService } from "../services/ConfigService";

interface PokemonResponse {
  id: number;
  name: string;
}

export interface PokemonDetails {
  id: number;
  name: string;
}

export class Pokemon {
  private readonly apiUrl: string;
  private configService: ConfigService;

  constructor(configService: ConfigService) {
    this.configService = configService;
    this.apiUrl = this.configService.get<string>(
      "POKEMON_API_URL",
      "https://pokeapi.co/api/v2",
    );
  }

  getPokemonDataById = async (id: number): Promise<PokemonResponse> => {
    try {
      const response = await axios.get(`${this.apiUrl}/pokemon/${id}`);

      return {
        id: response.data.id,
        name: response.data.name,
      };
    } catch (error) {
      console.log("There is an error in getPokemonDataById method " + error);
      throw new Error("Failed to fetch Pokemon data");
    }
  };
  async getPokemonDetailsByIds(ids: number[]): Promise<PokemonDetails[]> {
    const pokemonPromises = ids.map((id) => this.getPokemonDataById(id));
    const pokemonResults = await Promise.all(pokemonPromises);

    // Filter out any null results
    return pokemonResults.filter(
      (pokemon): pokemon is PokemonDetails => pokemon !== null,
    );
  }
}
