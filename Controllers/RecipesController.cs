using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using recipesAtYourFingertipsRev0.Data;
using recipesAtYourFingertipsRev0.Models;
using recipesAtYourFingertipsRev0.Services;

namespace recipesAtYourFingertipsRev0.Controllers;

[Authorize]
public class RecipesController : Controller
{
    private readonly ApplicationDbContext _db;

    public RecipesController(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<IActionResult> Index()
    {
        var appUser =
            CurrentUserService.GetCurrentUserFromRequest(HttpContext);

        if (appUser == null)
        {
            return Challenge();
        }

        var recipes = await _db.Recipes
            .Where(recipe => recipe.OwnerId == appUser.Id)
            .ToListAsync();

        ViewBag.AppUser = appUser;

        return View(recipes);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateRecipeViewModel model)
    {
        var appUser =
            CurrentUserService.GetCurrentUserFromRequest(HttpContext);

        if (appUser == null)
        {
            return Challenge();
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var now = DateTime.UtcNow;

        var recipe = new Recipe
        {
            OwnerId = appUser.Id,
            Title = model.Title,
            Ingredients = model.Ingredients,
            Instructions = model.Instructions,
            CreatedAt = now,
            UpdatedAt = now
        };

        _db.Recipes.Add(recipe);

        await _db.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }
}