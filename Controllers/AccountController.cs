using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace recipesAtYourFingertipsRev0.Controllers;

public class AccountController : Controller
{
    public IActionResult Login(string provider)
    {
        if (provider != GoogleDefaults.AuthenticationScheme)
        {
            return BadRequest("Unsupported authentication provider.");
        }

        Console.WriteLine($"Request Scheme: {Request.Scheme}");
        Console.WriteLine($"Request Host: {Request.Host}");
        Console.WriteLine($"Request PathBase: {Request.PathBase}");

        var properties = new AuthenticationProperties
        {
            RedirectUri = "/"
        };

        return Challenge(
            properties,
            GoogleDefaults.AuthenticationScheme);
    }

    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(
            CookieAuthenticationDefaults.AuthenticationScheme);

        return RedirectToAction("Index", "Home");
    }
}