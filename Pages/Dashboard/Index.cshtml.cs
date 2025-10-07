using CarrefourPolaire.Data;
using CarrefourPolaire.Models;
using CarrefourPolaire.Services;
using CarrefourPolaire.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace CarrefourPolaire.Pages.Dashboard;

[Authorize(Roles = nameof(UserRole.GroupLeader))] // only logged-in group leaders
public class IndexModel : PageModel
{
    private readonly EventContext _context;
    private readonly IInviteTokenService _tokenService;
    
    public IndexModel(EventContext context, IInviteTokenService tokenService)
    {
        _context = context;
        _tokenService = tokenService;
    }
    
    public RegistrationGroup? Group { get; set; } = null;
    public List<Participant> ConfirmedParticipants { get; set; } = new();
    public List<Participant> PendingParticipants { get; set; } = new();
    public string ShareLink { get; set; } = "";


public async Task<IActionResult> OnGetAsync()
    {
        var groupIdClaim = User.FindFirst("GroupId")?.Value;
        if (groupIdClaim == null)
            return RedirectToPage("/Login");

        var groupId = Guid.Parse(groupIdClaim);

        Group = await _context.RegistrationGroups
            .Include(r => r.Participants)
            .FirstOrDefaultAsync(r => r.Id == groupId);

        if (Group == null)
            return NotFound();

        // ensure there is always a token
        var token = await _tokenService.GetOrCreateTokenAsync(groupId);
        
        ShareLink = Url.Page(
            "/Dashboard/ParticipantJoin",
            null,
            values: new { token = token.Token },
            Request.Scheme
        );
        
        if (ShareLink == null)
            throw new Exception("Share link not found.");
        
        ConfirmedParticipants = Group.Participants.Where(p => p.GroupConfirmed).ToList();
        PendingParticipants = Group.Participants.Where(p => !p.GroupConfirmed).ToList();

        return Page();
    }

    public async Task<IActionResult> OnPostRegenerateLinkAsync()
    {
        var groupIdClaim = User.FindFirst("GroupId")?.Value;
        if (groupIdClaim == null)
            return Unauthorized();

        var groupId = Guid.Parse(groupIdClaim);

        // Generate new token
        var newToken = await _tokenService.CreateNewTokenAsync(groupId);

        var newLink = Url.Page(
            "/ParticipantJoin",
            null,
            values: new { token = newToken.Token },
            Request.Scheme
        );
        
        
        return new JsonResult(new { shareLink = newLink, message = "A new invite link has been generated. Old ones are now invalid." });
    }


    public async Task<IActionResult> OnPostConfirmAsync([FromBody] ParticipantActionDto dto)
    {
        var participant = await _context.Participants.FindAsync(dto.ParticipantId);
        if (participant == null) return NotFound();

        participant.GroupConfirmed = true;
        await _context.SaveChangesAsync();

        return new JsonResult(participant);

    }

    public async Task<IActionResult> OnPostDenyAsync([FromBody] ParticipantActionDto dto)
    {
        var participant = await _context.Participants.FindAsync(dto.ParticipantId);
        if (participant == null) return NotFound();

        _context.Participants.Remove(participant);
        await _context.SaveChangesAsync();

        return new JsonResult(new { success = true });
    }
    
    public class ParticipantActionDto
    {
        public Guid ParticipantId { get; set; }
    }
}

