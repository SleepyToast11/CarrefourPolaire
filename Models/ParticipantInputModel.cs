using System.ComponentModel.DataAnnotations;

namespace CarrefourPolaire.Models;

public class ParticipantInputModel
{
    [Required]
    public Guid GroupId { get; set; }
    
    [Required]
    public int GroupNumber { get; set; }
    
    [Required, MaxLength(256)]
    public string Name { get; set; } = "";

    [EmailAddress, MaxLength(256)]
    public string? Email { get; set; } //Make comment as a if we need to contact you, but optional?

    // Selected allergy IDs
    public List<Guid> SelectedAllergyIds { get; set; } = new();
}