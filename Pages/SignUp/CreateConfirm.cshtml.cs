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
            var loginToken = await _context.EmailVerificationTokens
                .Include(elt => elt.Registration)
                .Include(elt => elt.User)
                .FirstOrDefaultAsync(t => t.Id == token);
            if (loginToken == null || loginToken.Used || loginToken.ExpiresAt > DateTime.UtcNow)
            {
                Message = "Invalid or expired link.";
                return Page();
            }
            
            loginToken.User.Confirmed = true;
            loginToken.Used = true;
            loginToken.Registration.Confirmed = true;
            
            await _context.SaveChangesAsync();
            
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, loginToken.User.Id.ToString()),
                new Claim(ClaimTypes.Name,  loginToken.User.Email),
                new Claim(ClaimTypes.Email, loginToken.User.Email),
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
        
            Message = $"Logged in as {loginToken.User.Email}\nJoin link: ";
            
            await _context.SaveChangesAsync();
            
            return RedirectToPage("/Dashboard/Index");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            Message = e.Message;
            return Page();
        }
    }
}

