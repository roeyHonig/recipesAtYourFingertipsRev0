using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using recipesAtYourFingertipsRev0.Models;
using recipesAtYourFingertipsRev0.Services;

namespace recipesAtYourFingertipsRev0.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {

        ViewBag.RequestScheme = Request.Scheme;
        ViewBag.RequestHost = Request.Host.ToString();
        ViewBag.RequestPathBase = Request.PathBase.ToString();

        var user = CurrentUserService.GetCurrentUserFromRequest(HttpContext);

        return View(user);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
