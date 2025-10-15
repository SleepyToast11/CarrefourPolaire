using System.ComponentModel.DataAnnotations;

namespace CarrefourPolaire.Models;

public class RegistrationGroupInput
{
    [Required, MaxLength(256)]
    public string Name { get; set; } = "";
    
    [Required, MaxLength(256)]
    public string OwnerName { get; set; } = "";
    [EmailAddress, Required, MaxLength(256)]
    public string OwnerEmail { get; set; } = "";
    }