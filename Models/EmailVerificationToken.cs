using System.ComponentModel.DataAnnotations;

namespace CarrefourPolaire.Models;

public class EmailVerificationToken
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    [Required, EmailAddress, MaxLength(256)]
    public string Email { get; set; } = string.Empty;

    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime ExpiresAt { get; set; } = DateTime.UtcNow.AddMinutes(30);
    
    public Guid RegistrationId { get; set; }
    public required RegistrationGroup Registration { get; set; } = new();
    
    public required User User { get; set; }
    public Guid UserId { get; set; }
    
    
    public bool Used { get; set; } = false;

}