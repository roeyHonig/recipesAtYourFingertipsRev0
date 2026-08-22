# Recipes At Your Fingertips — Architecture & Design

## Overview

The project is a web-based recipe management system that enables users to create, organize, search, update, and manage their personal recipe collection.

The application will be built as a server-rendered **ASP.NET Core MVC application using Razor Views**.

The system will use:

- ASP.NET Core MVC
- Razor Views
- Entity Framework Core
- PostgreSQL
- Npgsql EF Core provider
- External authentication through providers such as Google and Apple
- Server-side authentication and authorization
- Relational database persistence

The application will follow a conventional multi-tier architecture.

---

# 1. Application Architecture

The application will use a conventional server-rendered MVC architecture.

```text
Browser
   |
   | HTTP Request
   v
ASP.NET Core
   |
   +----------------------+
   |                      |
   v                      v
Controller             Authentication
   |
   v
Business / Application Logic
   |
   v
Entity Framework Core
   |
   v
PostgreSQL
   |
   +----------------------+
   |
   v
Controller
   |
   v
Razor View
   |
   | HTML Response
   v
Browser
```

The browser will primarily receive rendered HTML.

Business logic, authentication, authorization, database access, and application secrets remain on the server.

---

# 2. MVC Structure

The application will follow the conventional ASP.NET Core MVC structure:

```text
RecipesAtYourFingertips
│
├── Controllers
│   ├── HomeController.cs
│   ├── AccountController.cs
│   └── RecipeController.cs
│
├── Models
│   ├── User.cs
│   ├── ExternalLogin.cs
│   └── Recipe.cs
│
├── Data
│   └── ApplicationDbContext.cs
│
├── Services
│   └── ...
│
├── Views
│   ├── Home
│   ├── Account
│   ├── Recipe
│   └── Shared
│
└── wwwroot
    ├── css
    ├── js
    └── ...
```

The exact structure may evolve as the project develops.

The goal is to keep the architecture conventional and understandable rather than over-engineering the application.

---

# 3. Frontend / UI

The application will initially use simple Razor Views.

No SPA framework will be used.

The initial UI should remain intentionally simple:

- Razor HTML
- Razor tag helpers
- Basic CSS
- Minimal JavaScript where necessary

The UI is not the current focus of the project.

The priority is to establish a clean backend architecture and functional application.

---

# 4. Hebrew and RTL Support

Recipes will initially be written in Hebrew.

The application will therefore use normal Unicode strings throughout the application and database.

There is no need to manually encode Hebrew characters into integer arrays or other custom representations.

For the initial implementation, the recipe pages can simply use RTL layout.

For example:

```html
<html lang="he" dir="rtl">
```

and/or:

```css
body {
    direction: rtl;
    text-align: right;
}
```

A full localization/multi-language architecture is not required at this stage.

---

# 5. Database

The application will use a relational PostgreSQL database.

The initial hosted PostgreSQL provider will be:

**Neon PostgreSQL**

Entity Framework Core will be used for:

- Database access
- Entity mapping
- Relationships
- Database migrations
- Database schema management

The PostgreSQL EF Core provider will be Npgsql.

The application architecture will therefore be:

```text
ASP.NET Core MVC
       |
       v
Entity Framework Core
       |
       v
Npgsql
       |
       v
PostgreSQL
```

Database schema changes should be managed through EF Core migrations rather than manually maintaining SQL schema scripts.

---

# 6. Database Entities

The initial database will contain three primary entities:

```text
User
ExternalLogin
Recipe
```

The relationships are:

```text
User
 |
 | 1
 |
 +-------------------* ExternalLogin
 |
 |
 | 1
 |
 +-------------------* Recipe
```

---

# 7. User

The `User` entity represents an application user.

Conceptually:

```text
Users
--------------------------------
Id              PK
Email
DisplayName
Role
CreatedAt
```

### Important design decision

`Email` is user information.

It is NOT the identity key of the application user.

The application will identify the user's external authentication identity through the `ExternalLogin` entity.

---

# 8. ExternalLogin

The `ExternalLogin` entity represents an identity supplied by an external authentication provider.

Conceptually:

```text
ExternalLogins
--------------------------------
Id                  PK
UserId              FK -> Users.Id
Provider
ProviderUserId
```

Examples of providers:

```text
Google
Apple
```

The combination of:

```text
Provider + ProviderUserId
```

must be unique.

Therefore:

```text
Google + 12345
```

can only exist once.

A database uniqueness constraint should enforce this rule.

---

# 9. External Account Policy

For simplicity, the application will NOT support linking multiple external authentication providers to the same application user.

For example:

```text
Google + 12345
        |
        v
User 42
```

and:

```text
Apple + 98765
        |
        v
User 43
```

Even if the same person controls both external accounts, they will initially be treated as two separate application users.

There will be no account-linking functionality in the initial version.

This decision can be revisited in the future if necessary.

---

# 10. Why ExternalLogin Has Its Own Id

