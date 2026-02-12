# AGENTS.md

This file provides guidance to WARP (warp.dev) when working with code in this repository.

## Project Overview

Events management application with a .NET 9.0 API backend and SvelteKit 5 frontend.

## Build & Run Commands

### Server (.NET API)

```powershell
# Build
dotnet build src/server/eventsAPI/eventsAPI.csproj

# Run (starts at http://localhost:5000 by default)
dotnet run --project src/server/eventsAPI/eventsAPI.csproj

# Run with hot reload
dotnet watch --project src/server/eventsAPI/eventsAPI.csproj
```

### Client (SvelteKit)

```powershell
# Install dependencies (run from src/client/events)
npm install --prefix src/client/events

# Development server
npm run dev --prefix src/client/events

# Build for production
npm run build --prefix src/client/events

# Type checking
npm run check --prefix src/client/events

# Lint & format
npm run lint --prefix src/client/events
npm run format --prefix src/client/events
```

### Database (EF Core Migrations)

```powershell
# Add migration (run from src/server/eventsAPI)
dotnet ef migrations add <MigrationName> --project ../events.infra

# Apply migrations
dotnet ef database update --project ../events.infra

# Remove last migration
dotnet ef migrations remove --project ../events.infra
```

## Architecture

### Server Structure

- **`src/server/eventsAPI/`** - ASP.NET Core minimal API
  - Endpoints are feature-based classes implementing `IEndpoint`
  - Register new endpoints in `RegisterEndpoints.cs` using `builder.Map<YourEndpoint>()`
  - OpenAPI docs available at `/scalar/v1` in development

- **`src/server/events.infra/`** - Data access layer
  - `EventsDbContext` - EF Core context with PostgreSQL
  - `Model/` - Domain entities (Event, Category, Feature)
  - `Migrations/` - EF Core migrations

### Endpoint Pattern

Each endpoint is a class implementing `IEndpoint`:

```csharp
public class YourEndpoint : IEndpoint
{
    public static void MapEndPoint(IEndpointRouteBuilder builder)
        => builder.MapGroup("your-group")
            .WithTags("Tag")
            .MapPost("/route", Handle)
            .WithName("EndpointName");

    private static async Task<IResult> Handle(
        [FromBody] RequestDto request,
        [FromServices] EventsDbContext ctx,
        CancellationToken ct)
    {
        // Implementation
    }
}
```

### Client Structure

- **`src/client/events/`** - SvelteKit app with Svelte 5
  - Uses Tailwind CSS for styling
  - TypeScript enabled
  - i18n via Paraglide (`npm run machine-translate` to auto-translate)

### Database

PostgreSQL with connection string in `appsettings.json` (key: `ConnectionStrings:default`).

Domain model relationships:
- Event → Category (many-to-one, nullable)
- Event ↔ Feature (many-to-many)
