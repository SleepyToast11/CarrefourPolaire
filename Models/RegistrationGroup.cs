using System.ComponentModel.DataAnnotations;

namespace CarrefourPolaire.Models;

public class RegistrationGroup
{
    public Guid Id { get; set; }
    
    [Required, MaxLength(256)]
    public string Name { get; set; } = "";
    public int GroupNumber { get; set; }
    
    [Required, MaxLength(256)]
    public string OwnerName { get; set; } = "";
    [EmailAddress, Required, MaxLength(256)]
    public string OwnerEmail { get; set; } = "";

    public List<Participant> Participants { get; set; } = new();
    
    public bool Confirmed { get; set; } = false;  
}