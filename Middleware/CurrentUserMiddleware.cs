using recipesAtYourFingertipsRev0.Services;

namespace recipesAtYourFingertipsRev0.Middleware;

public class CurrentUserMiddleware
{
    private readonly RequestDelegate _next;

    public CurrentUserMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(
        HttpContext context,
        CurrentUserService currentUserService)
    {
        if (context.User.Identity?.IsAuthenticated == true)
            {
                var user = await currentUserService.GetOrCreateUserAsync(
                    context.User);

                context.Items[CurrentUserContext.UserKey] = user;
            }

        await _next(context);
    }
}