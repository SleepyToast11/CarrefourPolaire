using System.ComponentModel.DataAnnotations;

namespace CarrefourPolaire.Models;

public class Participant
{
    public Guid Id { get; set; }

    [Required]
    public string Name { get; set; } = ""; //Possible fuzzy search in case someone just enters himself twice

    [EmailAddress]
    public string? Email { get; set; }

    // Many-to-many
    public List<Allergy> Allergies { get; set; } = new();

    public int GroupNumber { get; set; } = 0;
    
    public Guid RegistrationGroupId { get; set; }
    public RegistrationGroup RegistrationGroup { get; set; } = null!;
    
    public bool GroupConfirmed { get; set; }= false; 
}