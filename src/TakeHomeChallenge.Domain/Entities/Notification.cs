using System.ComponentModel.DataAnnotations;

namespace TakeHomeChallenge.Domain.Entities;

public class Notification
{
    [Required]
    public int Id { get; set; }
    [Required]
    public string Name { get; set; } = String.Empty;
}
