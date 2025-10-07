using System.ComponentModel.DataAnnotations;
using CarrefourPolaire.Data;
using CarrefourPolaire.Models;
using CarrefourPolaire.Services;
using CarrefourPolaire.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

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

            var token = new EmailLoginToken
            {
                Email = InputEmail,
                ExpiresAt = DateTime.UtcNow.AddMinutes(30)
            };

            _context.EmailLoginTokens.Add(token);
            await _context.SaveChangesAsync();

            var loginLink = Url.Page(
                "/Login/LoginConfirm",
                pageHandler: null,
                values: new { token = token.Id },
                protocol: Request.Scheme);
            
            await _emailService.SendEmail(InputEmail, "Magic link", loginLink);

            Message = "Check your email for the login link (demo: see logs).";
            return Page(); 
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }

        return Page();
    }
}

