using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using recipesAtYourFingertipsRev0.Data;
using recipesAtYourFingertipsRev0.Models;

namespace recipesAtYourFingertipsRev0.Services;

public class CurrentUserService
{
    private readonly ApplicationDbContext _db;
    private readonly ILogger<CurrentUserService> _logger;

    public CurrentUserService(
        ApplicationDbContext db,
        ILogger<CurrentUserService> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<User?> GetOrCreateUserAsync(
        ClaimsPrincipal principal)
    {
        if (principal.Identity?.IsAuthenticated != true)
        {
            return null;
        }

        var providerUserId =
            principal.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrWhiteSpace(providerUserId))
        {
            throw new InvalidOperationException(
                "Authenticated user does not have a provider user ID.");
        }

        var provider =
            principal.FindFirstValue("ExternalProvider");

        if (string.IsNullOrWhiteSpace(provider))
        {
            throw new InvalidOperationException(
                "Authenticated user does not have an external provider.");
        }

        var externalLogin = await _db.ExternalLogins
            .SingleOrDefaultAsync(x =>
                x.Provider == provider &&
                x.ProviderUserId == providerUserId);

        if (externalLogin != null)
        {
            var existingUser = await _db.Users
                .SingleAsync(x => x.Id == externalLogin.UserId);

            _logger.LogInformation(
                "Authenticated application user found. UserId: {UserId}, Provider: {Provider}",
                existingUser.Id,
                provider);

            return existingUser;
        }

        var email = principal.FindFirstValue(ClaimTypes.Email);
        var displayName = principal.FindFirstValue(ClaimTypes.Name);

        var user = new User
        {
            Email = email,
            DisplayName = displayName,
            Role = "User",
            CreatedAt = DateTime.UtcNow
        };

        _db.Users.Add(user);

        await _db.SaveChangesAsync();

        var newExternalLogin = new ExternalLogin
        {
            UserId = user.Id,
            Provider = provider,
            ProviderUserId = providerUserId
        };

        _db.ExternalLogins.Add(newExternalLogin);

        await _db.SaveChangesAsync();

        _logger.LogInformation(
            "Created new application user. UserId: {UserId}, Provider: {Provider}",
            user.Id,
            provider);

        return user;
    }
    public static User? GetCurrentUserFromRequest(HttpContext context)
{
    return context.Items.TryGetValue(
        CurrentUserContext.UserKey,
        out var value)
        ? value as User
        : null;
}
}