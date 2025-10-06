using CarrefourPolaire.Data;
using CarrefourPolaire.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace CarrefourPolaire.Pages;

public class ParticipantJoinModel : PageModel
{
    private readonly EventContext _context;

    public ParticipantJoinModel(EventContext context)
    {
        _context = context;
    }
    
    [BindProperty]
    public ParticipantInputModel Input { get; set; } = new();
    
    public string? GroupName { get; set; }
    
    public List<Allergy> Allergies { get; set; } = new();
    
    public async Task<IActionResult> OnGetAsync(Guid token)
    {
        var invite = await _context.GroupInviteTokens
            .Include(i => i.Group)
            .FirstOrDefaultAsync(i => i.Token == token && i.Active);
    
        if (invite == null)
        {
            return NotFound("This invite link is invalid or has been disabled.");
        }
    
        Allergies = await _context.Allergies.OrderBy(a => a.Name).ToListAsync();
    
        Input.GroupId = invite.GroupId;
        GroupName = invite.Group.Name; // could be group number or other label
    
        return Page();
    }
    
    public async Task<IActionResult> OnPostAsync(Guid token)
    {
        var invite = await _context.GroupInviteTokens.Include(groupInviteToken => groupInviteToken.Group)
            .FirstOrDefaultAsync(i => i.Token == token && i.Active);
        
        if (invite == null)
        {
            return NotFound("This invite link is invalid or has been disabled.");
        }
    
        if (!ModelState.IsValid)
        {
            Allergies = await _context.Allergies.ToListAsync();
            return Page();
        }
        var allergies = await _context.Allergies.Where(a => Input.SelectedAllergyIds.Distinct().Contains(a.Id)).ToListAsync();
    
        var participant = new Participant
        {
            Id = Guid.NewGuid(),
            RegistrationGroupId = invite.GroupId,
            GroupNumber = invite.Group.GroupNumber,
            Name = Input.Name,
            Email = Input.Email,
            Allergies = allergies,
        };
    
        _context.Participants.Add(participant);
        
        await _context.SaveChangesAsync();
    
        return Page();
    }

}

