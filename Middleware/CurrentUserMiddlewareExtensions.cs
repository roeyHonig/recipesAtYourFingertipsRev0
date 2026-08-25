namespace recipesAtYourFingertipsRev0.Middleware;

public static class CurrentUserMiddlewareExtensions
{
    public static IApplicationBuilder UseCurrentUser(
        this IApplicationBuilder app)
    {
        return app.UseMiddleware<CurrentUserMiddleware>();
    }
}