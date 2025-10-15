using System.ComponentModel.DataAnnotations;

namespace CarrefourPolaire.Models;

public class User
{
    public Guid Id {get; set;}
    
    [Required, EmailAddress, StringLength(128)]
    public string Email {get; set;}
    public RegistrationGroup? RegistrationGroup {get; set;}
    public Guid? RegistrationGroupId {get; set;}
    
    public Participant? Participant {get; set;}
    public Guid? ParticipantId {get; set;}
    
    public List<UserRole> UserRoles {get; set;} =  new List<UserRole>();
    
    public bool Confirmed {get; set;}
}