Although `Provider + ProviderUserId` uniquely identifies an external identity, `ExternalLogin` will still have its own internal primary key.

```text
ExternalLogins
--------------------------------
Id                  PK
UserId              FK
Provider
ProviderUserId
```

The combination:

```text
Provider + ProviderUserId
```

will have a UNIQUE constraint.

This keeps relationships straightforward and leaves room for the database model to evolve later.

---

# 11. Recipe

The `Recipe` entity represents a recipe owned by an application user.

Conceptually:

```text
Recipes
--------------------------------
Id              PK
OwnerId         FK -> Users.Id
Title
Ingredients
Instructions
CreatedAt
UpdatedAt
```

Each recipe belongs to exactly one application user.

The relationship is:

```text
User 1
 |
 |
 +---- * Recipe
```

The `OwnerId` property identifies the user who created the recipe.

---

# 12. Recipe Ownership

Recipe ownership is an important authorization rule.

A user may only modify recipes that belong to that user.

The application must therefore check:

```text
Recipe.OwnerId == CurrentUser.Id
```

before allowing operations such as:

- Edit
- Delete
- Update

Authentication alone is not sufficient.

For example:

```text
User is authenticated
        |
        v
Find requested recipe
        |
        v
Compare Recipe.OwnerId
with CurrentUser.Id
        |
        +----------------+
        |                |
       YES               NO
        |                |
        v                v
      Allow             Deny
```

Ownership checks must be performed server-side.

Hiding an Edit or Delete button in the UI is not considered a security mechanism.

The authorization check must also occur when processing the corresponding POST request.

---

# 13. Authentication

The application will support external authentication providers.

Initial providers:

```text
Google
Apple
```

The external provider is responsible for authenticating the user.

The application is responsible for maintaining its own application user record.

Conceptually:

```text
Browser
   |
   v
ASP.NET Core
   |
   v
Google / Apple
   |
   v
External identity
   |
   v
ExternalLogin
   |
   v
Application User
   |
   v
ASP.NET Core authentication
```

---

# 14. Authentication Cookies vs JWT

The application is a server-rendered ASP.NET Core MVC application.

Therefore, the application will use ASP.NET Core's normal browser authentication mechanism rather than creating its own JWT for every browser session.

The initial architecture will NOT use:

```text
External Provider
       |
       v
Create custom JWT
       |
       v
Browser stores JWT
       |
       v
JWT sent with every request
```

Instead, the application will use server-side ASP.NET Core authentication with an authentication cookie.

Conceptually:

```text
Google / Apple
      |
      v
ASP.NET Core verifies identity
      |
      v
Find/Create Application User
      |
      v
ASP.NET Core Authentication Cookie
      |
      v
Authenticated browser session
```

JWT authentication can be reconsidered in the future if the project develops a separate REST API or another type of client that requires token-based authentication.

---

# 15. Authorization

Authentication answers:

> Who is the user?

Authorization answers:

> What is the user allowed to do?

ASP.NET Core authorization will be used to protect authenticated areas.

For example:

```csharp
[Authorize]
```

can require a user to be authenticated.

Role-based authorization can be used where appropriate:

```csharp
[Authorize(Roles = "Admin")]
```

However, recipe ownership is a separate authorization concern.

A user being authenticated does not automatically mean they can modify every recipe.

---

# 16. Roles

Users will have an application-level role.

Initially the application may have a simple role such as:

```text
User
```

Additional roles, such as:

```text
Admin
```

can be introduced when required.

Roles will be application concepts rather than roles supplied directly by Google or Apple.

---

# 17. Public Recipe Pages

Recipe details must be publicly accessible.

A person does NOT need to:

- Register
- Log in
- Have an application account

to view a recipe.

Conceptually:

```text
GET /Recipe/Details/123
```

is public.

This allows recipes to be shared with other people.

For example:

```text
User A creates recipe 123

        |
        v

https://example.com/Recipe/Details/123

        |
        +---- Logged-in user -> can view
        |
        +---- Logged-out user -> can view
```

---

# 18. Recipe CRUD Access

Recipe creation requires authentication.

Recipe modification requires:

1. The user must be authenticated.
2. The recipe must belong to the current user.

Conceptually:

```text
Create
    |
    +--> Authenticated user required


Edit
    |
    +--> Authenticated user required
    |
    +--> User must own recipe


Delete
    |
    +--> Authenticated user required
    |
    +--> User must own recipe
```

---

# 19. Recipe Search

Recipe search is NOT a global search across all recipes.

Authenticated users may search only their own recipes.

Conceptually:

```text
Current User
      |
      v
CurrentUser.Id
      |
      v
Recipes
      |
      v
WHERE OwnerId == CurrentUser.Id
      |
      v
Search/filter user's recipes
```

A user must never receive another user's private recipe collection through the search functionality.

The database query should enforce ownership rather than relying on filtering in the UI.

---

# 20. Initial Routes

The application will eventually have routes conceptually similar to:

```text
/Recipe/Details/{id}
```

Public recipe page.

