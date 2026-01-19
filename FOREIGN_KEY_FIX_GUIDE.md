# Foreign Key Constraint Fix Guide

## Problem Description

You're encountering this error:
```
Foreign key 'FK_CampaignAnalytics_Campaigns_CampaignId' references invalid table 'Campaigns'.
Could not create constraint or index. See previous errors.
```

This happens when migrations try to create foreign keys before the referenced tables exist, which is a table creation ordering issue in Entity Framework migrations.

## Root Cause

The problem occurs when:
1. Multiple migrations exist that were created at different times
2. The migrations try to create tables in an order that doesn't respect foreign key dependencies
3. Entity Framework generates migrations with foreign keys pointing to tables not yet created

## Solution: Clean Slate Approach (Recommended)

The most reliable fix is to start with a fresh migration that properly orders all table creations.

### Option 1: Automated Scripts (Easiest)

I've created two scripts for you:

#### Windows Command Prompt:
```cmd
reset-and-recreate-database.cmd
```

#### PowerShell (Recommended):
```powershell
.\reset-and-recreate-database.ps1
```

These scripts will:
1. Drop the existing database (⚠️ all data will be lost)
2. Delete all migration files
3. Create a single fresh `InitialCreate` migration
4. Apply it to create a clean database

### Option 2: Manual Step-by-Step

If you prefer to do it manually or the scripts don't work:

#### Step 1: Navigate to Infrastructure Project
```cmd
cd "C:\Users\Monil Vy\.claude-worktrees\TextingPro\brave-euler\src\MarketingPlatform.Infrastructure"
```

#### Step 2: Drop the Database
```cmd
dotnet ef database drop --startup-project ..\MarketingPlatform.API --force
```

#### Step 3: Delete All Migrations
```cmd
rmdir /s /q Migrations
```

Or in PowerShell:
```powershell
Remove-Item -Path Migrations -Recurse -Force
```

#### Step 4: Create Fresh Migration
```cmd
dotnet ef migrations add InitialCreate --startup-project ..\MarketingPlatform.API
```

#### Step 5: Apply Migration
```cmd
dotnet ef database update --startup-project ..\MarketingPlatform.API
```

## Alternative: Keep Existing Data

If you need to keep existing data, this is more complex:

### Option A: Manual Table Creation Order Fix

1. Export your data first:
```sql
-- Use SQL Server Management Studio to script data
-- Right-click database → Tasks → Generate Scripts
-- Select "Schema and Data"
```

2. Follow the "Clean Slate Approach" above

3. Re-import your data using the generated scripts

### Option B: Fix Migration Files Manually

This is advanced and error-prone. Only do this if you're comfortable with C# and EF Core:

1. Open the `InitialCreate` migration file
2. Manually reorder the `CreateTable` calls to ensure parent tables are created before child tables
3. The order should be:
   - Identity tables (AspNetUsers, AspNetRoles, etc.)
   - Independent tables (no foreign keys)
   - Tables with foreign keys to independent tables
   - Tables with foreign keys to other dependent tables

This is tedious and error-prone, so the Clean Slate approach is strongly recommended.

## Prevention: Best Practices

To avoid this issue in the future:

### 1. Create Focused Migrations

Instead of one giant migration, create smaller, focused migrations:

```cmd
# Good - specific migrations
dotnet ef migrations add AddCampaignTables --startup-project ..\MarketingPlatform.API
dotnet ef migrations add AddAnalyticsTables --startup-project ..\MarketingPlatform.API
dotnet ef migrations add AddComplianceTables --startup-project ..\MarketingPlatform.API
```

### 2. Test Migrations Before Committing

Always test migrations on a development database:

```cmd
# Create migration
dotnet ef migrations add MyMigration --startup-project ..\MarketingPlatform.API

# Test on dev database
dotnet ef database update --startup-project ..\MarketingPlatform.API

# If issues occur, remove the migration
dotnet ef migrations remove --startup-project ..\MarketingPlatform.API
```

