using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CarrefourPolaire.Pages;

[IgnoreAntiforgeryToken]
public class ChangeLanguageModel : PageModel
{
    public IActionResult OnGet(string culture, string returnUrl = null)
    {
        if (string.IsNullOrEmpty(culture))
        {
            culture = "fr"; // default
        }

        // Set the culture cookie
        Response.Cookies.Append(
            CookieRequestCultureProvider.DefaultCookieName,
            CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
            new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1), Path = "/" }
        );

        // Redirect back to the referring page or home
        if (string.IsNullOrEmpty(returnUrl) || !Url.IsLocalUrl(returnUrl))
        {
            returnUrl = "/";
        }

        return LocalRedirect(returnUrl);
    }
}
