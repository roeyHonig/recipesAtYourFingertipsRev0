using System;
using System.Collections.Generic;

namespace recipesAtYourFingertipsRev0.Models;

public class PaginatedRecipesViewModel
{
    public List<Recipe> Recipes { get; set; } = new();

    public int CurrentPage { get; set; }

    public int PageSize { get; set; }

    public int TotalRecipes { get; set; }

    public int TotalPages =>
        (int)Math.Ceiling((double)TotalRecipes / PageSize);

    public int FirstItem =>
        TotalRecipes == 0
            ? 0
            : ((CurrentPage - 1) * PageSize) + 1;

    public int LastItem =>
        Math.Min(CurrentPage * PageSize, TotalRecipes);
}