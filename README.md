# Marketing Platform - SMS, MMS & Email Marketing Solution

## Overview
A robust, enterprise-grade SMS, MMS & Email Marketing Platform built with ASP.NET Core 8.0, SQL Server, and Bootstrap 5.

## Technology Stack
- **Framework**: ASP.NET Core 8.0
- **Database**: SQL Server with Entity Framework Core
- **Frontend**: Bootstrap 5, jQuery, Font Awesome
- **Authentication**: ASP.NET Core Identity with JWT
- **Background Jobs**: Hangfire
- **Logging**: Serilog
- **Payments**: Stripe
- **API Documentation**: Swagger/OpenAPI

## Projects

### MarketingPlatform.API
RESTful API for all platform operations.

### MarketingPlatform.Web
Web application (MVC) consuming the API.

### MarketingPlatform.Core
Domain entities, interfaces, enums, and constants.

### MarketingPlatform.Infrastructure
Data access, repositories, and external service integrations.

### MarketingPlatform.Application
Business logic, services, DTOs, and validators.

### MarketingPlatform.Shared
Shared utilities and helpers.

## Getting Started

### Prerequisites
- .NET 8.0 SDK
- SQL Server (LocalDB, Express, or Full)
- Visual Studio 2022 or VS Code
- Node.js (optional, for frontend tooling)

### Setup Instructions

1. **Clone the repository**
   ```bash
   git clone https://github.com/UmeshKamboj/MarketingPlatform.git
   cd MarketingPlatform
   ```

2. **Update connection strings**
   - Update `appsettings.json` in both API and Web projects with your SQL Server connection string

3. **Apply database migrations**
   ```bash
   cd src/MarketingPlatform.Infrastructure
   dotnet ef database update --startup-project ../MarketingPlatform.API
   ```
   
   Or run the API which will automatically migrate and seed the database:
   ```bash
   cd src/MarketingPlatform.API
   dotnet run
   ```

4. **Run the API**
   ```bash
   cd src/MarketingPlatform.API
   dotnet run
   ```
   API will be available at: https://localhost:7001

5. **Run the Web Application**
   ```bash
   cd src/MarketingPlatform.Web
   dotnet run
   ```
   Web app will be available at: https://localhost:7002

## Configuration

### API Settings (appsettings.json)
- **ConnectionStrings**: Database connection
- **JwtSettings**: JWT authentication configuration
- **Stripe**: Payment gateway keys
- **Serilog**: Logging configuration

### Web Settings (appsettings.json)
- **ConnectionStrings**: Database connection
- **ApiSettings**: API base URL
- **Stripe**: Publishable key

## Database Setup

The database is automatically migrated and seeded when you run the API. However, you can also manually manage migrations:

### Create Migration
```bash
cd src/MarketingPlatform.Infrastructure
dotnet ef migrations add MigrationName --startup-project ../MarketingPlatform.API
```

### Update Database
```bash
cd src/MarketingPlatform.Infrastructure
dotnet ef database update --startup-project ../MarketingPlatform.API
```

### Verify Setup
After running the API, verify the following:
- Database `MarketingPlatformDb` is created
- All tables are created successfully
- Seed data is inserted:
  - 3 roles: Admin, User, Manager
  - Admin user: admin@marketingplatform.com / Admin@123456
  - 3 subscription plans: Free, Pro, Enterprise
  - 2 message providers: Twilio SMS, SendGrid Email

## Features (Planned)

- ✅ Task 1.1: Solution structure and core projects
- ✅ Task 1.2: Database foundation ← **Current**
- ⏳ Task 1.3: Authentication & multi-tenancy
- ⏳ Campaign management
- ⏳ Contact & group management
- ⏳ Template management
- ⏳ Keyword campaigns
- ⏳ Automation & workflows
- ⏳ Analytics & reporting
- ⏳ Billing & subscriptions
- ⏳ Super admin platform

## Project Status
🚧 **In Development** - Task 1.2 Complete

## License
MIT License

## Contact
For questions or support, contact: support@marketingplatform.com