```text
/Recipe/MyRecipes
```

Authenticated user's recipes.

```text
/Recipe/Create
```

Create a recipe.

```text
/Recipe/Edit/{id}
```

Edit an owned recipe.

```text
/Recipe/Delete/{id}
```

Delete an owned recipe.

```text
/Account/Login
```

Login.

```text
/Account/Logout
```

Logout.

The exact routing structure can evolve during implementation.

---

# 21. Security Principles

The application should follow these principles:

### Server-side authorization

Authorization decisions must be made on the server.

### Ownership enforcement

Recipe ownership must be verified for every protected recipe operation.

### External identity separation

External provider identities are separate from application users.

### No secrets in the browser

Database credentials, provider secrets, and other sensitive configuration must remain server-side.

### Database constraints

Important invariants should be enforced by the database where appropriate.

For example:

```text
UNIQUE(Provider, ProviderUserId)
```

---

# 22. Initial Architecture Diagram

The overall architecture is:

```text
                         ┌─────────────┐
                         │   Google    │
                         └──────┬──────┘
                                │
                         ┌──────▼──────┐
                         │    Apple    │
                         └──────┬──────┘
                                │
                                v
                     ┌────────────────────┐
                     │   ASP.NET Core     │
                     │       MVC          │
                     └─────────┬──────────┘
                               │
              ┌────────────────┼─────────────────┐
              │                │                 │
              v                v                 v
       Authentication    Controllers          Services
              │                │                 │
              │                └────────┬────────┘
              │                         │
              │                         v
              │                  Entity Framework
              │                         Core
              │                          │
              │                          v
              │                    PostgreSQL
              │                          │
              │                          v
              │                       Neon
              │
              v
       Authentication
           Cookie

```

---

# 23. Database Relationship Diagram

```text
┌──────────────────────────────┐
│            Users             │
├──────────────────────────────┤
│ Id              PK           │
│ Email                        │
│ DisplayName                  │
│ Role                         │
│ CreatedAt                    │
└──────────────┬───────────────┘
               │
               │ 1
               │
       ┌───────┴────────┐
       │                │
       │ *              │ *
       v                v
┌─────────────────┐  ┌─────────────────────────┐
│ ExternalLogins  │  │         Recipes         │
├─────────────────┤  ├─────────────────────────┤
│ Id       PK     │  │ Id              PK       │
│ UserId   FK     │  │ OwnerId         FK       │
│ Provider        │  │ Title                    │
│ ProviderUserId  │  │ Ingredients              │
└─────────────────┘  │ Instructions             │
                     │ CreatedAt                │
                     │ UpdatedAt                │
                     └─────────────────────────┘

ExternalLogins:

UNIQUE(Provider, ProviderUserId)
```

---

# 24. Development Strategy

The application should be built incrementally.

The initial development sequence is:

```text
1. PostgreSQL setup
        ↓
2. EF Core setup
        ↓
3. ApplicationDbContext
        ↓
4. User entity
        ↓
5. ExternalLogin entity
        ↓
6. Recipe entity
        ↓
7. Entity relationships
        ↓
8. EF Core migration
        ↓
9. Create PostgreSQL database schema
        ↓
10. Google authentication
        ↓
11. Apple authentication
        ↓
12. Authorization
        ↓
13. Recipe functionality
```

The database foundation will therefore be established before implementing external authentication.

---

# 25. Current Decisions

The following architectural decisions have been made:

| Decision | Choice |
|---|---|
| Application type | ASP.NET Core MVC |
| UI | Razor Views |
| Frontend framework | None initially |
| Database | PostgreSQL |
| Hosted DB | Neon |
| ORM | Entity Framework Core |
| PostgreSQL provider | Npgsql |
| Authentication | External providers |
| Initial providers | Google + Apple |
| Browser authentication | ASP.NET Core authentication cookie |
| Custom JWT | Not initially required |
| User identity | Internal `User.Id` |
| External identity | `Provider + ProviderUserId` |
| External identity uniqueness | UNIQUE constraint |
| Account linking | Not supported initially |
| Recipe ownership | `Recipe.OwnerId` |
| Public recipe viewing | Yes |
| Recipe CRUD | Owner only |
| Recipe search | Current user's recipes only |
| Recipe language | Hebrew initially |
| Text encoding | Unicode |
| Initial layout | RTL |
| UI complexity | Simple Razor UI |

---

# 26. Immediate Next Step

The next implementation task is to establish the PostgreSQL database and EF Core infrastructure.

The first implementation steps will be:

1. Configure PostgreSQL.
2. Install/configure EF Core and Npgsql.
3. Create `ApplicationDbContext`.
4. Create the initial entities:
   - `User`
   - `ExternalLogin`
   - `Recipe`
5. Configure their relationships.
6. Configure the unique `(Provider, ProviderUserId)` constraint.
7. Create the first EF Core migration.
8. Apply the migration to PostgreSQL.
9. Verify the resulting database schema.

Authentication will be implemented after the database foundation is working.