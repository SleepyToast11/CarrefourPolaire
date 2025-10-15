using System.ComponentModel.DataAnnotations;
using CarrefourPolaire.Data;
using CarrefourPolaire.Models;
using CarrefourPolaire.Pages.Contact;
using CarrefourPolaire.Services;
using CarrefourPolaire.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Org.BouncyCastle.Asn1.X509;

namespace CarrefourPolaire.Pages.Login;

public class IndexModel : PageModel
{
    private readonly EventContext _context;
    private readonly ILogger<IndexModel> _logger;
    private readonly IEmailService _emailService;

    public IndexModel(EventContext context, ILogger<IndexModel> logger, IEmailService  emailService)
    {
        _context = context;
        _logger = logger;
        _emailService = emailService;
    }

    [BindProperty, Required, EmailAddress] public string InputEmail { get; set; } = string.Empty;

    public string? Message { get; set; }

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        try
        {
            if (!ModelState.IsValid)
                return Page();

            var user = _context.Users.FirstOrDefault(i => i.Confirmed == true && i.Email == InputEmail);
            
            if (user != null)
            {
                var token = new EmailLoginToken
                {
                    Email = InputEmail,
                    ExpiresAt = DateTime.UtcNow.AddMinutes(30),
                    Used = false
                };

                _context.EmailLoginTokens.Add(token);
                await _context.SaveChangesAsync();

                var loginLink = Url.Page(
                    "/Login/LoginConfirm",
                    pageHandler: null,
                    values: new { token = token.Id },
                    protocol: Request.Scheme);

                if (loginLink != null) await _emailService.SendEmail(InputEmail, "Magic link", loginLink);
                else
                {
                    throw new Exception("Invalid link");
                }

                Message = "Check your email for the login link";

            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
        
        Message = "Check your email for the login link";

        return Page();
    }
}

