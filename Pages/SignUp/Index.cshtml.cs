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
    public RegistrationGroup Input { get; set; } = new();

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        //Group logic
        var prevRegGroupExists = await _context.RegistrationGroups.AnyAsync(rg => rg.GroupNumber == Input.GroupNumber && rg.Confirmed);
        
        if (prevRegGroupExists)
        {
            var prevRegGroup =
                await _context.RegistrationGroups.FirstAsync(rg => rg.GroupNumber == Input.GroupNumber && rg.Confirmed);
            ErrorMessage = $"A group is already registered please contact: {prevRegGroup.OwnerName}. Please contact xyz@example.com if you need additional assistance."; //TODO: set the correct email
            return Page();
        }
        _context.RegistrationGroups.Add(Input);

        //Token logic
        var token = new EmailVerificationToken()
        {
            Email = Input.OwnerEmail,
            ExpiresAt = DateTime.UtcNow.AddMinutes(30),
            RegistrationId = Input.Id
        };

        var loginLink = Url.Page(
            "/Signup/CreateConfirm",
            pageHandler: null,
            values: new { token = token.Id },
            protocol: Request.Scheme);
        Console.WriteLine(loginLink);
        await _emailService.SendEmail(Input.OwnerEmail, "Magic link", loginLink);
        ErrorMessage = "Check your email for the login link, might be in junk!";
        
        _context.EmailVerificationTokens.Add(token);
        await _context.SaveChangesAsync();

        return Page();
    }
}
