namespace CarrefourPolaire.Models;

public class Allergy
{
    public Guid Id { get; set; }
    public string Name { get; set; } = "";

    // Navigation property for EF Core many-to-many
    public List<Participant> Participants { get; set; } = new();
}