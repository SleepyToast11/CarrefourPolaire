using CarrefourPolaire.Data;
using CarrefourPolaire.Models;
using CarrefourPolaire.Services;
using CarrefourPolaire.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace CarrefourPolaire.Pages.SignUp;

public class IndexModel : PageModel
{
    private readonly EventContext _context;
    private readonly IEmailService _emailService;

    public IndexModel(EventContext context, IEmailService emailService)
    {
        _context = context;
        _emailService = emailService;
    }
    
    public string? ErrorMessage { get; set; }

    [BindProperty]
    public RegistrationGroupInput Input { get; set; } = new();

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var user = _context.Users.FirstOrDefault(u => u.Confirmed && u.Email == Input.OwnerEmail);

        //If user does exist, will create a login link instead
        if (user != null)
        {
            var token = new EmailLoginToken
                {
                    Email = Input.OwnerEmail,
                    ExpiresAt = DateTime.UtcNow.AddMinutes(30)
                };
            
            _context.EmailLoginTokens.Add(token);
            await _context.SaveChangesAsync();
            
            var loginLink = Url.Page(
                "/Login/LoginConfirm",
                pageHandler: null,
                values: new { token = token.Id },
                protocol: Request.Scheme);

            if (loginLink != null)
                ;
            //TODO: maybe change this
            //await _emailService.SendEmail(Input.OwnerEmail, "Magic link", loginLink);
        }
        else
        {
            var registration = new RegistrationGroup()
            {
                Confirmed = false,
                OwnerEmail = Input.OwnerEmail,
                Name = Input.Name,
            };
        
            var User = new User()
            {
                Email = Input.OwnerEmail,
                RegistrationGroup = registration,
                UserRoles = new() { UserRole.GroupLeader },
                Confirmed = false,
            };
        
            //Token logic
            var token = new EmailVerificationToken()
            {
                Email = Input.OwnerEmail,
                ExpiresAt = DateTime.UtcNow.AddMinutes(30),
                Registration = registration,
                User = User,
                Used = false,
            };

            var loginLink = Url.Page(
                "/Signup/CreateConfirm",
                pageHandler: null,
                values: new { token = token.Id },
                protocol: Request.Scheme);
            
            Console.WriteLine(loginLink);
            
            //TODO: maybe change this
            //await _emailService.SendEmail(Input.OwnerEmail, "Magic link", loginLink);
            ErrorMessage = "Check your email for the login link, might be in junk!";
        
            _context.EmailVerificationTokens.Add(token);
            await _context.SaveChangesAsync();

        }

        return Page();
    }
}
