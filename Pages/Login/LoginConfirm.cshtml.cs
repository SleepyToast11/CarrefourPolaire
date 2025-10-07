using System.Security.Claims;
using CarrefourPolaire.Data;
using CarrefourPolaire.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace CarrefourPolaire.Pages.Login;

public class LoginConfirmModel : PageModel
{
    private readonly EventContext _context;

    public LoginConfirmModel(EventContext context)
    {
        _context = context;
    }

    public string Message { get; set; } = string.Empty;

    public async Task<IActionResult> OnGetAsync(Guid token)
    {
        try
        {
            var loginToken = await _context.EmailLoginTokens.Include(elt => elt.Registration).FirstOrDefaultAsync(t => t.Id == token);

            if (loginToken == null || loginToken.Used || loginToken.ExpiresAt < DateTime.UtcNow)
            {
                Message = "Invalid or expired link.";
                return Page();
            }

            loginToken.Used = true;
            await _context.SaveChangesAsync();
        
            var prevRegGroup = await _context.RegistrationGroups.SingleOrDefaultAsync(rg => rg.GroupNumber == loginToken.Registration.GroupNumber && rg.Confirmed);
    
            if (prevRegGroup == null)
            {
                throw new Exception("Registration group not found.");
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
            
            var url = Url.Page("/Dashboard");
            if (url == null) throw new Exception("Dashboard page URL not found");

            return RedirectToPage("/Dashboard");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            Message = e.Message;
            return Page();
        }
    }
}
