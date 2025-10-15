using CarrefourPolaire.Models;
using Microsoft.EntityFrameworkCore;

namespace CarrefourPolaire.Data;

    public class EventContext : DbContext
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        //Sets up the many-to-many key to insure unique pair
        modelBuilder.Entity<Participant>()
            .HasMany(p => p.Allergies)
            .WithMany(a => a.Participants)
            .UsingEntity<Dictionary<string, object>>(
                "ParticipantAllergy", // join table name
                j => j.HasOne<Allergy>().WithMany().HasForeignKey("AllergyId"),
                j => j.HasOne<Participant>().WithMany().HasForeignKey("ParticipantId"),
                j =>
                {
                    j.HasKey("ParticipantId", "AllergyId"); // composite primary key ensures uniqueness
                }
            );
    }

    public EventContext(DbContextOptions<EventContext> options) : base(options)
    {

    }

    public DbSet<GroupInviteToken> GroupInviteTokens { get; set; } = null!;
    public DbSet<RegistrationGroup> RegistrationGroups { get; set; } = null!;
    public DbSet<Participant> Participants { get; set; } = null!;
    public DbSet<Allergy> Allergies { get; set; } = null!;
    public DbSet<EmailLoginToken> EmailLoginTokens { get; set; } = null!;
    public DbSet<EmailVerificationToken> EmailVerificationTokens { get; set; } = null!;
    public DbSet<User> Users { get; set; } = null!;
}
