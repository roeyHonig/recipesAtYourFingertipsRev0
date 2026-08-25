feat: navbar
Refine authentication-based navigation and home page flow

- Redirect authenticated users from Home (/) to My Recipes
- Keep the Home page as the landing/sign-in page for unauthenticated users
- Hide the application navbar from unauthenticated users
- Add Recipes, Add a Recipe, Privacy, and Sign out actions to the authenticated navbar
- Keep logout as a POST action through AccountController
- Simplify the Home view by removing authenticated-user UI
- Preserve the existing Google sign-in flow
- Ensure Google is no longer the default authentication challenge scheme









feat: recipes
Google login/logout working.
Google is no longer the default challenge scheme.
Internal User + ExternalLogin registration.
Request-scoped current user.
[Authorize] protecting Recipes.
Per-user recipe querying.
Recipe creation with server-controlled OwnerId and timestamps.
RTL cosmetics for the recipe UI.











feat: application user integration
- Add CurrentUserService to find or create application users
- Add CurrentUserMiddleware to synchronize authenticated identities
  with the application Users and ExternalLogins tables
- Store the resolved application User in request-scoped HttpContext.Items
- Allow controllers to retrieve the current application user without
  repeating the database lookup during the same request
- Update HomeController and Home view to use the application User
- Log application user creation and assigned internal User ID
- Verify Google authentication works in both development and production
- Verify repeated authenticated requests do not create duplicate users






- Persist ASP.NET Core Data Protection keys in PostgreSQL
- Add DataProtectionKeys EF migration





Implemented Google OAuth authentication using ASP.NET Core's cookie-based authentication. Added Google login and logout flows, configured development and production OAuth redirect URIs, and integrated authentication state into the Home page with conditional login/logout UI.
Added environment-aware reverse-proxy handling to correctly detect the original HTTPS scheme and public host when running behind the GitHub Codespaces proxy, while retaining the groundwork for production proxy configuration.
- Add Google OAuth authentication with cookie-based sign-in
- Add reusable AccountController login and logout flow
- Add Google sign-in/sign-out UI to the Home page
- Configure development and production OAuth redirect handling
- Add forwarded-header support for Codespaces/proxy environments





### Added PostgreSQL database integration with EF Core
- Added Entity Framework Core with PostgreSQL (Npgsql) support.
- Added `ApplicationDbContext` and configured it for PostgreSQL.
- Added initial domain entities:
  - `User`
  - `ExternalLogin`
  - `Recipe`
- Configured database relationships:
  - One `User` can have many `Recipe` records.
  - Each `User` can have one `ExternalLogin`.
  - Recipes reference their owner through `OwnerId`.
- Added cascade delete behavior for dependent recipes and external login records.
- Added a unique constraint on `(Provider, ProviderUserId)` to prevent duplicate external identities.
- Added a unique constraint on `ExternalLogin.UserId` to enforce one external login per application user.
- Added the initial EF Core migration.
- Created the PostgreSQL schema in Neon using the EF Core migration.
- Configured the local database connection using .NET User Secrets so credentials are not stored in source control.
- Added connection-string validation during application startup.