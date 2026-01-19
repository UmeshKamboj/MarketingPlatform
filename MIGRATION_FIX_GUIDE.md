# Migration Fix Guide

## Problem Description

The migration system was encountering a `System.Runtime` assembly loading error due to a version mismatch between the globally installed `dotnet-ef` tools (version 10.0.1) and the project's Entity Framework Core version (8.0.0).

Additionally, there was a database synchronization issue where tables exist in the database but the `__EFMigrationsHistory` table doesn't reflect that the migrations have been applied.

## Solutions Applied

### 1. Fixed EF Tools Version Mismatch ✅

**Problem**: `dotnet-ef` version 10.0.1 was installed globally, but the project uses EF Core 8.0.0.

**Solution**:
```bash
# Uninstall the incompatible version
dotnet tool uninstall --global dotnet-ef

# Install the correct version matching the project
dotnet tool install --global dotnet-ef --version 8.0.0
```

**Result**: Migration commands now work without assembly loading errors.

### 2. Database Synchronization Issues

**Problem**: The database has tables (like `SubscriptionPlans`) but migrations show as "Pending", causing migration conflicts when trying to apply them.

**Solutions Available**:

#### Option A: Force Sync Migration History (Recommended if you want to keep data)

Use the provided PowerShell script:
```powershell
.\fix-migrations.ps1
```

Then choose option 2 to force sync the migration history. This will:
1. Generate a SQL script that inserts migration records into `__EFMigrationsHistory`
2. Save it as `sync-migrations.sql`
3. You can then execute it using:
   ```bash
   sqlcmd -S DESKTOP-5OB7VRJ -d MarketingPlatformDb -E -i src\MarketingPlatform.Infrastructure\sync-migrations.sql
   ```

#### Option B: Drop and Recreate Database (Clean slate - WARNING: Deletes all data!)

```bash
cd src/MarketingPlatform.Infrastructure
dotnet ef database drop --startup-project ../MarketingPlatform.API --force
dotnet ef database update --startup-project ../MarketingPlatform.API
```

Or use the PowerShell script and choose option 4.

#### Option C: Manual Migration Application

If you know which specific migrations need to be applied:
```bash
cd src/MarketingPlatform.Infrastructure
dotnet ef database update [MigrationName] --startup-project ../MarketingPlatform.API
```

## Common Migration Commands

### List all migrations
```bash
cd src/MarketingPlatform.Infrastructure
dotnet ef migrations list --startup-project ../MarketingPlatform.API
```

### Create a new migration
```bash
cd src/MarketingPlatform.Infrastructure
dotnet ef migrations add YourMigrationName --startup-project ../MarketingPlatform.API
```

### Apply all pending migrations
```bash
cd src/MarketingPlatform.Infrastructure
dotnet ef database update --startup-project ../MarketingPlatform.API
```

### Rollback to a specific migration
```bash
cd src/MarketingPlatform.Infrastructure
dotnet ef database update [MigrationName] --startup-project ../MarketingPlatform.API
```

### Remove last migration (if not applied)
```bash
cd src/MarketingPlatform.Infrastructure
dotnet ef migrations remove --startup-project ../MarketingPlatform.API
```

## Helper Scripts

### PowerShell Script: `fix-migrations.ps1`

Interactive script that provides options to:
1. Apply pending migrations
2. Force sync migrations history
3. Create a new migration
4. Drop database and recreate from scratch
5. List all migrations and their status

**Usage**:
```powershell
.\fix-migrations.ps1
```

## Prevention Tips

1. **Always use matching EF Tools version**: The `dotnet-ef` global tool version should match your project's EF Core version.

2. **Check migration status before manual changes**: Always run `dotnet ef migrations list` before making manual database changes.

3. **Keep migration history in sync**: If you manually create tables or make schema changes, you'll need to either:
   - Remove those changes and apply migrations properly
   - Manually sync the migration history

4. **Use version control**: Always commit your migration files to git along with the model changes.

5. **Test migrations**: Test migrations on a development database before applying to production.

## Current State

- **EF Tools Version**: 8.0.0 ✅
- **Project EF Core Version**: 8.0.0 ✅
- **Connection String**: `Server=DESKTOP-5OB7VRJ;Database=MarketingPlatformDb;Trusted_Connection=true;MultipleActiveResultSets=true;TrustServerCertificate=true`
- **Pending Migrations**: 18 migrations

### Pending Migrations List
1. `20260117211155_InitialCreate`
2. `20260117213055_AddRefreshToken`
3. `20260118063054_UpdateCampaignMessageEntity`
4. `20260118064723_AddTemplateManagementFields`
5. `20260118170818_AddSchedulingAndAutomation`
6. `20260118172455_AddMessageRoutingAndDeliveryTracking`
7. `20260118172828_AddComplianceAndConsentManagement`
8. `20260118175341_AddRBACSystem`
9. `20260118181947_AddApiRateLimiting`
10. `20260118182817_AddCampaignABTesting`
11. `20260118183433_AddJourneyDesignerFields`
12. `20260118185628_AddOAuth2SSOEntities`
13. `20260118190656_AddEncryptionAuditLog`
14. `20260118195158_AddGlobalConfigurationManagement`
15. `20260118195946_AddSuperAdminAndPrivilegedLogging`
16. `20260118201650_AddKeywordManagementAndPricingFeatures`
17. `20260119072127_UpdateKeywordForeignKeyConstraint`
18. `20260119075924_FixCascadeConstraints`

## Troubleshooting

### Issue: "There is already an object named 'X' in the database"
**Cause**: Database tables exist but migration history doesn't reflect they've been applied.
**Solution**: Use Option A (Force Sync) or Option B (Drop and Recreate) above.

### Issue: "Could not load file or assembly 'System.Runtime'"
**Cause**: Version mismatch between `dotnet-ef` tools and EF Core in project.
**Solution**: Already fixed by downgrading `dotnet-ef` to version 8.0.0.

### Issue: Migration fails with constraint errors
**Cause**: Database state doesn't match what migration expects.
**Solution**: Review the migration file, check database schema, consider dropping and recreating.

### Issue: Can't connect to database
**Cause**: SQL Server not running or connection string incorrect.
**Solution**:
1. Verify SQL Server is running
2. Check connection string in `appsettings.json`
3. Ensure Windows Authentication is working (Trusted_Connection=true)

## Next Steps

1. Choose your preferred solution (Option A or B above)
2. Execute the chosen solution
3. Verify migrations are in sync:
   ```bash
   dotnet ef migrations list --startup-project ../MarketingPlatform.API
   ```
4. All migrations should show as "(Applied)" instead of "(Pending)"

## Additional Resources

- [Entity Framework Core Migrations Documentation](https://learn.microsoft.com/en-us/ef/core/managing-schemas/migrations/)
- [dotnet ef CLI Reference](https://learn.microsoft.com/en-us/ef/core/cli/dotnet)
- Project Documentation: `docs/DATABASE_INITIALIZATION_GUIDE.md`
