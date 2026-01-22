using System.ComponentModel.DataAnnotations;

namespace TakeHomeChallenge.Domain.Entities;

public class Notification
{
    [Required]
    public int Id { get; set; }
    [Required]
    public string Title { get; set; } = String.Empty;

    public string Content { get; set; } = String.Empty;

    public string Channel { get; set; } = String.Empty;
}
