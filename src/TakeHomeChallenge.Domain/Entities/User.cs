using System.ComponentModel.DataAnnotations;

namespace TakeHomeChallenge.Domain.Entities;

public class User
{
    [Required]
    public int Id { get; set; }

    [Required]
    public string Name { get; set; } = string.Empty;

    [Required]
    public string Description { get; set; } = string.Empty;

    [Required]
    public int NotificationID { get; set; }

    public ICollection<Notification>? Notifications { get; set; }

}


