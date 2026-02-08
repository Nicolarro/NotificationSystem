using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;

namespace TakeHomeChallenge.Domain.Entities;

public class User
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [MaxLength(200)]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MaxLength(200)]
    public string Password { get; set; } = string.Empty;

    public ICollection<int>? PokemonsIds { get; set; }
}


