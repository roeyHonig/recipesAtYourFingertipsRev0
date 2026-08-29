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

    public async Task<IActionResult> Index(int page = 1, int pageSize = 20)
    {
        var appUser =
            CurrentUserService.GetCurrentUserFromRequest(HttpContext);

        if (appUser == null)
        {
            return Challenge();
        }

        var allowedPageSizes = new[] { 10, 20, 30, 40, 50 };

        if (!allowedPageSizes.Contains(pageSize))
        {
            pageSize = 20;
        }

        var totalRecipes = await _db.Recipes
            .Where(recipe => recipe.OwnerId == appUser.Id)
            .CountAsync();

        var totalPages = (int)Math.Ceiling(
            totalRecipes / (double)pageSize);

        if (totalPages == 0)
        {
            page = 1;
        }
        else if (page < 1)
        {
            page = 1;
        }
        else if (page > totalPages)
        {
            page = totalPages;
        }

        var recipes = await _db.Recipes
            .Where(recipe => recipe.OwnerId == appUser.Id)
            .OrderBy(recipe => recipe.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var viewModel = new PaginatedRecipesViewModel
            {
                Recipes = recipes,
                CurrentPage = page,
                PageSize = pageSize,
                TotalRecipes = totalRecipes
            };
        ViewBag.AppUser = appUser;

        return View(viewModel);
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

    [AllowAnonymous]
    public async Task<IActionResult> Details(int id)
    {
        var recipe = await _db.Recipes
            .FirstOrDefaultAsync(recipe => recipe.Id == id);

        if (recipe == null)
        {
            return NotFound();
        }

        return View(recipe);
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var appUser =
            CurrentUserService.GetCurrentUserFromRequest(HttpContext);

        if (appUser == null)
        {
            return Challenge();
        }

        var recipe = await _db.Recipes
            .SingleOrDefaultAsync(r =>
                r.Id == id &&
                r.OwnerId == appUser.Id);

        if (recipe == null)
        {
            return NotFound();
        }

        return View(recipe);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Recipe recipe)
    {
        var appUser =
            CurrentUserService.GetCurrentUserFromRequest(HttpContext);

        if (appUser == null)
        {
            return Challenge();
        }

        var existingRecipe = await _db.Recipes
            .SingleOrDefaultAsync(r =>
                r.Id == id &&
                r.OwnerId == appUser.Id);

        if (existingRecipe == null)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            return View(recipe);
        }

        existingRecipe.Title = recipe.Title;
        existingRecipe.Ingredients = recipe.Ingredients;
        existingRecipe.Instructions = recipe.Instructions;
        existingRecipe.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }
}