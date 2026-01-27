export class ConfigService {
  private config: Record<string, string | undefined> = {};

  constructor() {
    this.loadConfig();
  }

  private loadConfig(): void {
    // Load configuration from environment variables
    this.config = {
      POKEMON_API_URL: import.meta.env.VITE_POKEMON_API_URL || "https://pokeapi.co/api/v2",
    };
  }

  get<T = string>(key: string, defaultValue?: T): T {
    return (this.config[key] ?? defaultValue) as T;
  }

  set<T = string>(key: string, value: T): void {
    this.config[key] = String(value);
  }
}