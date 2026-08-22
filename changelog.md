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