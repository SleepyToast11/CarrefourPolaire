using System.Security.Claims;
using CarrefourPolaire.Data;
using CarrefourPolaire.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace CarrefourPolaire.Pages.SignUp;

public class CreateConfirmModel : PageModel
{
    public string Message { get; set; } = string.Empty;
    private readonly EventContext _context;

    public CreateConfirmModel(EventContext context)
    {
        _context = context;

    }
    public async Task<IActionResult> OnGetAsync(Guid token)
    {
        try
        {
            var loginToken = await _context.EmailVerificationTokens.Include(elt => elt.Registration).FirstOrDefaultAsync(t => t.Id == token);
    
            if (loginToken == null || loginToken.Used || loginToken.ExpiresAt < DateTime.UtcNow)
            {
                Message = "Invalid or expired link.";
                return Page();
            }
    
            loginToken.Used = true;
            await _context.SaveChangesAsync();
        
            var prevRegGroup = await _context.RegistrationGroups.SingleOrDefaultAsync(rg => rg.GroupNumber == loginToken.Registration.GroupNumber && rg.Confirmed);
    
            if (prevRegGroup != null)
            {
                Message = $"A group is already registered please contact: {prevRegGroup.OwnerName}. Please contact xyz@example.com if you need additional assistance."; //TODO: set the correct email
                return Page();
            }
            
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, loginToken.Email),
                new Claim(ClaimTypes.Role, nameof(UserRole.GroupLeader)),
                new Claim("GroupId", loginToken.RegistrationId.ToString())
            };
        
            var identity = new ClaimsIdentity(claims, "EmailLink");
            var principal = new ClaimsPrincipal(identity);
    
            await HttpContext.SignInAsync("EmailLink", principal, new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddDays(7)
            });
        
            Message = $"Logged in as {loginToken.Email}\nJoin link: ";
            
            var registration = loginToken.Registration;
            loginToken.Registration.Confirmed = true;
            var looseParticipants = _context.Participants.Where(p => p.GroupNumber == registration.GroupNumber).ToList();
            registration.Participants = looseParticipants;
            await _context.SaveChangesAsync();
            
            return RedirectToPage("/Dashboard/Login");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            Message = e.Message;
            return Page();
        }
    }
}

