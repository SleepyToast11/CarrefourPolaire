using System.ComponentModel.DataAnnotations;

namespace CarrefourPolaire.Models;

public class GroupInviteToken
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    public Guid GroupId { get; set; }
    public RegistrationGroup Group { get; set; } = null!;

    [Required]
    public Guid Token { get; set; } = Guid.NewGuid();
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime ExpiresAt { get; set; } = DateTime.UtcNow.AddYears(1); // if it becomes required, just make this a smaller val :)
    
    public bool Active { get; set; } = true;
}