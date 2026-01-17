# Marketing Platform

A comprehensive marketing platform built with .NET 8.0 using Clean Architecture principles.

## Solution Structure

- **MarketingPlatform.API** - RESTful API project
- **MarketingPlatform.Web** - ASP.NET Core MVC web application
- **MarketingPlatform.Core** - Domain entities and interfaces
- **MarketingPlatform.Infrastructure** - Data access and external services
- **MarketingPlatform.Application** - Business logic and application services
- **MarketingPlatform.Shared** - Shared utilities and helpers

## Prerequisites

- .NET 8.0 SDK
- SQL Server (LocalDB or full instance)
- Visual Studio 2022 or VS Code

## Getting Started

1. Clone the repository
2. Update connection strings in appsettings.json
3. Run database migrations (when available)
4. Build and run the solution

## Running the Projects

### API Project
```bash
cd src/MarketingPlatform.API
dotnet run
```

### Web Project
```bash
cd src/MarketingPlatform.Web
dotnet run
```

## Architecture

This solution follows Clean Architecture principles with clear separation of concerns:
- Domain-centric design
- Dependency inversion
- Separation of business logic from infrastructure
- Testable and maintainable codebase