### 3. Use Fluent API for Complex Relationships

In entity configurations, explicitly define relationships:

```csharp
public class CampaignAnalyticsConfiguration : IEntityTypeConfiguration<CampaignAnalytics>
{
    public void Configure(EntityTypeBuilder<CampaignAnalytics> builder)
    {
        builder.HasOne(ca => ca.Campaign)
            .WithMany(c => c.Analytics)
            .HasForeignKey(ca => ca.CampaignId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
```

### 4. Review Generated Migrations

Always review the generated migration files before applying:

```cmd
# After creating a migration, check the file
code src\MarketingPlatform.Infrastructure\Migrations\[timestamp]_MyMigration.cs
```

## Troubleshooting

### Error: "Build Failed"

If you get build errors when creating migrations:

```cmd
# Clean and rebuild
dotnet clean
dotnet build

# Then try migration again
dotnet ef migrations add InitialCreate --startup-project ..\MarketingPlatform.API
```

### Error: "Cannot connect to database"

Check that:
1. SQL Server is running
2. Connection string is correct in `appsettings.json`:
   ```
   Server=DESKTOP-5OB7VRJ;Database=MarketingPlatformDb;Trusted_Connection=true;
   MultipleActiveResultSets=true;TrustServerCertificate=true
   ```
3. You have permissions to create/drop databases

### Error: "Project file does not exist"

Make sure you're in the correct directory. The paths should be:
- Current directory: `src\MarketingPlatform.Infrastructure`
- Startup project: `..\MarketingPlatform.API` (relative path)

### Migrations Still Fail After Clean Slate

If migrations still fail after starting fresh:

1. Check for circular dependencies in entity configurations
2. Verify all entity configurations in `Configuration` folder
3. Ensure navigation properties are properly defined on both sides of relationships
4. Check for typos in foreign key property names

## Entity Configuration Files

Your project uses `IEntityTypeConfiguration<T>` for entity configurations. These are in:
```
src\MarketingPlatform.Infrastructure\Configurations\
```

Common issues in configurations:
- Incorrect foreign key names
- Circular relationships without proper navigation configuration
- Missing required relationships

Example of proper configuration:
```csharp
// CampaignAnalyticsConfiguration.cs
builder.HasOne(ca => ca.Campaign)
    .WithMany(c => c.Analytics)
    .HasForeignKey(ca => ca.CampaignId)
    .OnDelete(DeleteBehavior.Restrict); // or Cascade, depends on requirements
```

## Quick Command Reference

```cmd
# List migrations
dotnet ef migrations list --startup-project ..\MarketingPlatform.API

# Create migration
dotnet ef migrations add MigrationName --startup-project ..\MarketingPlatform.API

# Apply migrations
dotnet ef database update --startup-project ..\MarketingPlatform.API

# Rollback to specific migration
dotnet ef database update MigrationName --startup-project ..\MarketingPlatform.API

# Remove last migration (if not applied)
dotnet ef migrations remove --startup-project ..\MarketingPlatform.API

# Drop database
dotnet ef database drop --startup-project ..\MarketingPlatform.API --force

# Generate SQL script (without applying)
dotnet ef migrations script --startup-project ..\MarketingPlatform.API --output migration.sql
```

## Next Steps

1. **Choose your approach**: Clean Slate (recommended) or Keep Data
2. **Run the script**: Use `reset-and-recreate-database.ps1` or manual steps
3. **Verify**: Check that all tables are created properly
4. **Test**: Run your application to ensure everything works
5. **Document**: Note what caused the issue to prevent it in the future

## Need Help?

If you continue to have issues:

1. Check the generated migration file for table creation order
2. Look for circular dependencies in entity configurations
3. Verify all foreign key relationships are properly configured
4. Consider simplifying complex relationships

---

**Remember**: Always backup your database before running these operations if you have important data!
