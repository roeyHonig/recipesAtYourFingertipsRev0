# Recipes At Your Fingertips – Rev 0

## Overview

**Recipes At Your Fingertips** is a web-based recipe management system built with ASP.NET Core MVC and PostgreSQL.

Registered users can manage their personal recipe collections, including:

- Creating recipes
- Searching their own recipes
- Editing recipes
- Deleting recipes
- Viewing individual recipes

Individual recipe pages are public and read-only, allowing a recipe to be shared through its URL.

## Production

The production application is hosted on DigitalOcean App Platform:

https://octopus-app-ijx44.ondigitalocean.app/

## Requirements

To run the project locally, the developer needs:

- .NET 10 SDK
- Access to the PostgreSQL database
- The required local User Secrets

The project uses Entity Framework Core for database access and migrations.

## Local Configuration

The following secrets are required for running the application locally:

```text
ConnectionStrings:DefaultConnection

Authentication:Google:ClientId
Authentication:Google:ClientSecret

Authentication:Microsoft:ClientId
Authentication:Microsoft:ClientSecret
```

The required secret values are managed by **Roey Honig**, the repository owner and project administrator.

Do not commit secrets or other sensitive configuration values to the repository.

## Local Authentication Configuration

When running the application locally, the developer must make sure that the local authentication callback URLs are registered for both external identity providers.

The required redirect URI depends on the local URL and HTTPS port used by the application.

For example, if the application is running locally at:

```text
https://localhost:7003
```

the Google redirect URI should be:

```text
https://localhost:7003/signin-google
```

and the Microsoft redirect URI should be:

```text
https://localhost:7003/signin-microsoft
```

The redirect URIs must be configured in both external identity provider applications:

- **Google Cloud Console** – OAuth client configuration.
- **Microsoft Entra admin center** – application registration and authentication configuration.

The local port may be different on another developer's machine. The developer should therefore use the actual HTTPS URL and port configured for their local application.

If the required local redirect URI is not already registered, contact **Roey Honig**, the project administrator, to have the URI added to the appropriate Google and Microsoft application configurations.

> **Important:** The redirect URI must match the URL used by the application exactly, including the protocol (`http` or `https`), hostname, port, and callback path.

## Running Locally

Restore the project dependencies:

```bash
dotnet restore
```

Build the project:

```bash
dotnet build
```

Run the application:

```bash
dotnet run
```

The application will display the local URLs on which it is running.

## Database

After configuring the PostgreSQL connection string, apply the Entity Framework Core migrations:

```bash
dotnet ef database update
```

This creates or updates the required database structure.

## Production Deployment

The production application is hosted using **DigitalOcean App Platform**.

Production configuration contains the required database connection string and authentication configuration through environment variables.

Local User Secrets are used only for local development.

The production environment must contain the equivalent configuration for:

```text
ConnectionStrings:DefaultConnection

Authentication:Google:ClientId
Authentication:Google:ClientSecret

Authentication:Microsoft:ClientId
Authentication:Microsoft:ClientSecret
```

The production authentication redirect URIs are configured separately from the local development redirect URIs.

## External Services

The project uses the following external services:

- **DigitalOcean App Platform** – production hosting
- **Neon PostgreSQL** – PostgreSQL database hosting
- **Google Cloud Console** – Google authentication configuration
- **Microsoft Entra** – Microsoft authentication configuration

Access to the relevant project configurations and credentials is managed by **Roey Honig**.

## Security

Do not commit any of the following to the repository:

- Client secrets
- Database connection strings
- Passwords
- API keys
- Other sensitive configuration values

Use .NET User Secrets for local development and the appropriate environment configuration for production.