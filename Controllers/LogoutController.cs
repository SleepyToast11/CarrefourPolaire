using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarrefourPolaire.Controllers;

[Authorize] // Optional: only allow authenticated users to log out
public class LogoutController : Controller
{
    [HttpPost("/Logout")]
    [ValidateAntiForgeryToken] 
    public async Task<IActionResult> Post()
    {
        await HttpContext.SignOutAsync("EmailLink");

        // Optionally clear session or other data if you use it
        //HttpContext.Session?.Clear();

        // Redirect to home (or localized home)
        return Redirect("/");
    }
}
    